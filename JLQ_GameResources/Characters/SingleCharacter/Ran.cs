using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using JLQ_GameResources.Dialogs.GamePad.ChoosePoints;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>八云蓝</summary>
    public class Ran : CharacterHitBack
	{
		public Ran(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡03
            //显示将被影响的目标和被监禁的范围
            enterPad[2] = (s, ev) =>
            {
                if (!game.HandleIsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var points = Game.PadPoints.Where(p => game.MousePoint.IsInSquare(p, 5));
                foreach (var p in points)
                {
                    var c = game[p];
                    if (c == null) game.GetButton(p).SetButtonColor();
                    else if (IsEnemy(c)) c.SetLabelBackground();
                }
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            if (game.Characters.Count > Game.Column*Game.Row - 12)
            {
                game.ButtonSC[0].IsEnabled = false;
            }
        }

        //天赋
        protected override float HitBackGain => 0.1f;

        protected override IEnumerable<Character> LegalHitBackTarget
            => game.Characters.Where(c => IsInRangeAndEnemy((this.AttackRange + 1)/2, c));

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            var dialog = new GamePad_RanSC01(game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsTargetLegal = (SCee, point) => dialog.PointsChoose.Any(p => p.Distance(SCee) == 1);
                game.HandleTarget = SCee =>
                {
                    var count = dialog.PointsChoose.Count(p => p.Distance(SCee) == 1);
                    HandleDoDanmakuAttack(SCee, 0.3f*count);
                };
            }
            else
            {
                game.HandleIsLegalClick = point => false;
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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff1 = BuffAddProperty.BuffAddMoveAbility(this, this, this.BuffTime, 1, game);
                buff1.BuffTrigger();
                var buff2 = BuffGainProperty.BuffGainDefence(this, this, this.BuffTime, 0.2f, game);
                buff2.BuffTrigger();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick =
                point => point.Column > 1 && point.Column < Game.Column - 1 && point.Row > 1 && point.Row < Game.Row - 1;
            game.HandleIsTargetLegal =
                (SCee, point) => point.IsInSquare(SCee.Position, 5);
            game.HandleTarget = SCee =>
            {
                var buff1 = BuffGainProperty.BuffGainDefence(SCee, this, this.BuffTime, -0.2f, game);
                buff1.BuffTrigger();
                var buff2 = BuffGainProperty.BuffGainDodgeRate(SCee, this, this.BuffTime, -0.2f, game);
                buff2.BuffTrigger();
                Func<PadPoint, PadPoint, bool> handle = (origin, point) =>
                {
                    var rx = Math.Abs(point.Column - origin.Column);
                    var ry = Math.Abs(point.Row - origin.Row);
                    return (rx == 3 && ry <= 3) || (ry == 3 && rx <= 3);
                };
                var buff3 = new BuffLimit(SCee, this, this.BuffTime, game.MousePoint, handle, game);
                buff3.BuffTrigger();
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
