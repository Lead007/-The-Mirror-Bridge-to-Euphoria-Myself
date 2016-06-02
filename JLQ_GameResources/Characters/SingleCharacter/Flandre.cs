using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using JLQ_GameResources.Characters.CharacterBeCalled;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>芙兰</summary>
    public class Flandre : Character
	{
		public Flandre(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterButton[0] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
                FList.SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterButton[1] = (s, ev) =>
		    {
		        SC02points.Select(game.GetButton).SetButtonColor();
		    };
            SetDefaultLeaveSCButtonDelegate(1);
		    enterPad[2] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private const int SC02Range = 2;
        private const int SC02Num = 3;
        private const float SC03Gain = 1.7f;

        private List<FlandreLittle> FList { get; } = new List<FlandreLittle>();

        private IEnumerable<Point> SC02points
            => Game.PadPoints.Where(p => p.Distance(this) <= SC02Range && game[p] == null);

        public override void PreparingSection()
        {
            base.PreparingSection();
            if (SC02points.Count() < SC02Num)
            {
                game.ButtonSC[1].IsEnabled = false;
            }
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee == this || FList.Contains(SCee);
            game.HandleTarget = SCee =>
            {
                var buff = new BuffLetBloodingWhenBeAttacked(SCee, this, this.BuffTime, this.BuffTime, game);
                buff.BuffTrigger();
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
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                foreach (var p in random.RandomElements(SC02Num, SC02points))
                {
                    game.AddCharacter(p, this.Group, typeof (FlandreLittle), p, this.Group, this.game);
                    this.FList.Add(game.Characters.Last() as FlandreLittle);
                }
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick =
                point => point.X > 0 && point.X < Game.Column - 1 && point.Y > 0 && point.Y < Game.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) => point.IsIn33(SCee) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }
    }
}
