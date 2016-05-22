using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed
{
    /// <summary>增益命中率值的buff</summary>
    public sealed class BuffGainHitRate : BuffGainProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="hitRateGain">增益的命中率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainHitRate(Character buffee, Character buffer, int time, double hitRateGain, Game game)
            : base(buffee, buffer, time, "HitRate", hitRateGain, "精准：命中率", "眼花：命中率", game)
        {

        }
    }
}
