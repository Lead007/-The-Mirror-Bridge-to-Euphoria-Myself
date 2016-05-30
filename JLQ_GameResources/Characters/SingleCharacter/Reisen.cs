using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>铃仙·优昙华院·因幡</summary>
    public class Reisen : Character
	{
		public Reisen(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterPad[0] = (s, ev) =>
		    {
		        if (!IsInRangeAndEnemy(3, game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => c.X == game.MouseColumn || c.Y == game.MouseRow).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
		    enterPad[1] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemy.SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(3, point);
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && (SCee.X == point.X || SCee.Y == point.Y);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemy.Where(c => this.Distance(c) <= 3).SetLabelBackground();
            };
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
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var buff = new BuffCannotMove(SCee, this, this.Interval, game);
                buff.BuffTrigger();
            };
            AddPadButtonEvent(1);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemy.SetLabelBackground();
            };
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
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

    }
}
