using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation.Dialogs.GamePad
{
    public abstract class GamePad_ChoosePoints : Dialog_GamePad
    {
        protected int PointNum { get; }
        protected Point MousePoint { get; set; }
        public Queue<Point> PointsChoose { get; } = new Queue<Point>(); 
        protected GamePad_ChoosePoints(int pointNum, Game game) : base(game)
        {
            PointNum = pointNum;
            GridPad.Loaded += (s, ev) =>
            {
                #region buttons
                var buttons = new Button[Game.Column, Game.Row];
                for (var i = 0; i < Game.Column; i++)
                    for (var j = 0; j < Game.Row; j++)
                    {
                        buttons[i, j] = new Button
                        {
                            Background = GameColor.LabelBackground,
                            FontSize = 22,
                            Foreground = GameColor.DialogButtonBrush,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Opacity = 0
                        };
                        var b = buttons[i, j];
                        b.SetValue(Grid.ColumnProperty, i);
                        b.SetValue(Grid.RowProperty, j);
                        b.SetValue(Panel.ZIndexProperty, 4);
                        GridPad.Children.Add(b);
                        #region Click
                        b.Click += (sender, e) =>
                        {
                            var button = sender as Button;
                            var column = (int)button.GetValue(Grid.ColumnProperty);
                            var row = (int)button.GetValue(Grid.RowProperty);
                            var point = new Point(column, row);
                            if (PointsChoose.Contains(point)) return;
                            var borderP = GetBorder(point);
                            borderP.BorderThickness = new Thickness(3);
                            borderP.BorderBrush = Brushes.Red;
                            if (PointsChoose.Count == PointNum)
                            {
                                var p = PointsChoose.Dequeue();
                                var border = GetBorder(p);
                                border.BorderBrush = GameColor.PadBrush;
                                border.BorderThickness = new Thickness(1);
                                LegalCharacters(p)
                                    .Where(c => c != game.CurrentCharacter)
                                    .Aggregate(GameColor.BaseColor,
                                        (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);
                            }
                            PointsChoose.Enqueue(point);
                            foreach (var c in LegalCharacters(point).Where(c => c != game.CurrentCharacter))
                            {
                                SetLabelBackground(c);
                            }
                        };
                        #endregion
                        #region MouseEnter
                        buttons[i, j].MouseEnter += (sender, e) =>
                        {
                            var button = sender as Button;
                            var column = (int) button.GetValue(Grid.ColumnProperty);
                            var row = (int)button.GetValue(Grid.RowProperty);
                            MousePoint = new Point(column, row);
                            if (PointsChoose.Contains(MousePoint)) return;
                            foreach (var c in LegalCharacters(MousePoint).Where(c => c != game.CurrentCharacter))
                            {
                                SetLabelBackground(c);
                            }
                        };
                        #endregion
                        #region MouseLeave
                        buttons[i, j].MouseLeave += (sender, e) =>
                        {
                            var p = MousePoint;
                            if (PointsChoose.Contains(p)) return;
                            LegalCharacters(p)
                                .Where(c => c != game.CurrentCharacter)
                                .Aggregate(GameColor.BaseColor,
                                    (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);
                            MousePoint = Game.DefaultPoint;
                            foreach (var c in PointsChoose.SelectMany(point => LegalCharacters(point).Where(c => c != game.CurrentCharacter)))
                            {
                                SetLabelBackground(c);
                            }
                        };
                        #endregion
                    }
                #endregion
            };
            #region ButtonSure
            this.ButtonSure.Click += (s, ev) =>
            {
                if (this.PointsChoose.Count != PointNum)
                {
                    Game.IllegalMessageBox("选择点数不够！");
                    return;
                }
                this.DialogResult = true;
                this.Close();
            };
            #endregion
        }

        protected Border GetBorder(Point p)
        {
            return GridPad.Children.OfType<Border>().FirstOrDefault(
                b => (int) b.GetValue(Grid.ColumnProperty) == p.X && (int) b.GetValue(Grid.RowProperty) == p.Y);
        }

        protected abstract IEnumerable<Character> LegalCharacters(Point point);
        protected abstract void SetLabelBackground(Character c);
    }
}
