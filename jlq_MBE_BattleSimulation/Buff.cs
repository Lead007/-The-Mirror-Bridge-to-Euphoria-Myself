using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>
    /// 没有参数的buff；
    /// 默认的buff类可能会利用buff者自身与施加buff者的相关数据，可以对施加buff者造成影响；
    /// 更复杂的buff需要自建子类；
    /// 每个角色的buff效果自己编写，会有一些静态的buff供调用；
    /// 开发者备注：静态的buff需要有一系列参数，每人使用lambda表达式代入参数的具体值；
    /// </summary>
    public abstract class Buff
    {
        /// <summary>
        /// 将Interval设为此值，则buff无限剩余时间
        /// </summary>
        public const int Infinite = Int32.MaxValue;
        /// <summary>buff剩余时间</summary>
        public int Time { get; protected set; }
        /// <summary>buff执行的阶段</summary>
        public readonly Section ExecuteSection;
        /// <summary>buff名称</summary>
        public readonly string Name;

        /// <summary>buff效果的委托对象</summary>
        protected DBuffAffect BuffAffect;
        /// <summary>取消buff的委托对象</summary>
        protected DBuffCancel BuffCancels;
        /// <summary>游戏对象</summary>
        protected Game game;

        /// <summary>buff发出者</summary>
        public Character Buffer;
        /// <summary>buff承受者</summary>
        public Character Buffee;

        /// <summary>Buff类的构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="executeSection">buff执行的阶段</param>
        /// <param name="name">buff名称</param>
        /// <param name="game">游戏对象</param>
        protected Buff(Character buffee, Character buffer, int time, Section executeSection, string name, Game game)
        {
            this.Buffer = buffer;
            this.Buffee = buffee;
            this.Time = time;
            this.ExecuteSection = executeSection;
            this.Name = name;
            this.game = game;
            BuffCancels = Cancel;
        }

        /// <summary>buff引发</summary>
        public void BuffTrigger()
        {
            BuffAffect(Buffee, Buffer);
        }

        /// <summary>buff剩余时间减少</summary>
        /// <param name="time">减少的时间</param>
        /// <returns>减少后剩余时间是否小于等于0</returns>
        public bool Round(int time)
        {
            if (Time < time)
            {
                Time = 0;
                return true;
            }
            if (Time == Infinite) return false;
            Time -= time;
            return false;
        } 

        /// <summary>buff结束</summary>
        public void BuffEnd()
        {
            BuffCancels(Buffee, Buffer);
        }

        /// <summary>重写object类的ToString方法</summary>
        /// <returns>转化为字符串的结果</returns>
        public override string ToString() => string.Format("{0} By:{1} 剩余时间：{2}", Name, Buffer.Data.Name, Time);
        /// <summary>从buff列表中删除buff</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        protected void Cancel(Character buffee,Character buffer)
        {
            buffee.BuffList.Remove(this);
        }
    }
}
