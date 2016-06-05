using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.Gain.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    public class LilyWhite : Character
	{
		public LilyWhite(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    this.LabelDisplay.Content = "莉吹";
		    enterButton[0] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        if (IsWhite)
		        {
		            game.Characters.Where(c => IsInRangeAndFriend(SC01Range, c, false)).SetLabelBackground();
		        }
		        else
		        {
		            game.Characters.Where(c => IsInRangeAndEnemy(SC01Range, c)).SetLabelBackground();
		        }
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterPad[1] = (s, ev) =>
		    {
		        if (IsWhite && (!IsInRangeAndFriend(SC02Range, game.MousePoint))) return;
		        if ((!IsWhite) && (!IsEnemy(game.MouseCharacter))) return;
		        game.DefaultButtonAndLabels();
		        game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
		}

        private bool _isWhite = true;

        private bool IsWhite
        {
            get { return _isWhite; }
            set
            {
                _isWhite = value;
                if (!value)
                {
                    BlackRound = SC03BlackRound;
                    HasBlack = true;
                }
                LabelDisplay.Content = value ? "莉吹" : "莉黑";
            }
        }

        private bool HasBlack { get; set; } = false;
        private int BlackRound { get; set; } = 0;
        private const int skillNum = 100;
        private const int SC01Range = 3;
        private const int SC01Num = 50;
        private const float SC02Gain = -0.25f;
        private const float SC02BlackGain = 1.5f;
        private const float SC02BlackGain2 = 0.2f;
        private const int SC02Range = 2;
        private const int SC03BlackRound = 3;
        private const float SC03BlackGain = 0.6f;

        public override void PreparingSection()
        {
            base.PreparingSection();
            MpGain(IsWhite ? skillNum : (skillNum/2));
            if (!IsWhite)
            {
                BlackRound--;
                if (BlackRound == 0) IsWhite = true;
            }
            if (HasBlack && IsWhite)
            {
                game.ButtonSC[2].IsEnabled = false;
            }
        }

        public override string ToString()
            => (IsWhite ? "霍瓦特状态\n" : "布莱克状态\n") + base.ToString();

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            if (IsWhite)
            {
                game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndFriend(SC01Range, SCee, false);
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
                game.HandleIsLegalClick = point => IsInRangeAndFriend(SC02Range, point);
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee => SCee.MpGain(2*SCee.Attack);
                game.HandleResetShow = () =>
                {
                    game.DefaultButtonAndLabels();
                    game.Characters.Where(c => IsInRangeAndFriend(SC02Range, c)).SetLabelBackground();
                };
            }
            else
            {
                game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC02Range, game[point]);
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee =>
                {
                    HandleDoDanmakuAttack(SCee, SC02BlackGain);
                    var buff = new BuffGainDefence(SCee, this, 2*this.Interval, SC02BlackGain2, game);
                    buff.BuffTrigger();
                };
                game.HandleResetShow = () =>
                {
                    game.DefaultButtonAndLabels();
                    EnemyInRange(SC02Range).SetLabelBackground();
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
            if (IsWhite)
            {
                game.HandleIsTargetLegal = (SCee, point) => false;
                game.HandleSelf = () =>
                {
                    IsWhite = false;
                    var buff1 = new BuffGainAttack(this, this, BuffTime, 0.3, game);
                    buff1.BuffTrigger();
                    var buff2 = new BuffGainDefence(this, this, BuffTime, 0.3, game);
                    buff2.BuffTrigger();
                };
            }
            else
            {
                game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
                game.HandleTarget = SCee =>
                {
                    var buff = new BuffBeDanmakuAttacked(SCee, this, BuffTime, SC03BlackGain, this, game);
                };
            }
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
	}
}
