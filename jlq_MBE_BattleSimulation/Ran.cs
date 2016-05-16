using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>八云蓝</summary>
    class Ran : CharacterHitBack
	{
		public Ran(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡03
            //显示将被影响的目标和被监禁的范围
            enterPad[2] = (s, ev) =>
            {
                if (!game.HandleIsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var points = Game.PadPoints.Where(p => Calculate.IsInSquare(game.MousePoint, p, 5));
                foreach (var p in points)
                {
                    var c = game[p];
                    if (c == null) game.GetButton(p).Opacity = 1;
                    else if (Enemy.Contains(c)) c.LabelDisplay.Background = GameColor.LabelBackground;
                }
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        protected override IEnumerable<Character> LegalHitBackTarget
            => game.Characters.Where(c => IsInRangeAndEnemy((this.AttackRange + 1)/2, c));

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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffAddMoveAbility(this, this, this.BuffTime, 1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffGainDefence(this, this, this.BuffTime, 0.2f, game);
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
                point => point.X > 1 && point.X < Game.Column - 1 && point.Y > 1 && point.Y < Game.Row - 1;
            game.HandleIsTargetLegal =
                (SCee, point) => Calculate.IsInSquare(point, SCee.Position, 5);
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffGainDefence(SCee, this, this.BuffTime, -0.2f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffGainDodgeRate(SCee, this, this.BuffTime, -0.2f, game);
                buff2.BuffTrigger();
                DIsPointWall handle = (origin, point) =>
                {
                    var rx = Math.Abs(point.X - origin.X);
                    var ry = Math.Abs(point.Y - origin.Y);
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
