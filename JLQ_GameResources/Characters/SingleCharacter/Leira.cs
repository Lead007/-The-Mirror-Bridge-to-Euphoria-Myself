using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>蕾拉·普莉兹姆利巴</summary>
    public class Leira : CharacterPrismriver, IHuman
    {
		public Leira(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡02
            //显示将回血的角色
		    enterPad[1] = (s, e) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsFriend(c, false)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
		}

        public Human HumanKind => Human.FullHuman;

        private static PercentOfMaxHp SC02Gain { get; } = new PercentOfMaxHp(0.2f);

        public override void PreparingSection()
        {
            base.PreparingSection();
            var ps = game.Characters.Where(c => IsFriend(c)).OfType<CharacterPrismriver>().ToList();
            var count = ps.Count;
            foreach (var c in ps)
            {
                var buff1 = BuffGainProperty.BuffGainAttack(c, this, this.Interval, 0.5f*count, game);
                buff1.BuffTrigger();
                var buff2 = BuffGainProperty.BuffGainDefence(c, this, this.Interval, 0.5f*count, game);
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
            base.EndSC03();
        }

    }
}
