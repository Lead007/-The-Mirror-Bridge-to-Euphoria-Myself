using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MoreEnumerable;

namespace JLQ_MBE_BattleSimulation.Dialogs.GamePad
{
    public abstract class GamePad_ChoosePoints : Dialog_GamePad
    {
        public ArrayQueue<Point> PointsChoose { get; } 
        protected GamePad_ChoosePoints(int pointNum, Game game) : base(game)
        {
            PointsChoose = new ArrayQueue<Point>(pointNum);
            #region Queue Events
            PointsChoose.ItemDequeue += p =>
            {
                var border = GetBorder(p);
                border.BorderBrush = GameColor.PadBrush;
                border.BorderThickness = new Thickness(1);
                LegalCharacters(p).Where(c => c != game.CurrentCharacter)
                    .SetLabelBackground(GameColor.LabelDefalutBackground);
            };
            PointsChoose.ItemEnqueue += p =>
            {
                var borderP = GetBorder(p);
                borderP.BorderThickness = new Thickness(3);
                borderP.BorderBrush = Brushes.Red;
                foreach (var c in LegalCharacters(p).Where(c => c != game.CurrentCharacter))
                {
                    SetLabelBackground(c);
                }
            };
            #endregion
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
                            PointsChoose.Enqueue(point);
                        };
                        #endregion
                        #region MouseEnter
                        buttons[i, j].MouseEnter += (sender, e) =>
                        {
                            var mousePoint = GetMousePoint(sender as Button);
                            if (PointsChoose.Contains(mousePoint)) return;
                            foreach (var c in LegalCharacters(mousePoint).Where(c => c != game.CurrentCharacter))
                            {
                                this.SetLabelBackground(c);
                            }
                        };
                        #endregion
                        #region MouseLeave
                        buttons[i, j].MouseLeave += (sender, e) =>
                        {
                            var p = GetMousePoint(sender as Button);
                            if (PointsChoose.Contains(p)) return;
                            LegalCharacters(p).Where(c => c != game.CurrentCharacter)
                                .SetLabelBackground(GameColor.LabelDefalutBackground);
                            foreach (var c in PointsChoose.SelectMany(
                                point => LegalCharacters(point).Where(c => c != game.CurrentCharacter)))
                            {
                                this.SetLabelBackground(c);
                            }
                        };
                        #endregion
                    }
                #endregion
            };
            #region ButtonSure
            this.ButtonSure.Click += (s, ev) =>
            {
                if (this.PointsChoose.Count != pointNum)
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
                b => (int)b.GetValue(Grid.ColumnProperty) == p.X && (int)b.GetValue(Grid.RowProperty) == p.Y);
        }

        protected Point GetMousePoint(Button sender)
        {
            return new Point((int)sender.GetValue(Grid.ColumnProperty), (int)sender.GetValue(Grid.RowProperty));
        }

        protected abstract IEnumerable<Character> LegalCharacters(Point point);
        protected abstract void SetLabelBackground(Character c);
    }
}
