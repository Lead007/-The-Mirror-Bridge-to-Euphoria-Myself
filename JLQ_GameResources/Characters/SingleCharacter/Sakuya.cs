using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>十六夜咲夜</summary>
    public class Sakuya : Character, IHuman
    {
		public Sakuya(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterPad[1] = (s, ev) =>
		    {
		        if (!SC02IsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        EnemyInMouseRange(SC02Range).SetLabelBackground();
                game.MouseCharacter.SetLabelBackground(GameColor.LabelBackground2);
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

        public Human HumanKind => Human.FullHuman;

        private Character _cChange;
	    private const int SC02Range = 2;
        private const float SC02Gain = 0.2f;

        private int _moveTime = 1;
        private int _attackTime = 1;
        public override bool HasMoved
        {
            get { return _moveTime == 0; }
            set
            {
                if (value)
                {
                    _moveTime--;
                }
                else
                {
                    _attackTime = 1;
                }
            }
        }
        public override bool HasAttacked
        {
            get { return _attackTime == 0; }
            set
            {
                if (value)
                {
                    _attackTime--;
                }
                else
                {
                    _attackTime = 1;
                }
            }
        }

        //天赋
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
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                EnemyInRange(2*this.AttackRange).SetLabelBackground();
            };
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
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                game.ButtonSC.DoAction(b => b.IsEnabled = false);
                _attackTime += 2;
                _moveTime++;
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

	    private bool SC02IsLegalClick(Point point)
	    {
	        return point.IsInRange(this, 2*this.AttackRange) && game[point] != null;
	    }
    }
}
