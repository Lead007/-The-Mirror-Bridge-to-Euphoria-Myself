using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.Add.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>蕾蒂</summary>
    public class Letty : Character
	{
		public Letty(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterPad[0] = (s, ev) =>
		    {
		        if (game.MousePoint.IsInRange(this, SC01Range)) return;
		        game.DefaultButtonAndLabels();
		        EnemyInMouseRange(SC01Range2).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
		}

        private const int skillRange = 2;
        private const float skillGain = 0.3f;
        private const int SC01Range = 4;
        private const int SC01Range2 = 1;
        private static PercentOfMaxHp SC02Gain { get; } = new PercentOfMaxHp(0.4f);

        public override void PreparingSection()
        {
            Enemies.Where(c => c.Distance(this) <= skillRange)
                .Select(c => new BuffSlowDownGain(c, this, this.Interval, skillGain, game))
                .DoAction(b => b.BuffTrigger());
        }

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC01Range);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(SC01Range2, SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee);
                var buff = new BuffAddMoveAbility(SCee, this, this.BuffTime, -1, game);
                buff.BuffTrigger();
            };
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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee => Cure(SC02Gain);
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
            base.EndSC03();
        }

	}
}
