using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.Add
{
    /// <summary>增加角色属性的Buff抽象类</summary>
    public abstract class BuffAddProperty:BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="propertyName">属性名（首字母大写）</param>
        /// <param name="add">增量</param>
        /// <param name="positiveName">正面效果名（未格式化）</param>
        /// <param name="negativeName">负面效果名（未格式化）</param>
        /// <param name="game">游戏对象</param>
        protected BuffAddProperty(Character buffee, Character buffer, int time, string propertyName, int add,
            string positiveName, string negativeName, Game game)
            : base(buffee, buffer, time,
                string.Format((add > 0 ? positiveName + "+" : negativeName + "-") + "{0}", Math.Abs(add)), add > 0, game)
        {
            _setProperty = typeof (Character).GetProperty(propertyName + "Add").SetMethod;
            _add = add;
        }

        private readonly int _add;
        private readonly MethodInfo _setProperty;

        protected override void BuffAffect()
        {
            _setProperty.Invoke(Buffee, new object[] {_add});
        }

        protected override void Cancel()
        {
            _setProperty.Invoke(Buffee, new object[] {-_add});
            base.Cancel();
        }
    }
}
