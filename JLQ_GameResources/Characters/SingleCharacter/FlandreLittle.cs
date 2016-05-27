using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    public class FlandreLittle : Flandre
    {
        public FlandreLittle(int id, Point position, Group group, Random random, Game game)
            : base(0, position, group, random, game)
        {
            
        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC.Aggregate(false, (c, b) => b.IsEnabled = false);
        }

        public override void EndSection()
        {
            base.EndSection();
            game.ButtonSC.Aggregate(false, (c, b) => b.IsEnabled = true);
        }

        public override bool DoAttack(Character target, float times = 1)
        {
            var b = base.DoAttack(target, times);
            var buff = new BuffBlooding(target, this, this.BuffTime, game);
            return b;
        }
    }
}
