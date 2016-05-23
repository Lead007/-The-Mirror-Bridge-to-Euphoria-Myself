using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation.Buffs.Add.Sealed
{
    /// <summary>增加 的buff</summary>
    public sealed class BuffAddHitRate : BuffAddProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="hitRateAdd">增加的命中率率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddHitRate(Character buffee, Character buffer, int time, int hitRateAdd, Game game)
            : base(buffee, buffer, time, "HitRate", hitRateAdd, "精准：命中率", "眼花：命中率", game)
        {

        }
    }
}
