using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>梅露兰·普莉兹姆利巴</summary>
    class Merlin : CharacterPoltergeist
	{
		public Merlin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            foreach (var buff in
                game.Characters.Where(c => IsInRangeAndFriend(skillRange, c))
                    .Select(c => new BuffSlowDownGain(c, this, this.Interval, -0.1f, game)))
            {
                buff.BuffTrigger();
            }
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
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

        }

    }
}
