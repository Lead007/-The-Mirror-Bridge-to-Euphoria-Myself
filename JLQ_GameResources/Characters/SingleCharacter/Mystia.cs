using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    public class Mystia : Character
	{
		public Mystia(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将被攻击的角色和将受影响的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!game.MousePoint.IsInRange(this, SC01Range)) return;
		        game.DefaultButtonAndLabels();
		        var c1 = game.MouseCharacter;
		        if (IsEnemy(c1))
		        {
		            c1.SetLabelBackground(GameColor.LabelBackground2);
		        }
		        Enemies.Where(c => game.MousePoint.Distance(c) == 1).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将受影响的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (!game.MousePoint.IsInRange(this, SC02Range)) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将受影响的角色
		    enterPad[2] = (s, ev) =>
		    {
                if (!game.MousePoint.IsInRange(this, SC03Range)) return;
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsFriend(c) && game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private const int SC01Range = 3;
        private const int SC02Range = 4;
        private const int SC03Range = 4;

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC01Range);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, 1, SCee);
            game.HandleTarget = SCee =>
            {
                if (SCee.Position == game.MousePoint)
                {
                    HandleDoDanmakuAttack(SCee, 2);
                }
                var buff = BuffAddProperty.BuffAddMoveAbility(SCee, this, SCee.Interval, -1, game);
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
            game.HandleIsLegalClick = point => point.IsInRange(this, SC02Range);
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                var buff1 = BuffAddProperty.BuffAddHitRate(SCee, this, BuffTime, -10, game);
                buff1.BuffTrigger();
                var buff2 = BuffAddProperty.BuffAddDodgeRate(SCee, this, BuffTime, -10, game);
                buff2.BuffTrigger();
                var buffs = SCee.BuffList.Where(b => b.IsPositive == true).ToList();
                if (!buffs.Any()) return;
                var bs = random.RandomElement(buffs);
                bs.BuffEnd();
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
            game.HandleIsLegalClick = point => point.IsInRange(this, SC03Range);
            game.HandleIsTargetLegal = (SCee, point) => IsFriend(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                SCee.Cure(0.2*SCee.Attack);
                var buff = new BuffSlowDownGain(SCee, this, 2*this.Interval, -0.4, game);
                buff.BuffTrigger();
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
