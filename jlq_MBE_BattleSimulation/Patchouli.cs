using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
	class Patchouli : Character
	{
		public Patchouli(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡02
            //显示将被影响的敌人
		    enterPad[1] = (s, ev) =>
		    {
		        if (Calculate.Distance(game.MousePoint, this) > SC02Range) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => Calculate.IsIn33(game.MousePoint, c.Position))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被影响的敌人
            enterPad[2] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) > SC03Range) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => Calculate.IsIn33(game.MousePoint, c.Position))
                    .Aggregate((Brush)Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

	    private const int SC02Range = 5;
        private const int SC03Range = 5;

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleSelf = () => MpGain((int) (this.Mp/10));
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) <= SC02Range;
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && Calculate.IsIn33(point, SCee.Position);
            game.HandleTarget = SCee =>
            {
                HandleDoAttack(SCee, 1.3f);
                var buff1 = new BuffAddAttackRange(SCee, this, 3*this.Interval, -1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDown(SCee, this, 3*this.Interval, 5, game);
                buff2.BuffTrigger();
            };
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) <= SC03Range;
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && Calculate.IsIn33(point, SCee.Position);
            game.HandleTarget = SCee =>
            {
                HandleDoAttack(SCee, 1.7f);
                var buff1 = new BuffGainDefence(SCee, this, 3*this.Interval, -0.2f, game);
                buff1.BuffTrigger();
                
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
