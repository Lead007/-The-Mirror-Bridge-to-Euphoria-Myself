using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>蓝</summary>
    class Ran : CharacterHitBack
	{
		public Ran(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        protected override IEnumerable<Character> LegalHitBackTarget
            => game.Characters.Where(c => IsInRangeAndEnemy((this.AttackRange + 1)/2, c));

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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffAddMoveAbility(this, this, 3*this.Interval, 1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffGainDefence(this, this, 3*this.Interval, 0.2f, game);
                buff2.BuffTrigger();
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

    }
}
