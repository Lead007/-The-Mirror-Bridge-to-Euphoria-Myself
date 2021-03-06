﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_BaseBuffs.SingleBuff
{
    /// <summary>每回合准备阶段受到一定量真实伤害</summary>
    public class BuffBeAttacked : BuffExecuteInSection
    {
        public BuffBeAttacked(Character buffee, Character buffer, int time, int damage, Character attacker, Game game)
            : base(buffee, buffer, time, Section.Preparing, string.Format("受伤：每回合准备阶段受到{0}点真实伤害", damage), false, game)
        {
            _attacker = attacker;
            _damage = damage;
        }

        private readonly Character _attacker;
        protected readonly int _damage;

        protected override void BuffAffect()
        {
            Buffee.HandleBeAttacked(_damage, _attacker);
        }

    }
}
