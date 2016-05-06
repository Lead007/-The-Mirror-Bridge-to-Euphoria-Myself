using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
	class Reimiria : Character
	{
		public Reimiria(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        //TODO 天赋
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
            //TODO SC03
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {

        }

    }
}
