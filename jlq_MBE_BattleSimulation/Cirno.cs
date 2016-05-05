using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
	class Cirno : Character
	{
		public Cirno(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterPad[2] = (s, ev) =>
		    {
		        var c = game[game.MousePoint];
		        if (c == null || (!IsEnemy(c))) return;
		        game.DefaultButtonAndLabels();
		        c.LabelDisplay.Background = Brushes.LightBlue;
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            //TODO SC01
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            
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
            game.HandleIsLegalClick = point => game[point] != null && IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                DoAttack(SCee, 1.3f);
                var buff1 = new BuffAddDamage(SCee, this, 3*this.Interval, 0.1f, game);
                SCee.BuffList.Add(buff1);
                buff1.BuffTrigger();
                var buff2 = new BuffCannotMove(SCee, this, 3*this.Interval, game);
                SCee.BuffList.Add(buff2);
                buff2.BuffTrigger();
            };
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

    }
}
