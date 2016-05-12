using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>橙</summary>
    class Chen : Character
	{
		public Chen(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            enterPad[1] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) > SC02Range) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => Calculate.Distance(game.MousePoint, c) <= 1)
                    .Aggregate((Brush)Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeavePadButtonDelegate(1);
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemy.Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int SC02Range = 4;

        public override void PreparingSection()
        {
            base.PreparingSection();
            if (game[Game.CenterPoint] != null)
            {
                game.ButtonSC[2].IsEnabled = false;
            }
        }

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffAddMoveAbility(this, this, 3*this.Interval, 1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDownGain(this, this, 3*this.Interval, -0.2, game);
                buff2.BuffTrigger();
            };
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
            game.HandleIsTargetLegal = (SCee, point) => Calculate.Distance(point, SCee) <= 1 && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
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
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleSelf = () => Move(Game.CenterPoint);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 0.25f * (9 - Calculate.Distance(SCee, this)));
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        public override void SCShow()
        {
            AddSCButtonEvent(2);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(2);
        }
	}
}
