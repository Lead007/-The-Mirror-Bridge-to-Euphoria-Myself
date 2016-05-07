using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
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
            leavePad[0] = (s, ev) => SetDefaultLeavePadButtonDelegate(0);
        }

        private float SCGain = 1.0f;

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
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

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

        private static bool SC01IsLegalClick(Point point)
        {
            return point.X > 0 && point.X < MainWindow.Column - 1 && point.Y > 0 && point.Y < MainWindow.Row - 1;
        }

        private bool SC01IsTargetLegal(Character SCee, Point point)
        {
            return Calculate.IsIn33(point, SCee.Position) && IsEnemy(SCee);
        }
    }
}
