using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using JLQ_GameResources.Characters.SingleCharacter;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.CharacterBeCalled
{
    /// <summary>芙兰 召唤出的分身</summary>
    public class FlandreLittle : Flandre
    {
        public FlandreLittle(PadPoint position, Group group, Game game)
            : base(0, position, group, game)
        {
            
        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC.DoAction(b => b.IsEnabled = false);
        }

        public override bool DoAttack(Character target, float times = 1)
        {
            var b = base.DoAttack(target, times);
            var buff = new BuffBlooding(target, this, this.BuffTime, game);
            return b;
        }
    }
}
