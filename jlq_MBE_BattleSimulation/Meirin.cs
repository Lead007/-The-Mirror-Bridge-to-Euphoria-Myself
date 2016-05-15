using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>红美铃</summary>
    class Meirin : Character
	{
		public Meirin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡03
            //显示所有己方角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        foreach (var c in game.Characters.Where(c => IsFriend(c, false)))
		        {
		            c.LabelDisplay.Background = GameColor.LabelBackground;
		        }
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

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
                return IsEnemy(c) && Calculate.Distance(c, this) <= this.AttackRange;
            };
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                var buff = new BuffShield(this, this, 3*this.Interval, game);
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
                var buff1 = new BuffGainAttack(this, this, 3*this.Interval, 0.25f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffAddAttackRange(this, this, 3*this.Interval, 1, game);
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
                SCee.Cure(SCee.Data.MaxHp*0.1);
                var buff = new BuffGainDoDamage(SCee, this, 3*this.Interval, 0.1f, game);
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
