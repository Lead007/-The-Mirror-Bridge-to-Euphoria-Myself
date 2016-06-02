using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>蓬莱山辉夜</summary>
    public class Kaguya : Character
	{
		public Kaguya(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡02
            //显示将被攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

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
            game.HandleIsLegalClick = point => true;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.5f);
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

        }

    }
}
