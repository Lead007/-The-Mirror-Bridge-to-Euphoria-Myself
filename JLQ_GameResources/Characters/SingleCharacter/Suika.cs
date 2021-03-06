using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.Attributes;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.BuffAboutCharacter;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>伊吹萃香</summary>
    public class Suika : Character
	{
		public Suika(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将受影响的角色
		    enterButton[0] = (s, ev) =>
		    {
		        Enemies.Where(c => this.Position.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(0);
            //符卡02
            //显示将受攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.IsInRange(this, SC02Range)) return;
		        game.DefaultButtonAndLabels();
                EnemyInMouseRange(SC02Range2).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将受攻击的角色
		    enterPad[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
                EnemyInMouseRange(SC03Range).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int skillAdd = 0;//TODO skill add
        private const int SC02Range = 3;
        private const int SC02Range2 = 2;
        private const int SC03Range = 2;
        public bool SC03IsBuffing = false;

        protected override void AddBuff(Buff buff)
        {
            var b = Attribute.GetCustomAttributes(buff.GetType(), typeof (BuffKindAttribute))
                .OfType<BuffKindAttribute>()
                .Any(a => a.Kind == BuffKinds.Control);
            if (SC03IsBuffing && b) return;
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
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                var buff1 = BuffGainProperty.BuffGainHitRate(SCee, this, this.Interval, -0.2f, game);
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
            game.HandleIsLegalClick = point => point.IsInRange(this, SC02Range);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC02Range2, SCee);
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
                var buff1 = BuffGainProperty.BuffGainDefence(this, this, this.Interval, -0.2f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSuikaUncontrolable(this, 2*this.Interval, game);
                buff2.BuffTrigger();
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 2.5f);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                EnemyInRange(SC03Range).SetLabelBackground();
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
