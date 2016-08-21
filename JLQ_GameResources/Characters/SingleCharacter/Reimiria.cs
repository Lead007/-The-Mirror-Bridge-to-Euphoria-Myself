using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>蕾米</summary>
    public class Reimiria : CharacterMayRepeatedlyDoDamage
	{
		public Reimiria(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡02
            //显示将被攻击的角色
            enterButton[1] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                Enemies.SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(1);
            //符卡03
            //显示将被攻击的角色
            enterPad[2] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemies.Where(c => SC03IsTargetLegal(c, game.MousePoint)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        //天赋
        public override bool DoingAttack(Character target, float times = 1)
        {
            if (!(((float)target.Hp)/target.MaxHp <= 0.1f)) return base.DoingAttack(target, times);
            target.BeAttacked(9999, this);
            HandleEAttackDone(target, 9999);
            return false;
        }

        //符卡
        private int SC01Parameter1 => this.CharacterLevel > Level.Normal ? 2 : 1;
        private int SC01Parameter2 => 2 + (int) this.CharacterLevel;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                var buff = new BuffAddDamageTimes(this, this.Interval*SC01Parameter2, SC01Parameter1, game);
                buff.BuffTrigger();
            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        private const float SC02Gain = 0.7f;
        private Tuple<float> SC02Pamameter { get; } = new Tuple<float>(0.5f);
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                //判断是否命中
                if (HandleIsHit(SCee)) return;
                //造成伤害
                DoingAttack(SCee, SC02Gain);
                this.Cure(SC02Pamameter);
                if (SCee.IsDead) this.DamageTimes++;
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        private const float SC03Gain = 2.0f;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = (SCee, point) => SC03IsTargetLegal(SCee, point) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private bool SC03IsTargetLegal(Character SCee, PadPoint point)
        {
            if (point.Column == this.Column)
            {
                if (point.Row > this.Row)
                {
                    return SCee.Column == this.Column && SCee.Row > this.Row;
                }
                return SCee.Column == this.Column && SCee.Row < this.Row;
            }
            if (point.Column > this.Column)
            {
                return SCee.Row == this.Row && SCee.Column > this.Column;
            }
            return SCee.Row == this.Row && SCee.Column < this.Column;
        }
	}
}
