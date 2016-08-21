using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.BuffAboutCharacter;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>大妖精</summary>
    public class Daiyousei : Character
    {
        public Daiyousei(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将瞬移到的点和将回血的角色
            enterPad[0] = (s, ev) =>
            {
                if (!game.MousePoint.IsInRange(this, SC01Range1) || game.MouseCharacter != null) return;
                game.DefaultButtonAndLabels();
                game.GetButton(game.MousePoint).SetButtonColor();
                game.Characters.Where(c => SC01IsTargetLegal(c, game.MousePoint)).DoAction(c =>
                    c.SetLabelBackground(HpPercent(c) > SC01Parameter1
                        ? GameColor.LabelBackground
                        : GameColor.LabelBackground2));
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
            enterPad[1] = (s, ev) =>
            {
                if (game.MousePoint.Distance(this) != 1) return;
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => SC02IsTargetLegal(c, game.MousePoint))
                    .DoAction(c => c.SetLabelBackground(IsEnemy(c) ? GameColor.LabelBackground : GameColor.LabelBackground2));
            };
            SetDefaultLeavePadButtonDelegate(1);
            game.ETurnToBattle += () =>
            {
                foreach (var c in game.Characters.Where(c => IsFriend(c)).ToArray())
                {
                    c.EWillDie += () => SC03Rebirth(c);
                    {
                        if (!this.SC03HasUsed)
                        {
                            c.Cure(-c.Hp);
                            c.Cure(SC03Parameter);
                        }
                    }
                }
            };
        }

        private static float HpPercent(Character c) => ((float)c.Hp) / c.MaxHp;

        private const int skillRange = 2;
        private static Tuple<float> skillGain { get; } = new Tuple<float>(0.05f);

        /// <summary>天赋</summary>
        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC[2].IsEnabled = false;
            var cc = game.Characters.Where(c => IsInRangeAndFriend(skillRange, c))
                .Min(c => new CharacterHpPercentComparable(c, random)).Character;
            cc.Cure(skillGain);
            var buff = new BuffDayouseiCure(cc, this, this.Interval, game);
        }

        //符卡
        private const int SC01Range1 = 3;
        private const int SC01Range2 = 1;
        private float SC01Parameter1 => (Math.Min(2, (int)this.CharacterLevel) + 1) * 0.1f;
        private float SC01Gain2 => this.CharacterLevel == Level.Lunatic ? 0.5f : 0.3f;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC01Range1) && game[point] == null;
            game.HandleIsTargetLegal =
                (SCee, point) => (point.IsInRange(SCee, SC01Range2) && IsFriend(SCee, false)) || SCee == this;
            game.HandleSelf = () => Move(game.MousePoint);
            game.HandleTarget = SCee => SCee.Cure(this.Attack * (HpPercent(SCee) <= SC01Parameter1 ? 1 : (1 + SC01Gain2)));
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        #region 符卡01相关函数
        private bool SC01IsTargetLegal(Character SCee, PadPoint point) =>
            (point.IsInRange(SCee, SC01Range2) && IsFriend(SCee, false)) || SCee == this ||
            SCee.BuffList.OfType<BuffDayouseiCure>().Any(b => b.Buffer == this);
        #endregion

        private const float SC02Gain = 1.5f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = SC02IsTargetLegal;
            game.HandleTarget = SCee =>
            {
                if (IsEnemy(SCee)) HandleDoDanmakuAttack(SCee, SC02Gain);
                else
                {
                    var buff = new BuffDayouseiCure(SCee, this, this.BuffTime, game);
                }
            };
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }

        #region 符卡02相关函数
        private bool SC02IsTargetLegal(Character SCee, PadPoint point) =>
            SCee != this && (point.Column == this.Column
                ? ((SCee.Row > this.Row) == (point.Row > this.Row))
                : ((SCee.Column > this.Column) == (point.Column > this.Column)));
        #endregion

        private bool SC03HasUsed { get; set; } = false;
        private Tuple<float> SC03Parameter => new Tuple<float>((2 + (int) this.CharacterLevel)*0.1f);
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            throw new NotImplementedException();
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        #region 符卡03相关函数
        private void SC03Rebirth(Character c)
        {
            if (!this.SC03HasUsed)
            {
                c.Cure(SC03Parameter);
                this.SC03HasUsed = true;
            }
        }
        #endregion

        public override string ToString() => base.ToString() + string.Format("\n符卡03{0}使用", SC03HasUsed ? "已" : "未");
    }
}
