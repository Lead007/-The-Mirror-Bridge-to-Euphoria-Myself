using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>获得时立即执行BuffAffect委托的Buff</summary>
    public abstract class BuffExecuteImmediately:Buff
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="name">buff名称</param>
        /// <param name="game">游戏对象</param>
        protected BuffExecuteImmediately(Character buffee, Character buffer, int time, string name, Game game)
            : base(buffee, buffer, time, name, game)
        {
            
        }

    }
}
