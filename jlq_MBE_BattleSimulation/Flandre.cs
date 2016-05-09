using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>芙兰</summary>
    class Flandre : Character
	{
		public Flandre(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterPad[2] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => Calculate.IsIn33(game.MousePoint, c.Position))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private const float SC03Gain = 1.7f;

        //TODO 天赋

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
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick =
                point => point.X > 0 && point.X < MainWindow.Column - 1 && point.Y > 0 && point.Y < MainWindow.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) => Calculate.IsIn33(point, SCee.Position) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
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
