using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    class LilyWhite : Character
	{
		public LilyWhite(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterButton[0] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        if (IsWhite)
		        {
		            game.Characters.Where(c => Calculate.Distance(c, this) <= SC01Range && c.Group == this.Group)
		                .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		        }
		        else
		        {
		            game.Characters.Where(c => IsInRangeAndEnemy(SC01Range, c))
		                .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		        }
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterPad[1] = (s, ev) =>
		    {
		        if (IsWhite && (!SC02WhiteIsLegalClick(game.MousePoint))) return;
		        if ((!IsWhite) && (!IsEnemy(game.MouseCharacter))) return;
		        game.DefaultButtonAndLabels();
		        game.MouseCharacter.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

        public bool IsWhite { get; set; } = true;
        private const int skillNum = 100;
        private const int SC01Range = 3;
        private const int SC01Num = 50;
        private const float SC02Gain = -0.25f;
        private const float SC02BlackGain = 1.5f;
        private const float SC02BlackGain2 = 0.2f;
        private const int SC02Range = 2;

        public override void PreparingSection()
        {
            base.PreparingSection();
            MpGain(IsWhite ? skillNum : (skillNum/2));
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            if (IsWhite)
            {
                game.HandleIsTargetLegal = (SCee, point) =>
                    Calculate.Distance(SCee, this) <= SC01Range && SCee.Group == this.Group && SCee != this;
                game.HandleTarget = SCee => SCee.MpGain(SC01Num);
            }
            else
            {
                game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(SC01Range, SCee);
                game.HandleTarget = SCee =>
                {
                    HandleDoDanmakuAttack(SCee);
                    var buff = new BuffGainDoDamage(SCee, this, this.Interval, SC02Gain, game);
                    buff.BuffTrigger();
                };
            }
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            if (IsWhite)
            {
                game.HandleIsLegalClick = SC02WhiteIsLegalClick;
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee => SCee.MpGain(2*SCee.Attack);
            }
            else
            {
                game.HandleIsLegalClick = point => IsEnemy(game[point]);
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee =>
                {
                    HandleDoDanmakuAttack(SCee, SC02BlackGain);
                    var buff = new BuffGainDefence(SCee, this, 2*this.Interval, SC02BlackGain2, game);
                    buff.BuffTrigger();
                };
            }
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
            //TODO SC03
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {

        }

        public override void SCShow()
        {
            AddSCButtonEvent(0);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(0);
        }

        private bool SC02WhiteIsLegalClick(Point point)
        {
            if (Calculate.Distance(point, this) > SC02Range) return false;
            var c = game[point];
            return c != null && c.Group == this.Group;
        }
    }
}
