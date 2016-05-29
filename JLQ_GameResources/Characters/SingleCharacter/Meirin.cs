using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.Add.Sealed;
using JLQ_BaseBuffs.Gain.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using Number;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>红美铃</summary>
    public class Meirin : Character
	{
		public Meirin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!IsInRangeAndEnemy(this.AttackRange, game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        game.MouseCharacter.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡03
            //显示所有己方角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
                game.Characters.Where(c => IsFriend(c, false)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private static RationalNumber SC03Gain = new RationalNumber(1, 10, true, false);

        //TODO 天赋
	    public override void BeAttacked(int damage, Character attacker)
	    {
	        if (random.NextBool(0.2)) return;
	        base.BeAttacked((int)(damage*0.8), attacker);
	    }

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(this.AttackRange, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                var buff = new BuffShield(this, this, this.BuffTime, game);
                buff.BuffTrigger();
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.3f);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                game.UpdateLabelBackground();
            };
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
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffGainAttack(this, this, this.BuffTime, 0.25f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffAddAttackRange(this, this, this.BuffTime, 1, game);
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
            game.HandleIsTargetLegal = (SCee, point) => IsFriend(SCee);
            game.HandleTarget = SCee =>
            {
                SCee.Cure(SC03Gain);
                var buff = new BuffGainDoDamage(SCee, this, this.BuffTime, 0.1f, game);
                buff.BuffTrigger();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
	}
}
