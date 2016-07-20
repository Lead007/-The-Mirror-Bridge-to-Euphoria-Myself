using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>蕾蒂</summary>
    public class Letty : Character
    {
        public Letty(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            enterPad[0] = (s, ev) =>
            {
                if (game.MousePoint.IsInRange(this, SC01Range)) return;
                game.DefaultButtonAndLabels();
                EnemyInMouseRange(SC01Range2).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            enterButton[2] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                Enemies.SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
        }

        private const int skillRange = 2;
        private int skillParameter => ((int)this.CharacterLevel << 1) + 5;
        public override void PreparingSection()
        {
            Enemies.Where(c => c.Distance(this) <= skillRange)
                .Select(c => new BuffSlowDownGain(c, this, this.Interval, skillParameter, game))
                .DoAction(b => b.BuffTrigger());
        }

        //符卡
        private const int SC01Range = 4;
        private const int SC01Range2 = 1;
        private const float SC01Gain = 1.2f;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC01Range);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(SC01Range2, SCee);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, SC01Gain);
                var buff = BuffAddProperty.BuffAddMoveAbility(SCee, this, 2 * this.Interval, -1, game);
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

        private float SC02Parameter => (7 + (int)this.CharacterLevel) * 0.05f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () => Cure(SC02Parameter * (this.MaxHp - this.Hp));
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        private int SC03Parameter
        {
            get
            {
                switch (this.CharacterLevel)
                {
                    case Level.Easy:
                        return 10;
                    case Level.Normal:
                        return 12;
                    case Level.Hard:
                        return 13;
                    default:
                        return 15;
                }
            }
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                var buff = new BuffSlowDown(SCee, this, this.BuffTime, SC03Parameter, game);
                buff.BuffTrigger();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

    }
}
