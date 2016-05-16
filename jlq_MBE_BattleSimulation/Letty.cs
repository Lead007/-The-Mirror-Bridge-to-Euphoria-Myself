using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>蕾蒂</summary>
    class Letty : Character
	{
		public Letty(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterPad[0] = (s, ev) =>
		    {
		        if (Calculate.Distance(game.MousePoint, this) > SC01Range) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => Calculate.Distance(game.MousePoint, c) <= 1)
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeavePadButtonDelegate(0);
		}

        private const int skillRange = 2;
        private const float skillGain = 0.3f;
        private const int SC01Range = 4;

        public override void PreparingSection()
        {
            foreach (var buff in
                Enemy.Where(c => Calculate.Distance(c, this) <= skillRange)
                .Select(c => new BuffSlowDownGain(c, this, this.Interval, skillGain, game)))
            {
                buff.BuffTrigger();
            }
        }

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) <= SC01Range;
            game.HandleIsTargetLegal = (SCee, point) => Calculate.Distance(point, SCee) <= 1 && IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee);
                var buff = new BuffAddMoveAbility(SCee, this, this.BuffTime, -1, game);
                buff.BuffTrigger();
            };
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee => Cure(0.4*this.Data.MaxHp);
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
