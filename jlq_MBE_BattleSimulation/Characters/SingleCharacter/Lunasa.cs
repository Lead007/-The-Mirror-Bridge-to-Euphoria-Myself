using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.Gain.Sealed;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;
using JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChoosePoints;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>露娜萨·普莉兹姆利巴</summary>
    public class Lunasa : CharacterPoltergeist
	{
		public Lunasa(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        c.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(0);
		}

        public override void PreparingSection()
        {
            base.PreparingSection();
            foreach (var buff in game.Characters.Where(c => IsInRangeAndEnemy(skillRange, c)).Select(c => new BuffSlowDownGain(c, this, this.Interval, 0.1f, game)))
            {
                buff.BuffTrigger();
            }
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
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
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
	}
}
