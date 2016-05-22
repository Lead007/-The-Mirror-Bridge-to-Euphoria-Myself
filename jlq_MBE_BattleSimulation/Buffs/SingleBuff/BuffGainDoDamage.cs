using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.SingleBuff
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
                    ? string.Format("力量：伤害增加{0}%", (int)(damageGain*100))
                    : string.Format("无力：伤害降低{0}%", (int)(-damageGain*100)), damageGain > 0, game)
        {
            _damageGain = damageGain;
        }

        private DDoAttack _temp;
        private readonly float _damageGain;

        protected override void BuffAffect()
        {
            _temp = (DDoAttack)Buffee.HandleDoingAttack.Clone();
            Buffee.HandleDoingAttack = (target, times) => _temp(target, Math.Max(0, 1 + _damageGain));
        }

        protected override void Cancel()
        {
            Buffee.HandleDoingAttack = _temp;
            base.Cancel();
        }
    }
}
