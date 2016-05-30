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

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>帕秋莉</summary>
    public class Patchouli : Character
	{
		public Patchouli(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡02
            //显示将被影响的敌人
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) > SC02Range) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被影响的敌人
            enterPad[2] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) > SC03Range) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        private int _attackTimes = 0;
	    private const int SC02Range = 5;
        private const int SC03Range = 5;

        public override bool DoingAttack(Character target, float times = 1)
        {
            if (_attackTimes != 2) _attackTimes++;
            return base.DoingAttack(target, times);
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleSelf = () => MpGain(this.Mp/10);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.Distance(this) <= SC02Range;
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 1.3f);
                var buff1 = new BuffAddAttackRange(SCee, this, this.BuffTime, -1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDown(SCee, this, this.BuffTime, 5, game);
                buff2.BuffTrigger();
            };
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
            game.HandleIsLegalClick = point => point.Distance(this) <= SC03Range;
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 1.7f);
                var buff1 = new BuffGainDefence(SCee, this, this.BuffTime, -0.2f, game);
                buff1.BuffTrigger();
                
            };
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

    }
}
