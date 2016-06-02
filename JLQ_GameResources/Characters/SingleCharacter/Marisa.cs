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
                if (!SC01IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => SC01IsTargetLegal(c, game.MousePoint)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemies.Where(c => SC02IsTargetLegal(c, game.MousePoint)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被受影响的角色和其将被移动至的位置（若存在）
            enterPad[2] = (s, ev) =>
            {
                if (!SC03IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
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
        private const float SC02Gain = 2.5f;
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
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = SC01IsTargetLegal;
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
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, (float) (SC02Gain*SCGain));
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
                Enemies.Where(c => this.Distance(c) <= SC03Const1 && (c.X == this.X || c.Y == this.Y))
                    .SetLabelBackground();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private static bool SC01IsLegalClick(Point point)
        {
            return point.X > 0 && point.X < Game.Column - 1 && point.Y > 0 && point.Y < Game.Row - 1;
        }

        private bool SC01IsTargetLegal(Character SCee, Point point)
        {
            return point.IsIn33(SCee) && IsEnemy(SCee);
        }

        private bool SC02IsTargetLegal(Character SCee, Point point)
        {
            if (point.X == this.X)
            {
                if (point.Y > this.Y)
                {
                    return SCee.X == this.X && SCee.Y > this.Y;
                }
                return SCee.X == this.X && SCee.Y < this.Y;
            }
            if (point.X > this.X)
            {
                return SCee.Y == this.Y && SCee.X > this.X;
            }
            return SCee.Y == this.Y && SCee.X < this.X;
        }

        private bool SC03IsLegalClick(Point point)
        {
            return IsInRangeAndEnemy(SC03Const1, point) && (point.X == this.X || point.Y == this.Y);
        }

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
    }
}
