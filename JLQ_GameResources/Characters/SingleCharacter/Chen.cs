using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.Add.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>橙</summary>
    public class Chen : Character
	{
		public Chen(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
            //天赋
		    _skillMove = (l, m) =>
		    {
		        if (!game.IsMoving) return;
		        var c = this.game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        var p = this.X == c.X
		            ? new Point(c.X, c.Y + (this.Y > c.Y ? 1 : -1))
		            : new Point(c.X + (this.X > c.X ? 1 : -1), c.Y);
		        if (game[p] != null) return;
		        this.Move(p);
		        HandleDoAttack(c, 0.5f);
		        game.HasMoved = true;
		        game.IsMoving = false;
		        game.ResetPadButtons();
		        game.UpdateLabelBackground();
		        //如果同时已经攻击过则进入结束阶段
		        if (!game.HasAttacked || !game.HasMoved) return;
		        //Thread.Sleep(500);
		        game.EndSection();
		    };
            enterPad[1] = (s, ev) =>
            {
                if (!game.MousePoint.IsInRange(this, SC02Range)) return;
                game.DefaultButtonAndLabels();
                EnemyInMouseRange(SC02Range2).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemies.SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int SC02Range = 4;
        private const int SC02Range2 = 1;
        private readonly DGridPadClick _skillMove;

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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffAddMoveAbility(this, this, this.BuffTime, 1, game);
                buff1.BuffTrigger();
                var buff2 = new BuffSlowDownGain(this, this, this.BuffTime, -0.2, game);
                buff2.BuffTrigger();
            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => point.IsInRange(this, SC02Range);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC02Range2, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
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
            game.HandleSelf = () => Move(Game.CenterPoint);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 0.25f * (9 - SCee.Distance(this)));
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
	}
}
