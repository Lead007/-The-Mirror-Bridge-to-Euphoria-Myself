using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JLQ_BaseBuffs.Gain.Sealed;
using JLQ_GameBase;
using JLQ_GameResources.Characters.CharacterBeCalled.AliceFigures;
using JLQ_GameResources.Dialogs.GamePad.ChoosePoints;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>爱丽丝·玛格特洛依德</summary>
	public class Alice : Character
	{
		public Alice(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterPad[1] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(1);
		    enterPad[2] = (s, ev) =>
		    {
		        if (!game.MousePoint.IsInRange(this, SC03Range1)) return;
		        game.DefaultButtonAndLabels();
		        EnemyInMouseRange(SC03Range2).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private const int SC03Range1 = 5;
        private const int SC03Range2 = 2;

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
            var dialog = new GamePad_AliceSC02(this, game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsLegalClick = point => IsEnemy(game[point]);
                game.HandleIsTargetLegal = (SCee, point) => false;
                game.HandleSelf = () =>
                {
                    var point = dialog.PointsChoose.Peek();
                    game.AddCharacter(point, Group.Middle, typeof (AliceFigure2), point, game, game.MouseCharacter,
                        this.Attack, this.Defence);
                    FigureBuff();
                };
                AddPadButtonEvent(1);
                game.HandleResetShow = () => Enemies.SetLabelBackground();
            }
            else
            {
                game.HandleIsLegalClick = point => false;
            }
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
            game.HandleIsLegalClick = point => point.IsInRange(this, SC03Range1);
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC03Range2, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private void FigureBuff()
        {
            var buff1 = new BuffGainAttack(this, this, 5*this.Interval, 0.1, game);
            buff1.BuffTrigger();
            var buff2 = new BuffGainDefence(this, this, 5*this.Interval, -0.05, game);
            buff2.BuffTrigger();
        }
    }
}
