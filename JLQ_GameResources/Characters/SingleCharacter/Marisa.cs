using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>魔理沙</summary>
    public class Marisa : Character, IHuman
    {
        public Marisa(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                EnemyInMouseRange(1).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemies.Where(c => SC02IsTargetLegal(c, this.Position)).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将瞬移到的点、将被受影响的角色和其将被移动至的位置（若存在）
            enterPad[2] = (s, ev) =>
            {
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        public Human HumanKind => Human.FullHuman;

        //天赋
        /// <summary>天赋参数</summary>
        private int SkillParameter => (int)this.CharacterLevel*1000;
        /// <summary>天赋带来的符卡伤害增益</summary>
        private float SkillSCDamageGain => ((float) this.Mp)/SkillParameter;

        //符卡
        /// <summary>符卡01增益</summary>
        private float SC01Gain => this.CharacterLevel == Level.Easy || this.CharacterLevel == Level.Normal ? 1 : 1.2f;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => true;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, 1, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01Gain*SkillSCDamageGain);
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        private const int SC02Gain1 = 2;
        private const float SC02Gain2 = 1.5f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = SC02IsTargetLegal;
            game.HandleTarget = SCee =>
                HandleDoDanmakuAttack(SCee,
                    (Math.Min(Math.Abs(SCee.Column - this.Column), Math.Abs(SCee.Row - this.Row)) == 0
                        ? SC02Gain1
                        : SC02Gain2)*SkillSCDamageGain);
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }

        #region SC02相关函数
        /// <summary>SC02目标是否合法</summary>
        /// <param name="SCee">目标</param>
        /// <param name="point">单击点</param>
        /// <returns>是否合法</returns>
        private bool SC02IsTargetLegal(Character SCee, PadPoint point)
        {
            if (!IsEnemy(SCee)) return false;
            int rRange;
            switch (this.CharacterLevel)
            {
                case Level.Easy:
                case Level.Normal:
                    rRange = 0;
                    break;
                default:
                    rRange = 1;
                    break;
            }
            if (point.Column == this.Column)
            {
                return Math.Abs(SCee.Column - this.Column) <= rRange && ((point.Row > this.Row) == (SCee.Row > this.Row));
            }
            return Math.Abs(SCee.Row - this.Row) <= rRange && ((point.Column > this.Column) == (SCee.Column > this.Column));
        }
        #endregion

        private int SC03Range => this.CharacterLevel == Level.Lunatic ? 3 : 2;
        private const int SC03Gain = 2;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () => Move(this.Position.FacePoint(game.MousePoint));
            //TODO No Defence
            game.HandleTarget = SCee =>
            {
                //判断是否命中
                if (HandleIsHit(SCee)) return;
                //判断是否近战
                var closeGain = HandleCloseGain(SCee);
                //计算基础伤害
                var damage = /*基础伤害*/ Calculate.Damage(this.Attack, SCee.Defence)* /*近战补正*/closeGain*FloatDamage*SC03Gain;
                //判断是否暴击
                var isCriticalHit = HandleIsCriticalHit(SCee);
                if (isCriticalHit)
                {
                    damage *= this.CriticalHitGain;
                }
            };
            AddPadButtonEvent(2);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
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
