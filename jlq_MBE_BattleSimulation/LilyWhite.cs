using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    class LilyWhite : Character
	{
		public LilyWhite(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        public bool IsWhite { get; set; } = true;
        private const int skillNum = 100;
        private const int SC01Range = 3;
        private const int SC01Num = 50;
        private const int SC02Range = 2;

        public override void PreparingSection()
        {
            base.PreparingSection();
            MpGain(IsWhite ? skillNum : (skillNum/2));
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            if (IsWhite)
            {
                game.HandleIsTargetLegal = (SCee, point) => Calculate.Distance(SCee, this) <= SC01Range;
                game.HandleTarget = SCee => SCee.MpGain(SC01Num);
            }
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            if (IsWhite)
            {
                game.HandleIsLegalClick = SC02IsLegalClick;
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee => SCee.MpGain(2*SCee.Attack);
            }
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
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

        private bool SC02IsLegalClick(Point point)
        {
            if (Calculate.Distance(point, this) > SC02Range) return false;
            var c = game[point];
            return c != null && c.Group == this.Group;
        }
    }
}
