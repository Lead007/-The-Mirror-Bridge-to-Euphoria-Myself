using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
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

	    public override bool DoAttack(Character target, float times = 1)
	    {
            //判断是否命中
            if (HandleIsHit(target)) return false;
            //判断是否近战
            var closeGain = HandleCloseGain(target);
            //计算基础伤害
            var damage = /*基础伤害*/ Calculate.Damage(this.Attack, target.Defence) * /*近战补正*/closeGain * FloatDamage * times;
            //判断是否暴击
            var isCriticalHit = HandleIsCriticalHit(target);
            if (isCriticalHit)
            {
                damage *= this.CriticalHitGain;
            }
            target.HandleBeAttacked((int)damage, this);
	        var buff = new BuffSlowDown(target, this, 3*this.Interval, 2, game);
	        buff.BuffTrigger();
            return isCriticalHit;
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
            game.HandleTarget = SCee => HandleDoAttack(SCee, 0.2f);
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
