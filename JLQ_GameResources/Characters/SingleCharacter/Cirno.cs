using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>琪露诺</summary>
    public class Cirno : CharacterMayRepeatedlyDoDamage
    {
        public Cirno(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将攻击的敌人
            enterPad[0] = (s, ev) =>
            {
                if (!IsInRangeAndEnemy(this.AttackRange, game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //对敌人造成buff
            SC02DamageHandle = (c, damage) =>
            {
                var buff1 = BuffAddProperty.BuffAddAttack(c, this, this.Interval, -5, game);
                buff1.BuffTrigger();
            };
            //显示将攻击的敌人
            enterPad[1] = (s, ev) =>
            {
                if (!IsInRangeAndEnemy(SC02Range, game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将受影响的敌人
            enterPad[2] = (s, ev) =>
            {
                var c = game.MouseCharacter;
                if (!IsEnemy(c)) return;
                game.DefaultButtonAndLabels();
                c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        public override bool DoAttack(Character target, float times = 1)
        {
            if (target.Column == this.Column || target.Row == this.Row) return false;
            return base.DoAttack(target, times);
        }

        //符卡
        private const float SC01Gain = 0.7f;
        private int SC01Parameter => (1 + (int)this.CharacterLevel) * 15;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(this.AttackRange, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, SC01Gain);
                var buff = BuffAddProperty.BuffAddDefence(SCee, this, this.Interval, SC01Parameter, game);
                buff.BuffTrigger();
            };
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

        private const int SC02Range = 3;
        private int SC02Times => 2 + (int)this.CharacterLevel;
        private const float SC02Gain = 0.3f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC02Range, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                DamageTimes = SC02Times;
                this.EAttackDone += SC02DamageHandle;
            };
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02Gain);
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            DamageTimes = 1;
            this.EAttackDone -= SC02DamageHandle;
            RemovePadButtonEvent(1);
        }

        #region 符卡02相关属性
        private Action<Character, int> SC02DamageHandle { get; }
        #endregion

        private float SC03Gain1 => (1 + (int)this.CharacterLevel)*0.05f;
        private const float SC03Gain2 = 1.3f;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffGainBeDamaged(SCee, this, this.BuffTime, SC03Gain1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffCannotMove(SCee, this, this.Interval, game);
                buff2.BuffTrigger();
                HandleDoDanmakuAttack(SCee, SC03Gain2);
            };
            AddPadButtonEvent(2);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemies.SetLabelBackground();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }
    }
}
