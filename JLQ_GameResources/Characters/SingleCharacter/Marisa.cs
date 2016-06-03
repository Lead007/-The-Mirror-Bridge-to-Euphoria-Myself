using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>魔理沙</summary>
    public class Marisa : Character, IHuman
    {
        public Marisa(int id, Point position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                EnemyInMouseRange(1).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) != 1) return;
                game.DefaultButtonAndLabels();
                if (game.MouseColumn == this.X)
                {
                    Enemies.Where(c => c.X == this.X && ((game.MouseRow > this.Y) == (c.Y > this.Y)))
                        .SetLabelBackground();
                    Enemies.Where(c =>
                        Math.Abs(c.X - this.X) == 1 && ((game.MouseRow > this.Y) == (c.Y > this.Y)) && c.Y != this.Y)
                        .SetLabelBackground(GameColor.LabelBackground2);
                }
                else
                {
                    Enemies.Where(c => c.Y == this.Y && ((game.MouseColumn > this.X) == (c.X > this.X)))
                        .SetLabelBackground();
                    Enemies.Where(c =>
                        Math.Abs(c.Y - this.Y) == 1 && ((game.MouseColumn > this.X) == (c.X > this.X)) && c.X != this.X)
                        .SetLabelBackground(GameColor.LabelBackground2);
                }
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将瞬移到的点、将被受影响的角色和其将被移动至的位置（若存在）
            enterPad[2] = (s, ev) =>
            {
                if (!SC03IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var p = this.Position.FacePoint(game.MousePoint);
                if (this.Position != p) game.GetButton(p).SetButtonColor();
                var c = game.MouseCharacter;
                c.SetLabelBackground();
                var point = Destination(c);
                if (game[point] == null)
                {
                    game.GetButton(point).SetButtonColor();
                }
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        public Human HumanKind => Human.FullHuman;

        private float SCGain = 1.0f;
        private const float SC02Gain1 = 2;
        private const float SC02Gain2 = 1.5f;
        private const int SC03Const1 = 2;
        private const int SC03Const2 = 2;

        public override int Mp
        {
            get { return base.Mp; }
            set
            {
                base.Mp = value;
                SCGain = 1 + Mp/4000;
            }
        }

        //符卡
        /// <summary>符卡01：星屑幻想，选定一块3*3的区域，对其内敌人造成1.0倍率的弹幕攻击。</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => true;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, 1, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SCGain);
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
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = (SCee, point) => SC02IsTargetLegal(SCee, point) && IsEnemy(SCee);
            game.HandleTarget =
                SCee => HandleDoDanmakuAttack(SCee,
                    (float)
                        ((Math.Min(Math.Abs(SCee.X - this.X), Math.Abs(SCee.Y - this.Y)) == 0
                            ? SC02Gain1
                            : SC02Gain2)*SCGain));
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
            game.HandleIsLegalClick = SC03IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () => Move(this.Position.FacePoint(game.MousePoint));
            game.HandleTarget = SCee =>
            {
                //判断是否命中
                if (HandleIsHit(SCee)) return;
                //判断是否近战
                var closeGain = HandleCloseGain(SCee);
                //计算基础伤害
                var damage = /*基础伤害*/ Calculate.Damage(this.Attack, SCee.Defence)* /*近战补正*/closeGain*FloatDamage*2.5;
                //判断是否暴击
                var isCriticalHit = HandleIsCriticalHit(SCee);
                if (isCriticalHit)
                {
                    damage *= this.CriticalHitGain;
                }
                var point = Destination(SCee);
                if (game[point] == null) SCee.Move(point);
                SCee.HandleBeAttacked((int) damage, this);
            };
            AddPadButtonEvent(2);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemies.Where(c => SC03IsLegalClick(c.Position)).SetLabelBackground();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private bool SC02IsTargetLegal(Character SCee, Point point)
        {
            if (point.X == this.X)
            {
                return Math.Abs(SCee.X - this.X) <= 1 && ((point.Y > this.Y) == (SCee.Y > this.Y)) && SCee.Y != this.Y;
            }
            return Math.Abs(SCee.Y - this.Y) <= 1 && ((point.X > this.X) == (SCee.X > this.X)) && SCee.X != this.X;
        }

        private bool SC03IsLegalClick(Point point) =>
            IsInRangeAndEnemy(SC03Const1, point) && (point.X == this.X || point.Y == this.Y) &&
            IsCurrentOrNull(game[this.Position.FacePoint(point)]);

        private Point Destination(Character SCee)
        {
            return SCee.X == this.X
                ? new Point(SCee.X,
                    SCee.Y > this.Y
                        ? Math.Min(SCee.Y + SC03Const2, Game.Row - 1)
                        : Math.Max(SCee.Y - SC03Const2, 0))
                : new Point(
                    SCee.X > this.X
                        ? Math.Min(SCee.X + 2, Game.Column - 1)
                        : Math.Max(SCee.X - SC03Const2, 0),
                    SCee.Y);
        }

        private bool IsCurrentOrNull(Character c)
        {
            return c == null || c == this;
        }
    }
}
