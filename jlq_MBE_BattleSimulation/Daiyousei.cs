using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>大妖精</summary>
	class Daiyousei : Character
	{
		public Daiyousei(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示有效单击点
		    enterButton[0] = (s, ev) =>
		    {
		        this.game.DefaultButtonAndLabels();
		        Game.PadPoints.Where(point => this.Position != point && SC01IsLegalClick(point))
		            .Aggregate((Brush) Brushes.White, (c, point) => game[point].LabelDisplay.Background = Brushes.LightBlue);
		        pointTemp1 = Game.DefaultPoint;
		    };
            SetDefaultLeaveSCButtonDelegate(0);
            //显示将瞬移到的点和将回血的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        if (this.Position == game.MousePoint) return;
		        if (this.Position != pointTemp1)
		        {
		            game.GetButton(pointTemp1).Opacity = 1;
		        }
		        game[game.MousePoint].LabelDisplay.Background = Brushes.LightBlue;
		        pointTemp1 = Game.DefaultPoint;
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示有效单击点
		    enterButton[1] = (s, ev) =>
		    {
                this.game.DefaultButtonAndLabels();
		        Game.PadPoints.Where(SC02IsLegalClick)
		            .Aggregate((Brush) Brushes.White, (c, point) => game[point].LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeaveSCButtonDelegate(1);
            //显示将被攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (!SC02IsLegalClick(game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        game[game.MousePoint].LabelDisplay.Background = Brushes.LightBlue;
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //显示将回血的角色
		    enterButton[2] = (s, ev) =>
		    {
                this.game.DefaultButtonAndLabels();
		        game.Characters.Where(c => c != this && SC03IsTargetLegal(c))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int skillRange = 2;
        private const float skillGain = 0.05f;
        private const int SC01Range = 4;
        private const int SC02Range = 2;
        private const float SC02Gain = 1.5f;

        private Point pointTemp1 = Game.DefaultPoint;

        /// <summary>天赋：雾之湖的恩惠</summary>
        public override void PreparingSection()
        {
            foreach (var c in game.Characters.Where(
                        c => c.Group == this.Group && c != this &&
                            Calculate.Distance(c, this) <= skillRange))
            {
                c.Cure((int) (skillGain*c.Data.MaxHp));
            }
        }

        //符卡
        /// <summary>符卡01：贴心的妖精，选择4格内一个目标（可以是自己），瞬移到他背后（如果是自己就不用瞬移），并使目标回复自己攻击力0.7倍率的目标血量。</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () => Move(pointTemp1);
            game.HandleTarget = SCee => SCee.Cure((int) (0.7*this.Attack));
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02：花仙炮，对4格内一敌方目标造成1.5倍率的伤害。</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = SC02IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => DoAttack(SCee, 1.5f);
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }
        /// <summary>符卡03：妖精狂欢，使2格内所有己方角色恢复自己攻击力1.5倍率的血量</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => SC03IsTargetLegal(SCee);
            game.HandleTarget = SCee => SCee.Cure((int) (SCee.Attack*SC02Gain));
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        public override void SCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                AddSCButtonEvent(i);
            }
        }

        public override void ResetSCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                RemoveSCButtonEvent(i);
            }
        }

        private bool SC01IsLegalClick(Point point)
        {
            if (Calculate.Distance(point, this) > SC01Range || game[point] == null) return false;
            if (point == this.Position)
            {
                pointTemp1 = point;
                return true;
            }
            if (point.Y == this.Position.Y)
            {
                if (point.X > this.Position.X)
                {
                    if (point.X == 8) return false;
                    pointTemp1 = new Point(point.X + 1, point.Y);
                    if (game[pointTemp1] == null) return true;
                    pointTemp1 = Game.DefaultPoint;
                    return false;
                }
                if (point.X == 0) return false;
                pointTemp1 = new Point(point.X - 1, point.Y);
                if (game[pointTemp1] == null) return true;
                pointTemp1 = Game.DefaultPoint;
                return false;
            }
            if (point.Y > this.Position.Y)
            {
                if (point.Y == 8) return false;
                pointTemp1 = new Point(point.X, point.Y + 1);
                if (game[pointTemp1] == null) return true;
                pointTemp1 = Game.DefaultPoint;
                return false;
            }
            if (point.Y == 0) return false;
            pointTemp1 = new Point(point.X, point.Y - 1);
            if (game[pointTemp1] == null) return true;
            pointTemp1 = Game.DefaultPoint;
            return false;
        }

        private bool SC02IsLegalClick(Point point)
        {
            if (Calculate.Distance(point, this) > SC01Range) return false;
            return IsEnemy(game[point]);
        }

        private bool SC03IsTargetLegal(Character SCee)
        {
            return Calculate.Distance(SCee, this) <= SC02Range && SCee.Group == this.Group;
        }
    }
}
