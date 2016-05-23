using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;
using JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChoosePoints;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>梅露兰·普莉兹姆利巴</summary>
    public class Merlin : CharacterPoltergeist
	{
		public Merlin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterPad[0] = (s, ev) =>
		    {
		        if (!SC01IsLegalClick(game.MousePoint)) return;
		        var cs = Enemy.Where(c => SC01IsTargetLegal(c, game.MousePoint)).ToList();
		        if (!cs.Any()) return;
                game.DefaultButtonAndLabels();
                var ce = cs[0];
		        if (ce != null) ce.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(0);
		}

        public override void PreparingSection()
        {
            base.PreparingSection();
            foreach (var buff in
                game.Characters.Where(c => IsInRangeAndFriend(skillRange, c))
                    .Select(c => new BuffSlowDownGain(c, this, this.Interval, -0.1f, game)))
            {
                buff.BuffTrigger();
            }
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = SC01IsTargetLegal;
            game.HandleTarget = SCee =>
            {
                var d = Math.Max(Math.Abs(SCee.X - this.X), Math.Abs(SCee.Y - this.Y));
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
                game.HandleIsTargetLegal = (SCee, point) => dialog.PointsChoose.Any(p => Calculate.Distance(p, SCee) <= 2);
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff = new BuffGainAttack(SCee, this, 2 * this.Interval, 0.1, game);
                        buff.BuffTrigger();
                    }
                    else if (IsEnemy(SCee))
                    {
                        var buff = new BuffGainHitRate(SCee, this, 2 * this.Interval, -0.1, game);
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

        private bool IsInLine(Character SCee, Point point)
        {
            if (point.X == this.X)
            {
                return SCee.X == this.X && ((point.Y > this.Y) == (SCee.Y > this.Y));
            }
            if (point.Y == this.Y)
            {
                return SCee.Y == this.Y && ((point.X > this.X) == (SCee.X > this.X));
            }
            return Math.Abs(SCee.X - this.X) == Math.Abs(SCee.Y - this.Y) &&
                   ((point.Y > this.Y) == (SCee.Y > this.Y)) && (point.X > this.X) == (SCee.X > this.X);
        }

        private bool SC01IsLegalClick(Point point)
        {
            return Math.Abs(point.X - this.X) <= 1 && Math.Abs(point.Y - this.Y) <= 1 && point != this.Position;
        }

        private bool SC01IsTargetLegal(Character SCee, Point point)
        {
            var cInLine = Enemy.Where(c => IsInLine(c, point)).ToList();
            if (!cInLine.Any()) return false;
            var min = cInLine.Min(c => Calculate.Distance(c, this));
            return IsInLine(SCee, point) && Calculate.Distance(SCee, this) == min;
        }
    }
}
