using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>西行寺幽幽子</summary>
    public class Yuyuko : Character
	{
		public Yuyuko(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{

		}

        public override bool DoAttack(Character target, float times = 1)
        {
            //判断是否命中
            if (HandleIsHit(target)) return false;
            //计算伤害增益
            float gain;
            switch (this.Distance(target))
            {
                case 1:
                    gain = 1.2f;
                    break;
                case 2:
                    gain = 1.1f;
                    break;
                case 3:
                    gain = 1;
                    break;
                default:
                    gain = 0.9f;
                    break;
            }
            //造成伤害
            return DoingAttack(target, times * gain);

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
