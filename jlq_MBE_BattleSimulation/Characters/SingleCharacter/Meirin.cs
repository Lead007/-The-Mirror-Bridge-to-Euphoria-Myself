using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Add.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;
using Number;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>红美铃</summary>
    public class Meirin : Character
	{
		public Meirin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
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
	        if (random.NextDouble() < 0.2) return;
	        base.BeAttacked((int)(damage*0.8), attacker);
	    }

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point =>
            {
                var c = game[point];
                return IsEnemy(c) && c.Distance(this) <= this.AttackRange;
            };
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                var buff = new BuffShield(this, this, this.BuffTime, game);
                buff.BuffTrigger();
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.3f);
            //显示可选择的敌人
            game.DefaultButtonAndLabels();
            game.UpdateLabelBackground();
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
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

	    public override void SCShow()
	    {
	        AddSCButtonEvent(2);
	    }

	    public override void ResetSCShow()
	    {
	        RemoveSCButtonEvent(2);
	    }
	}
}
