using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益防御值的buff</summary>
    public class BuffGainDefence : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceGain">增益的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainDefence(Character buffee, Character buffer, int time, float defenceGain, Game game)
            : base(buffee, buffer, time,
                defenceGain >= 0
                    ? string.Format("坚固：防御增加{0}%", (int) (defenceGain*100))
                    : string.Format("破碎：防御降低{0}%", (int) (-defenceGain*100)), defenceGain > 0, game)
        {
            BuffAffect += (bee, ber) => bee._defenceX *= (1 + defenceGain);
            BuffCancels += (bee, ber) => bee._defenceX /= (1 + defenceGain);
        }

    }
}
