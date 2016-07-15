using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Dialogs.GamePad.ChoosePoints;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>梅露兰·普莉兹姆利巴</summary>
    public class Merlin : CharacterPoltergeist
	{
		public Merlin(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!SC01IsLegalClick(game.MousePoint)) return;
		        var cs = Enemies.Where(c => SC01IsTargetLegal(c, game.MousePoint)).ToList();
		        if (!cs.Any()) return;
                game.DefaultButtonAndLabels();
		        cs[0].SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
		}

        private const int SC02Range = 2;

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.Characters.Where(c => IsInRangeAndFriend(skillRange, c))
                .Select(c => new BuffSlowDownGain(c, this, this.Interval, -0.1f, game)).DoAction(b => b.BuffTrigger());
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = SC01IsTargetLegal;
            game.HandleTarget = SCee =>
            {
                var d = Math.Max(Math.Abs(SCee.Column - this.Column), Math.Abs(SCee.Row - this.Row));
                HandleDoDanmakuAttack(SCee, 1.6f - 0.1f*d);
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
            var dialog = new GamePad_MerlinSC02(game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsTargetLegal = (SCee, point) => dialog.PointsChoose.Any(p => p.IsInRange(SCee, SC02Range));
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff = BuffGainProperty.BuffGainAttack(SCee, this, 2 * this.Interval, 0.1f, game);
                        buff.BuffTrigger();
                    }
                    else if (IsEnemy(SCee))
                    {
                        var buff = BuffGainProperty.BuffGainHitRate(SCee, this, 2 * this.Interval, -0.1f, game);
                        buff.BuffTrigger();
                    }
                };
            }
            else
            {
                game.HandleIsLegalClick = point => false;
            }
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        private bool IsInLine(Character SCee, PadPoint point)
        {
            if (point.Column == this.Column)
            {
                return SCee.Column == this.Column && ((point.Row > this.Row) == (SCee.Row > this.Row));
            }
            if (point.Row == this.Row)
            {
                return SCee.Row == this.Row && ((point.Column > this.Column) == (SCee.Column > this.Column));
            }
            return Math.Abs(SCee.Column - this.Column) == Math.Abs(SCee.Row - this.Row) &&
                   ((point.Row > this.Row) == (SCee.Row > this.Row)) && (point.Column > this.Column) == (SCee.Column > this.Column);
        }

        private bool SC01IsLegalClick(PadPoint point)
        {
            return Math.Abs(point.Column - this.Column) <= 1 && Math.Abs(point.Row - this.Row) <= 1 && point != this.Position;
        }

        private bool SC01IsTargetLegal(Character SCee, PadPoint point)
        {
            var cInLine = Enemies.Where(c => IsInLine(c, point)).ToList();
            if (!cInLine.Any()) return false;
            var ct = cInLine.OrderBy(c => c.Distance(this)).First();
            return SCee == ct;
        }
    }
}
