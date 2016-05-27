using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.Gain.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>魂魄妖梦</summary>
    public class Youmu : Character, IHuman
    {
		public Youmu(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将被攻击的角色，鼠标处角色特殊标出
		    enterPad[0] = (s, ev) =>
		    {
		        if (!SC01IsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        var mc = game.MouseCharacter;
		        Enemy.Where(c => game.MousePoint.IsIn33(c.Position))
		            .DoAction(c => c.SetLabelBackground(c == mc ? GameColor.LabelBackground2 : GameColor.LabelBackground));
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡03
            //显示将受影响的角色
		    enterPad[2] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        public Human HumanKind => Human.HalfHuman;

        private const int SC01Range = 3;

        private int _attackTime = 1;
        public override bool HasAttacked
        {
            get { return _attackTime == 0; }
            set
            {
                if (value)
                {
                    _attackTime--;
                }
                else
                {
                    _attackTime = 1;
                }
            }
        }

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee.Position);
            game.HandleTarget = SCee =>
            {
                if (SCee.Position == game.MousePoint)
                {
                    HandleDoDanmakuAttack(SCee, 1.5f);
                }
                else
                {
                    HandleDoDanmakuAttack(SCee, 0.5f);
                }
            };
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
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                this.BuffList.RemoveAll(b => b.IsPositive == false);
                var buff1 = new BuffGainBeDamaged(this, this, this.Interval + 1, -0.25f, game);
                buff1.BuffTrigger();
                var buff2 = new BuffGainAttack(this, this, this.Interval + 1, 0.5f, game);
                buff2.BuffTrigger();
                game.ButtonSC.DoAction(b => b.IsEnabled = false);
                _attackTime++;
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 1.5f);
                SCee.AttackAdd = -20;
                SCee.DefenceAdd = -20;
                SCee.HitRateAdd = -20;
                SCee.DodgeRateAdd = -20;
            };
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private bool SC01IsLegalClick(Point point)
        {
            return (point.X == this.X && Math.Abs(point.Y - this.Y) >= SC01Range) ||
                   (point.Y == this.Y && Math.Abs(point.X - this.X) >= SC01Range);
        }

    }
}
