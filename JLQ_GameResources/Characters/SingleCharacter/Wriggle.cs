using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>莉格露·奈特巴格</summary>
    public class Wriggle : Character
	{
		public Wriggle(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (game.MousePoint.Distance(this) != 1) return;
		        var cs = Enemy.Where(c => SC01IsTargetLegal(c, game.MousePoint)).ToList();
		        if (!cs.Any()) return;
		        game.DefaultButtonAndLabels();
		        cs[0].SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡03
            //显示将受影响的角色
		    enterPad[2] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        private List<Character> LightCharacters { get; } = new List<Character>();
        private List<Label> LightLabels { get; } = new List<Label>(); 

        public override void PreparingSection()
        {
            base.PreparingSection();
            foreach (var c in LightCharacters.Where(c => !game.Characters.Contains(c)))
            {
                LightLabels.RemoveAll(l => c.StatesControls.Contains(l));
                LightCharacters.Remove(c);
            }
            LightLabels.DoAction(l => l.Visibility = Visibility.Visible);
        }

        public override void EndSection()
        {
            base.EndSection();
            LightLabels.DoAction(l => l.Visibility = Visibility.Hidden);
        }

        //天赋
        public override bool DoingAttack(Character target, float times = 1)
        {
            var t = 1.0f;
            for (int i = 0, count = LightCharacters.Count(c => c == target); i < count; i++)
            {
                t *= 1.2f;
            }
            return base.DoingAttack(target, times*t);
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => point.Distance(this) == 1;
            game.HandleIsTargetLegal = SC01IsTargetLegal;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.2f);
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
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                var buff = new BuffGainBeDamaged(this, this, this.Interval, -0.5f, game);
                buff.BuffTrigger();
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
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var count = LightCharacters.Count(c => c == SCee);
                if (count == 5) return;
                LightCharacters.Add(SCee);
                if (count == 0)
                {
                    var label = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Background = Brushes.PaleGoldenrod,
                        FontSize = 8,
                        Width = 16,
                        Height = 10,
                        Content = "荧1",
                        Padding = new Thickness()
                    };
                    label.SetValue(Panel.ZIndexProperty, 6);
                    SCee.AddStateControl(label);
                    this.LightLabels.Add(label);
                }
                else
                {
                    Brush brush;
                    switch (count)
                    {
                        case 1:
                            brush = Brushes.Yellow;
                            break;
                        case 2:
                            brush = Brushes.GreenYellow;
                            break;
                        case 3:
                            brush = Brushes.LimeGreen;
                            break;
                        default:
                            brush = Brushes.DarkGreen;
                            break;
                    }
                    var l = SCee.StatesControls.OfType<Label>().Intersect(this.LightLabels).First();
                    l.Background = brush;
                    l.Content = "荧" + (count + 1);
                }
            };
            AddPadButtonEvent(2);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemy.SetLabelBackground();
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private bool IsInLine(Character SCee, Point point)
        {
            if (point.X == this.X) return (point.Y > this.Y) == (SCee.Y > this.Y);
            return (point.X > this.X) == (SCee.X > this.X);
        }

        private bool SC01IsTargetLegal(Character SCee, Point point)
        {
            var cs = Enemy.Where(c => IsInLine(SCee, point)).ToList();
            if (!cs.Any()) return false;
            var ct = cs.OrderBy(c => c.Distance(this)).First();
            return SCee == ct;
        }
    }
}
