using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>灵梦</summary>
    class Reimu : CharacterMovingIgnoreEnemy
    {
        public Reimu(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {
            //符卡01
            //显示将被攻击的目标
            enterButton[0] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => IsInRangeAndEnemy(this.Position, SC01Range, c))
                    .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeaveSCButtonDelegate(0);
            //符卡02
            //显示将被影响的目标和被监禁的范围
            enterPad[1] = (s, ev) =>
            {
                if (!game.HandleIsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var points = Game.PadPoints.Where(p => Calculate.IsIn33(game.MousePoint, p));
                foreach (var p in points)
                {
                    var c = game[p];
                    if (c == null) game.GetButton(p).Opacity = 1;
                    else if (Enemy.Contains(c)) c.LabelDisplay.Background = Brushes.LightBlue;
                }
            };
            SetDefaultLeavePadButtonDelegate(1);
        }

        /// <summary>符卡01的参数</summary>
        private const int SC01Range = 4;
        /// <summary>符卡02的参数</summary>
        private const int SC02Gain = 10;

        private Point SC02PointTemp = Game.DefaultPoint;
        private List<Character> SC02CharactersBeSlowed = new List<Character>();


        /// <summary>天赋：1.2倍灵力获取</summary>
        /// <param name="mp">获得的灵力量</param>
        public override void MpGain(int mp)
        {
            base.MpGain(Calculate.Floor(1.2*mp));
        }

        /// <summary>符卡01：梦想封印，对所有4格内的敌人造成1.0倍率的弹幕攻击</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(this.Position, SC01Range, SCee);
            game.HandleTarget = t => HandleDoAttack(t);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick =
                point => point.X > 0 && point.X < MainWindow.Column - 1 && point.Y > 0 && point.Y < MainWindow.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) =>
            {
                if (!Calculate.IsIn33(point, SCee.Position) || !IsEnemy(SCee)) return false;
                SC02PointTemp = point;
                return true;
            };
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffSlowDown(SCee, this, 3*this.Interval, SC02Gain, game);
                buff1.BuffTrigger();
                DIsPointWall handle = (origin, point) =>
                {
                    var rx = Math.Abs(point.X - origin.X);
                    var ry = Math.Abs(point.Y - origin.Y);
                    return (rx == 2 && ry <= 2) || (ry == 2 && rx <= 2);
                };
                var buff2 = new BuffLimit(SCee, this, 3*this.Interval, SC02PointTemp, handle, game);
                buff2.BuffTrigger();
                SC02PointTemp = Game.DefaultPoint;
            };
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
    }
}
