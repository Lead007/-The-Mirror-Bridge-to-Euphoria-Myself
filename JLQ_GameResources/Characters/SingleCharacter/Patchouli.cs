using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>帕秋莉</summary>
    public class Patchouli : Character
	{
		public Patchouli(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡02
            //显示将被影响的敌人
		    enterPad[1] = (s, ev) =>
		    {
		        if (!game.MousePoint.IsInRange(this, SC02Range)) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被影响的敌人
            enterPad[2] = (s, ev) =>
            {
                if (!game.MousePoint.IsInRange(this, SC03Range)) return;
                game.DefaultButtonAndLabels();
                Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        //天赋
        private int _attackTimes = 0;
        private int AttackTimes
        {
            get { return _attackTimes; }
            set { _attackTimes = Math.Min(SkillParameter, value); }
        }
        private const int SkillParameter = 3;
        public override bool HasAttacked
        {
            get { return base.HasAttacked; }
            set
            {
                base.HasAttacked = value;
                if (value)
                {
                    AttackTimes++;
                }
            }
        }

        public override bool MpUse(int mp)
        {
            if (AttackTimes != SkillParameter) return base.MpUse(mp);
            AttackTimes = 0;
            return true;
        }

        public override string ToString() =>
            base.ToString() +
            string.Format("\n已攻击/符卡次数：{0}。{1}", AttackTimes, AttackTimes == 3 ? "\n下一次符卡不消耗灵力。" : string.Empty);

        //符卡
        private float SC01Parameter => (5 + (int)this.CharacterLevel)*0.02f;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleSelf = () => MpGain((int)(this.MaxMp*SC01Parameter));
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        private const int SC02Range = 5;
        private const float SC02Gain = 1.5f;
        private int SC02Parameter1 => this.CharacterLevel > Level.Normal ? -2 : -1;
        private int SC02Parameter2
        {
            get
            {
                switch (this.CharacterLevel)
                {
                    case Level.Easy:
                        return 0;
                    case Level.Lunatic:
                        return 10;
                    default:
                        return 5;
                }
            }
        }
        private int SC02Parameter3 => this.CharacterLevel == Level.Lunatic ? 2 : 1;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC02Range);
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, SC02Gain);
                var buff1 = BuffAddProperty.BuffAddAttackRange(SCee, this, this.Interval*SC02Parameter3, SC02Parameter1,
                    game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDown(SCee, this, this.Interval*SC02Parameter3, SC02Parameter2, game);
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

        private const int SC03Range = 5;
        private const float SC03Gain = 1.7f;
        private int SC03Parameter => 1 + (int)this.CharacterLevel;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC03Range);
            game.HandleIsTargetLegal =
                (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, SC03Gain);
                var buff = new BuffBeAttacked(SCee, this, this.Interval*SC03Parameter, (int) (0.25f*this.Attack), this,
                    game);
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
