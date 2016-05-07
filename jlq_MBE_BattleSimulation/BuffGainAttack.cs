using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益攻击值的buff</summary>
    public class BuffGainAttack : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackGain">增益的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainAttack(Character buffee, Character buffer, int time, float attackGain, Game game)
            : base(buffee, buffer, time,
                attackGain >= 0
                    ? string.Format("锋利：攻击增加{0}%", (int) (attackGain*100))
                    : string.Format("钝化：攻击降低{0}%", (int) (-attackGain*100)), game)
        {
            BuffAffect += (bee, ber) => bee._attackX *= (1 + attackGain);
            BuffCancels += (bee, ber) => bee._attackX /= (1 + attackGain);
        }

    }
}
