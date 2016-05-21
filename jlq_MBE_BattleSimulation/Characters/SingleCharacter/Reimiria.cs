using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Number;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>蕾米</summary>
    class Reimiria : CharacterMayRepeatedlyDoDamage
	{
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">位置</param>
        /// <param name="group">阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
		public Reimiria(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡02
            //显示将被攻击的角色
            enterButton[1] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                Enemy.Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
            };
            SetDefaultLeaveSCButtonDelegate(1);
            //符卡03
            //显示将被攻击的角色
            enterPad[2] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => SC03IsTargetLegal(c, game.MousePoint))
                    .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        private const float SC02Gain = 0.7f;
        private const float SC03Gain = 2.0f;

        //TODO 天赋

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff = new BuffAddDamageTimes(this, this.BuffTime, 1, game);
                buff.BuffTrigger();
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
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                //判断是否命中
                if (HandleIsHit(SCee)) return;
                //造成伤害
                DoingAttack(SCee, SC02Gain);
                this.Cure(new RationalNumber(1, 20, true, false));
                if (SCee.IsDead) this.DamageTimes++;
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
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) == 1;
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

        private bool SC03IsTargetLegal(Character SCee, Point point)
        {
            if (point.X == this.X)
            {
                if (point.Y > this.Y)
                {
                    return SCee.X == this.X && SCee.Y > this.Y;
                }
                return SCee.X == this.X && SCee.Y < this.Y;
            }
            if (point.X > this.X)
            {
                return SCee.Y == this.Y && SCee.X > this.X;
            }
            return SCee.Y == this.Y && SCee.X < this.X;
        }

        public override void SCShow()
        {
            AddSCButtonEvent(1);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(1);
        }
	}
}
