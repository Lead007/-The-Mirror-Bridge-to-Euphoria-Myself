using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增加攻击范围的buff</summary>
    public class BuffAddAttackRange : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackRangeAdd">增加的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddAttackRange(Character buffee, Character buffer, int time, int attackRangeAdd, Game game)
            : base(buffee, buffer, time,
                attackRangeAdd >= 0
                    ? string.Format("远程：攻击范围增加{0}", attackRangeAdd)
                    : string.Format("进程：攻击范围降低{0}", -attackRangeAdd), game)
        {
            BuffAffect += (bee, ber) => bee._attackRangeAdd += attackRangeAdd;
            BuffCancels += (bee, ber) => bee._attackRangeAdd -= attackRangeAdd;
        }

    }
}
