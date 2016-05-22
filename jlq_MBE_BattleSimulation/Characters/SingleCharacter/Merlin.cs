using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;
using JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChoosePoints;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>梅露兰·普莉兹姆利巴</summary>
    public class Merlin : CharacterPoltergeist
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
            var dialog = new GamePad_MerlinSC02(game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsTargetLegal = (SCee, point) => dialog.PointsChoose.Any(p => Calculate.Distance(p, SCee) <= 2);
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff = new BuffGainAttack(SCee, this, 2 * this.Interval, 0.1, game);
                        buff.BuffTrigger();
                    }
                    else if (IsEnemy(SCee))
                    {
                        var buff = new BuffGainHitRate(SCee, this, 2 * this.Interval, -0.1, game);
                        buff.BuffTrigger();
                    }
                };
            }
            else
            {
                game.HandleIsLegalClick = point => false;
            }
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

    }
}
