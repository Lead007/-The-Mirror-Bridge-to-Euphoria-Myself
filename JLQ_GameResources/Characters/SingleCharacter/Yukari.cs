using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>八云紫</summary>
    public class Yukari : CharacterTeleportMoving
	{
		public Yukari(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //天赋
		    _skillMove = (l, m) =>
		    {
		        if (!game.IsMoving) return;
		        if ((game.MouseColumn + this.Column == Game.Column || game.MouseRow + this.Row == Game.Row) &&
		            !game.CanReachPoint[game.MouseColumn, game.MouseRow])
		        {
		            //移动
		            this.Move(game.MousePoint);
		            game.HasMoved = true;
		            game.IsMoving = false;
		            game.ResetPadButtons();
		            game.UpdateLabelBackground();
		            //如果同时已经攻击过则进入结束阶段
		            if (!game.HasAttacked || !game.HasMoved) return;
		            //Thread.Sleep(500);
		            game.EndSection();
		        }
		    };
            //符卡02
            //显示将被攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) != 1) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => SC02IsTargetLegal(c, game.MousePoint)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示所有敌方角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemies.SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private readonly DGridPadClick _skillMove;
        private const float SC02Gain = 0.7f;

        //天赋
        public override void PreparingSection()
        {
            base.PreparingSection();
            if (game[Game.CenterPoint] != null)
            {
                game.ButtonSC[2].IsEnabled = false;
            }
            game.EventGridPadClick += _skillMove;
        }
        public override void EndSection()
        {
            base.EndSection();
            game.EventGridPadClick -= _skillMove;
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
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && SC02IsTargetLegal(SCee, point);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02Gain);
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
            game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee);
            game.HandleTarget = SCee =>
            {
                var d = Math.Max(Math.Abs(SCee.Column - 4), Math.Abs(SCee.Row - 4));
                HandleDoDanmakuAttack(SCee, d*0.5f);
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        private bool SC02IsTargetLegal(Character SCee, PadPoint point)
        {
            return (point.Column == this.Column && SCee.Column == this.Column && ((SCee.Row > this.Row) == (point.Row > this.Row))) ||
                   (point.Row == this.Row && SCee.Row == this.Row && ((SCee.Column > this.Column) == (point.Column > this.Column)));
        }
    }
}
