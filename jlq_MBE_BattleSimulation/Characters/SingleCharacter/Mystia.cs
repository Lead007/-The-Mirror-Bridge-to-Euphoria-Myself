using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Add.Sealed;
using RandomHelper;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    public class Mystia : Character
	{
		public Mystia(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡02
            //显示将受影响的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) > 4) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => game.MousePoint.IsIn33(c.Position)).SetLabelBackground();
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
            game.HandleIsLegalClick = point => point.Distance(this) <= 4;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && point.IsIn33(SCee.Position);
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffAddHitRate(SCee, this, BuffTime, -10, game);
                buff1.BuffTrigger();
                var buff2 = new BuffAddDodgeRate(SCee, this, BuffTime, -10, game);
                buff2.BuffTrigger();
                var buffs = SCee.BuffList.Where(b => b.IsPositive).ToList();
                if (!buffs.Any()) return;
                var bs = random.RandomElement(buffs);
                bs.BuffEnd();
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

        }

    }
}
