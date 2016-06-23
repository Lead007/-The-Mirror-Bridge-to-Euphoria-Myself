using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>铃仙·优昙华院·因幡</summary>
    public class Reisen : Character
	{
		public Reisen(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterPad[0] = (s, ev) =>
		    {
		        if (!IsInRangeAndEnemy(SC01Range, game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => c.Column == game.MouseColumn || c.Row == game.MouseRow).SetLabelBackground();
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
		        Enemies.SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int SC01Range = 3;

        //TODO 天赋

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC01Range, point);
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && (SCee.Column == point.Column || SCee.Row == point.Row);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                EnemyInRange(SC01Range).SetLabelBackground();
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
                Enemies.SetLabelBackground();
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
