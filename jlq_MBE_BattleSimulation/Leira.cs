using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Number;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>蕾拉·普莉兹姆利巴</summary>
    class Leira : CharacterPrismriver
	{
		public Leira(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡02
            //显示将回血的角色
		    enterPad[1] = (s, e) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsFriend(c, false)) return;
		        game.DefaultButtonAndLabels();
		        c.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

        private static RationalNumber SC02Gain => new RationalNumber(1, 5, true, false);

        public override void PreparingSection()
        {
            base.PreparingSection();
            var ps = game.Characters.Where(c => IsFriend(c) && c is CharacterPrismriver).ToList();
            var count = ps.Count;
            foreach (var c in ps)
            {
                var buff1 = new BuffGainAttack(c, this, this.Interval, 0.5f*count, game);
                buff1.BuffTrigger();
                var buff2 = new BuffGainDefence(c, this, this.Interval, 0.5f*count, game);
                buff2.BuffTrigger();
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
            game.HandleIsLegalClick = point => IsFriend(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => SCee.Cure(SC02Gain);
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

    }
}
