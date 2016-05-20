using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益防御值的buff</summary>
    public sealed class BuffGainDefence : BuffGainProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceGain">增益的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainDefence(Character buffee, Character buffer, int time, double defenceGain, Game game)
            : base(buffee, buffer, time, "Defence", defenceGain, "坚固：防御", "破碎：防御", game)
        {
            
        }
    }
}
