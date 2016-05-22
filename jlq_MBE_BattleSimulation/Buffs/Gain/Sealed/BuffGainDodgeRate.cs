using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed
{
    /// <summary>增益闪避值的buff</summary>
    public sealed class BuffGainDodgeRate : BuffGainProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="dodgeRateGain">增益的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainDodgeRate(Character buffee, Character buffer, int time, double dodgeRateGain, Game game)
            : base(buffee, buffer, time, "DodgeRate", dodgeRateGain, "幻影：闪避", "迟钝：闪避", game)
        {

        }

    }
}
