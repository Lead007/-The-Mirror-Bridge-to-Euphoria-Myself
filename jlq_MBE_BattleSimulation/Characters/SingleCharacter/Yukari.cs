using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>八云紫</summary>
    public class Yukari : CharacterTeleportMoving
	{
		public Yukari(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //天赋
		    _skillMove = (l, m) =>
		    {
		        if (!game.IsMoving) return;
		        if ((game.MouseColumn + this.X == Game.Column || game.MouseRow + this.Y == Game.Row) &&
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
		        Enemy.Where(c => SC02IsTargetLegal(c, game.MousePoint)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示所有敌方角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemy.SetLabelBackground();
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
                var d = Math.Max(Math.Abs(SCee.X - 4), Math.Abs(SCee.Y - 4));
                HandleDoDanmakuAttack(SCee, d*0.5f);
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        private bool SC02IsTargetLegal(Character SCee, Point point)
        {
            return (point.X == this.X && SCee.X == this.X && ((SCee.Y > this.Y) == (point.Y > this.Y))) ||
                   (point.Y == this.Y && SCee.Y == this.Y && ((SCee.X > this.X) == (point.X > this.X)));
        }
    }
}
