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
        }

        /// <summary>符卡01的参数</summary>
        private const int SC01Range = 4;
        /// <summary>符卡02的参数</summary>
        private const int SC02Range1 = 1;
        private const int SC02Range2 = 1;
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
            game.HandleTarget = t => DoAttack(t);
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
                if (!Calculate.IsIn33(point, SCee.Position) || !Enemy.Contains(SCee)) return false;
                SC02PointTemp = point;
                return true;
            };
            game.HandleTarget = SCee =>
            {
                SCee.BuffList.Add(new BuffSlowDown(SCee, this, 3*this.Interval, SC02Gain, game));
                SC02PointTemp = Game.DefaultPoint;
                //TODO another buff
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
