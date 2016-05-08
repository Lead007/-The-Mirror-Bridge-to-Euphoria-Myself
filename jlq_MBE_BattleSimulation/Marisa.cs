using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>魔理沙</summary>
    class Marisa : Character
    {
        public Marisa(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {
            //符卡01
            //显示将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                if (!SC01IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => SC01IsTargetLegal(c, game.MousePoint))
                    .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => SC02IsTargetLegal(c, game.MousePoint))
                    .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被攻击的角色和将移动至的位置（若存在）
            enterPad[2] = (s, ev) =>
            {
                if (!SC03IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var c = game[game.MousePoint];
                c.LabelDisplay.Background = Brushes.LightBlue;
                var point = Destination(c);
                if (game[point] == null)
                {
                    game.Buttons[(int)point.X, (int)point.Y].Opacity = 1;
                }
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

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
            game.HandleTarget = SCee => HandleDoAttack(SCee, SCGain);
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
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) == 1;
            game.HandleIsTargetLegal = (SCee, point) => SC02IsTargetLegal(SCee, point) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoAttack(SCee, (float) (SC02Gain*SCGain));
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
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private static bool SC01IsLegalClick(Point point)
        {
            return point.X > 0 && point.X < MainWindow.Column - 1 && point.Y > 0 && point.Y < MainWindow.Row - 1;
        }

        private bool SC01IsTargetLegal(Character SCee, Point point)
        {
            return Calculate.IsIn33(point, SCee.Position) && IsEnemy(SCee);
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
            return Calculate.Distance(point, this) <= SC03Const1 && (point.X == this.X || point.Y == this.Y) &&
                   IsEnemy(game[point]);

        }

        private Point Destination(Character SCee)
        {
            return SCee.X == this.X
                ? new Point(SCee.X,
                    SCee.Y > this.Y
                        ? Math.Min(SCee.Y + SC03Const2, MainWindow.Row - 1)
                        : Math.Max(SCee.Y - SC03Const2, 0))
                : new Point(
                    SCee.X > this.X
                        ? Math.Min(SCee.X + 2, MainWindow.Column - 1)
                        : Math.Max(SCee.X - SC03Const2, 0),
                    SCee.Y);
        }
    }
}
