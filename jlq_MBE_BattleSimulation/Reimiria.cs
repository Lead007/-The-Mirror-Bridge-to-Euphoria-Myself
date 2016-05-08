using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>
    /// 蕾米
    /// </summary>
	class Reimiria : Character
	{
		public Reimiria(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡03
            //显示将被攻击的角色
            enterPad[2] = (s, ev) =>
            {
                if (Calculate.Distance(game.MousePoint, this) != 1) return;
                game.DefaultButtonAndLabels();
                Enemy.Where(c => SC03IsTargetLegal(c, game.MousePoint))
                    .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        private const float SC03Gain = 2.0f;

	    public override bool DoAttack(Character target, float times = 1)
	    {
            //判断是否命中
            if (HandleIsHit(target)) return false;
            //判断是否近战
            var closeGain = HandleCloseGain(target);
            //计算基础伤害
	        double damage;
	        if (target.Hp*10 < target.Data.MaxHp) damage = 9999;
            else damage = /*基础伤害*/ Calculate.Damage(this.Attack, target.Defence) * /*近战补正*/closeGain * FloatDamage * times;
            //判断是否暴击
            var isCriticalHit = HandleIsCriticalHit(target);
            if (isCriticalHit)
            {
                damage *= this.CriticalHitGain;
            }
            target.HandleBeAttacked((int)damage, this);
            return isCriticalHit;
        }

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            //TODO SC01
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {

        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) == 1;
            game.HandleIsTargetLegal = (SCee, point) => SC03IsTargetLegal(SCee, point) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoAttack(SCee, SC03Gain);
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


    }
}
