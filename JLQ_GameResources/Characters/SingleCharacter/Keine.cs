using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    public class Keine : Character
	{
		public Keine(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{

		}

        private bool _isBaize = false;

        private bool IsBaize
        {
            get { return _isBaize; }
            set
            {
                _isBaize = value;
                this.LabelDisplay.Content = value ? "慧白" : "慧";
                this.CloseAmendmentX = value ? (2/1.1) : (1.1/2);
            }
        }

        //天赋
        public override void EndSection()
        {
            base.EndSection();
            _isBaize = random.NextBool(1.0/6);
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
