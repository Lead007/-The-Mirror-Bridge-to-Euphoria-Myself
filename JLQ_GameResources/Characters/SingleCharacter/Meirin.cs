using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>红美铃</summary>
    public class Meirin : Character
	{
		public Meirin(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
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
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (!IsInRangeAndEnemy(this.AttackRange, game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示所有将被攻击的角色
            enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => this.IsInRange(c, SC03Range)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        protected override bool IsHit(Character target)
        {
            if (random.NextBool(0.2)) return true;
            return base.IsHit(target);
        }

        public override void BeAttacked(int damage, Character attacker)
	    {
	        if (random.NextBool(0.2)) return;
	        base.BeAttacked(damage, attacker);
	    }

	    //符卡
        private const float SC01Gain = 1.3f;
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
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01Gain);
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

        private const float SC02Gain = 1.5f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(this.AttackRange, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02Gain);
            AddPadButtonEvent(1);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                game.UpdateLabelBackground();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }

        private const int SC03Range = 2;
        private const float SC03Gain = 1.5f;
        private Tuple<float> SC03Parameter => new Tuple<float>((2 + (int) this.CharacterLevel)*0.03f);
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(SC03Range, point);
            game.HandleSelf = () => Cure(SC03Parameter);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
	}
}
