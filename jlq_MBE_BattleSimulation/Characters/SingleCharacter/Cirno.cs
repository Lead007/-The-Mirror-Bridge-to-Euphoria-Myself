using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Add.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>琪露诺</summary>
    public class Cirno : CharacterMayRepeatedlyDoDamage
	{
		public Cirno(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将攻击的敌人
		    enterPad[0] = (s, ev) =>
		    {
		        if (SC01IsLegalClick(game.MousePoint)) game.MouseCharacter.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡03
            //显示将攻击的敌人
            enterPad[2] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        game.DefaultButtonAndLabels();
		        c.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 0.7f);
                var buff = new BuffAddDefence(SCee, this, this.Interval, -20, game);
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
            game.HandleSelf = () =>
            {
                var buff = new BuffAddDamageTimes(this, this.BuffTime, 2, game);
                buff.BuffTrigger();
                var buff2 = new BuffDecreaseMoveAbilityWhenHit(this, this, this.BuffTime, -1, game);
                buff2.BuffTrigger();
            };
            game.HandleTarget = SCee => { };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => game[point] != null && IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 1.3f);
                var buff1 = new BuffGainBeDamaged(SCee, this, this.BuffTime, 0.1f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffCannotMove(SCee, this, this.BuffTime, game);
                buff2.BuffTrigger();
            };
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

	    private bool SC01IsLegalClick(Point point)
	    {
            var c = game[point];
            return IsEnemy(c) && Calculate.Distance(c, this) <= this.AttackRange;
        }

    }
}