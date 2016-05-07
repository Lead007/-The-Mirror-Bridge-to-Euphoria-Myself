using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益造成的伤害buff</summary>
    public class BuffGainDoDamage : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="damageGain">受伤增益，若负则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainDoDamage(Character buffee, Character buffer, int time, float damageGain, Game game)
            : base(buffee, buffer, time,
                damageGain > 0
                    ? string.Format("力量：伤害增加{0}%", (int) (damageGain*100))
                    : string.Format("无力：伤害降低{0}%", (int) (-damageGain*100)), game)
        {
            BuffAffect += (bee, ber) =>
            {
                _temp = (DDoAttack)bee.HandleDoAttack.Clone();
                bee.HandleDoAttack = (target, times) => _temp(target, Math.Max(0, 1 + damageGain));
            };
            BuffCancels += (bee, ber) => bee.HandleDoAttack = _temp;
        }

        private DDoAttack _temp;
    }
}
