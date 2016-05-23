using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>灵梦</summary>
    public class Reimu : CharacterTeleportMoving
    {
        public Reimu(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {
            //符卡01
            //显示将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) > SC01Range) return;
                game.DefaultButtonAndLabels();
                var cs = Enemy.Where(c => Calculate.IsIn33(game.MousePoint, c.Position)).ToList();
                if (cs.Count == 1)
                {
                    cs[0].LabelDisplay.Background = GameColor.LabelBackground2;
                }
                else
                {
                    cs.Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
                }
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被影响的目标和被监禁的范围
            enterPad[1] = (s, ev) =>
            {
                if (!game.HandleIsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                var points = Game.PadPoints.Where(p => Calculate.IsIn33(game.MousePoint, p));
                foreach (var p in points)
                {
                    var c = game[p];
                    if (c == null) game.GetButton(p).Opacity = 1;
                    else if (Enemy.Contains(c)) c.LabelDisplay.Background = GameColor.LabelBackground;
                }
            };
            SetDefaultLeavePadButtonDelegate(1);
        }

        /// <summary>符卡01的参数</summary>
        private const int SC01Range = 4;
        private float SC01Gain;
        /// <summary>符卡02的参数</summary>
        private const int SC02Gain = 10;

        private List<Character> SC02CharactersBeSlowed = new List<Character>();


        /// <summary>天赋：1.2倍灵力获取</summary>
        /// <param name="mp">获得的灵力量</param>
        public override void MpGain(int mp)
        {
            base.MpGain(Calculate.Floor(1.2*mp));
        }

        /// <summary>符卡01：梦想封印，对所有4格内的敌人造成1.0倍率的弹幕攻击</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) <= SC01Range;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && Calculate.IsIn33(point, SCee.Position);
            game.HandleSelf = () => SC01Gain =
                game.Characters.Count(c => game.HandleIsTargetLegal(c, game.MousePoint)) == 1 ? 6 : 1.5f;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01Gain);
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
            game.HandleIsLegalClick =
                point => point.X > 0 && point.X < Game.Column - 1 && point.Y > 0 && point.Y < Game.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) => Calculate.IsIn33(point, SCee.Position) && IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffSlowDown(SCee, this, this.BuffTime, SC02Gain, game);
                buff1.BuffTrigger();
                DIsPointWall handle = (origin, point) =>
                {
                    var rx = Math.Abs(point.X - origin.X);
                    var ry = Math.Abs(point.Y - origin.Y);
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
