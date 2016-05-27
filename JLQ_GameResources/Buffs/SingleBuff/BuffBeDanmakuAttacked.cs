using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_GameResources.Buffs.SingleBuff
{
    /// <summary>每回合准备阶段被攻击的Buff</summary>
    public class BuffBeDanmakuAttacked : BuffExecuteInSection
    {
        public BuffBeDanmakuAttacked(Character buffee, Character buffer, int time, float gain, Character attacker, Game game)
            : base(buffee, buffer, time, Section.Preparing, string.Format("丰收祭：每回合准备阶段受到{0}倍率弹幕伤害", gain), false, game)
        {
            _attacker = attacker;
            _gain = gain;
        }

        private readonly Character _attacker;
        private readonly float _gain;

        protected override void BuffAffect()
        {
            _attacker.HandleDoDanmakuAttack(Buffee, _gain);
        }
    }
}
