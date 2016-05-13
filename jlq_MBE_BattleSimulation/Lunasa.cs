using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>露娜萨·普莉兹姆利巴</summary>
    class Lunasa : Character
	{
		public Lunasa(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(this.Position, SC03Range, c))
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int SC03Range = 5;
        private const float SC03Gain = 0.7f;

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
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(this.Position, SC03Range, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
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
