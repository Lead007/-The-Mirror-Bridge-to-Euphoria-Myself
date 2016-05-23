using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    public class Suika : Character
	{
		public Suika(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterButton[0] = (s, ev) =>
		    {
		        Enemy.Where(c => this.Position.IsIn33(c.Position)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) > 3) return;
		        Enemy.Where(c => game.MousePoint.Distance(c) <= 2).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

        //TODO 天赋

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
            //TODO SC03
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {

        }

        public override void SCShow()
        {
            AddSCButtonEvent(0);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(0);
        }
	}
}
