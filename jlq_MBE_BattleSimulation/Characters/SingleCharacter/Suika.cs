using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;
using JLQ_MBE_BattleSimulation.Buffs;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>伊吹萃香</summary>
    public class Suika : Character
	{
		public Suika(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将受影响的角色
		    enterButton[0] = (s, ev) =>
		    {
		        Enemy.Where(c => this.Position.IsIn33(c.Position)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(0);
            //符卡02
            //显示将受攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) > 3) return;
		        Enemy.Where(c => game.MousePoint.Distance(c) <= 2).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将受攻击的角色
		    enterButton[2] = (s, ev) =>
		    {
		        Enemy.Where(c => IsInRangeAndEnemy(SC03Range, c)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int skillAdd = 0;//TODO skill add
        private const int SC03Range = 2;
        public bool SC03IsBuffing = false;

        public override void AddBuff(Buff buff)
        {
            if (buff is IControl && SC03IsBuffing) return;
            base.AddBuff(buff);
        }

        //天赋
        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            this.AttackAdd = skillAdd;
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee.Position);
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffGainHitRate(SCee, this, this.Interval, -0.2f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDownGain(SCee, this, this.Interval, 0.2, game);
                buff2.BuffTrigger();
            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.Distance(this) <= 3;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.Distance(SCee) <= 2;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.5f);
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
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(SC03Range, SCee);
            game.HandleSelf = () =>
            {
                var buff1 = new BuffGainDefence(this, this, this.Interval, -0.2f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSuikaUncontrolable(this, 2*this.Interval, game);
                buff2.BuffTrigger();
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 2.5f);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
	}
}
