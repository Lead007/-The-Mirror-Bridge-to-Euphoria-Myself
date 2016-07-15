using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>灵梦</summary>
    public class Reimu : CharacterTeleportMoving, IHuman
    {
        public Reimu(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                if (!game.MousePoint.IsInRange(this, SC01Range)) return;
                game.DefaultButtonAndLabels();
                var cs = Enemies.Where(c => game.MousePoint.IsIn33(c)).ToList();
                if (cs.Count == 1)
                {
                    cs[0].LabelDisplay.Background = GameColor.LabelBackground2;
                }
                else
                {
                    cs.SetLabelBackground();
                }
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被影响的目标和被监禁的范围
            enterPad[1] = (s, ev) =>
            {
                if (!game.HandleIsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var points = Game.PadPoints.Where(p => game.MousePoint.IsIn33(p));
                foreach (var p in points)
                {
                    var c = game[p];
                    if (c == null) game.GetButton(p).SetButtonColor();
                    else if (IsEnemy(c)) c.SetLabelBackground();
                }
            };
            SetDefaultLeavePadButtonDelegate(1);
        }

        private List<Character> SC02CharactersBeSlowed = new List<Character>();

        public Human HumanKind => Human.FullHuman;

        //天赋
        /// <summary>天赋增益</summary>
        private float SkillGain => 1.1f + (int)CharacterLevel*0.05f;
        public override void MpGain(int mp)
        {
            base.MpGain((SkillGain*mp).Floor());
        }

        //符卡
        /// <summary>符卡01距离</summary>
        private const int SC01Range = 4;
        /// <summary>符卡01真实增益</summary>
        private float _SC01Gain;
        /// <summary>符卡01增益1</summary>
        private float SC01Gain1 => this.CharacterLevel == Level.Easy ? 1.5f : 2;
        /// <summary>符卡01增益2</summary>
        private int SC01Gain2 => this.CharacterLevel == Level.Easy ? 6 : 8;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC01Range);
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleSelf = () => _SC01Gain =
                game.Characters.Count(c => game.HandleIsTargetLegal(c, game.MousePoint)) == 1 ? SC01Gain2 : SC01Gain1;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, _SC01Gain);
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02增量</summary>
        private int SC02Increment
        {
            get
            {
                switch (this.CharacterLevel)
                {
                    case Level.Easy:
                        return 8;
                    case Level.Normal:
                        return 10;
                    case Level.Hard:
                        return 12;
                    default:
                        return 15;
                }
            }
        }
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick =
                point => point.Column > 0 && point.Column < Game.Column - 1 && point.Row > 0 && point.Row < Game.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) => point.IsIn33(SCee) && IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffSlowDown(SCee, this, this.BuffTime, SC02Increment, game);
                buff1.BuffTrigger();
                Func<PadPoint, PadPoint, bool> handle = (origin, point) =>
                {
                    var rx = Math.Abs(point.Column - origin.Column);
                    var ry = Math.Abs(point.Row - origin.Row);
                    return (rx == 2 && ry <= 2) || (ry == 2 && rx <= 2);
                };
                var buff2 = new BuffLimit(SCee, this, this.BuffTime, game.MousePoint, handle, game);
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

        /// <summary>符卡03持续回合数</summary>
        private int SC03CountOfRound => this.CharacterLevel == Level.Easy ? 1 : 2;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            //TODO SC03
        }

        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
    }
}
