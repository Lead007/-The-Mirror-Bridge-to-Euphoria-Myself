using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>十六夜咲夜</summary>
	class Sakuya : Character
	{
		public Sakuya(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterPad[1] = (s, ev) =>
		    {
		        if (!SC02IsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(game.MousePoint, SC02Range, c))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		        game[game.MousePoint].LabelDisplay.Background = Brushes.LawnGreen;
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

	    private Character _cChange;
	    private const int SC02Range = 2;
        private const float SC02Gain = 0.2f;

	    public override bool DoingAttack(Character target, float times = 1)
	    {
            var buff = new BuffSlowDown(target, this, 3 * this.Interval, 2, game);
            buff.BuffTrigger();
	        return base.DoingAttack(target, times);
        }

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
            game.HandleIsLegalClick = point =>
            {
                if (!SC02IsLegalClick(point)) return false;
                _cChange = game[point];
                return true;
            };
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC02Range, SCee);
            game.HandleSelf = () =>
            {
                var p = this.Position;
                this.Move(_cChange.Position);
                _cChange.Move(p);
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02Gain);
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

	    private bool SC02IsLegalClick(Point point)
	    {
	        return Calculate.Distance(point, this) <= 2*this.AttackRange && game[point] != null;
        }
    }
}
