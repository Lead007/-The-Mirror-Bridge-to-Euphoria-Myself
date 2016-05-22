using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.Add.Sealed
{
    /// <summary>增加攻击范围的buff</summary>
    public sealed class BuffAddAttackRange : BuffAddProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackRangeAdd">增加的攻击范围值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddAttackRange(Character buffee, Character buffer, int time, int attackRangeAdd, Game game)
            : base(buffee, buffer, time, "AttackRange", attackRangeAdd, "远程：攻击范围", "近程：攻击范围", game)
        {
            
        }
    }
}
