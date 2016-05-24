using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>莉格露·奈特巴格</summary>
    public class Wriggle : Character
	{
		public Wriggle(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {

        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                var buff = new BuffGainBeDamaged(this, this, this.Interval, -0.5f, game);
                buff.BuffTrigger();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            //TODO SC03
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {

        }

    }
}
