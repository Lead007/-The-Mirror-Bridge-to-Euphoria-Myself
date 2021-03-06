﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_BaseBuffs.SingleBuff
{
    /// <summary>增益受到的伤害buff</summary>
    public class BuffGainBeDamaged : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="damageGain">受伤增益，若负则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainBeDamaged(Character buffee, Character buffer, int time, float damageGain, Game game)
            : base(buffee, buffer, time,
                damageGain > 0
                    ? string.Format("虚弱：受伤增加{0}%", (int)(damageGain*100))
                    : string.Format("抵抗：受伤降低{0}%", (int)(-damageGain*100)), damageGain <= 0, game)
        {
            _damageGain = damageGain;
        }

        private DBeAttacked _temp;
        private readonly float _damageGain;

        protected override void BuffAffect()
        {
            _temp = Buffee.HandleBeAttacked.Clone() as DBeAttacked;
            Buffee.HandleBeAttacked = (damage, attacker) => _temp((int)(damage * Math.Max(0, 1 + _damageGain)), attacker);
        }

        protected override void Cancel()
        {
            Buffee.HandleBeAttacked = _temp;
            base.Cancel();
        }
    }
}
