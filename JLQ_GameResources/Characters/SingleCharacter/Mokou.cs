using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    public class Mokou : Character, IHuman
    {
        public Mokou(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        public Human HumanKind => Human.Mokou;

        //TODO 天赋

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
