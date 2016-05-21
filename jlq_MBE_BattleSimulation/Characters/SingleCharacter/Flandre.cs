using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>芙兰</summary>
    class Flandre : Character
	{
		public Flandre(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    enterButton[0] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        FList.Aggregate(GameColor.BaseColor, (c, f) => f.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterButton[1] = (s, ev) =>
		    {
		        SC02points.Aggregate(0.0, (c, p) => game.GetButton(p).Opacity = 1);
		    };
            SetDefaultLeaveSCButtonDelegate(1);
		    enterPad[2] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemy.Where(c => Calculate.IsIn33(game.MousePoint, c.Position))
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private const int SC02Range = 2;
        private const int SC02Num = 3;
        private const float SC03Gain = 1.7f;

        private List<FlandreLittle> FList { get; } = new List<FlandreLittle>();

        private IEnumerable<Point> SC02points
            => Game.PadPoints.Where(p => Calculate.Distance(p, this) <= SC02Range && game[p] == null);

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
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var plist = SC02points.ToList();
                var ps = new Point[3];
                for (var i = 0; i < SC02Num; i++)
                {
                    var index = random.Next(plist.Count);
                    var point = plist[index];
                    plist.Remove(point);
                    ps[i] = point;
                }
                foreach (var p in ps)
                {
                    game.AddCharacter(p, this.Group, "芙分");
                    this.FList.Add((FlandreLittle) game.Characters.Last());
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
            game.HandleIsTargetLegal = (SCee, point) => Calculate.IsIn33(point, SCee.Position) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        public override void SCShow()
        {
            AddSCButtonEvent(0);
            AddSCButtonEvent(1);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(0);
            RemoveSCButtonEvent(1);
        }
    }
}
