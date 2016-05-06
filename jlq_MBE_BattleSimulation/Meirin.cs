using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
	class Meirin : Character
	{
		public Meirin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point =>
            {
                var c = game[point];
                return c != null && Calculate.Distance(c, this) <= this.AttackRange;
            };
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                var buff = new BuffShield(this, 3*this.Interval, game);
                this.BuffList.Add(buff);
                buff.BuffTrigger();
            };
            game.HandleTarget = SCee => DoAttack(SCee, 1.3f);
            game.DefaultButtonAndLabels();
            game.UpdateLabelBackground();
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

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
