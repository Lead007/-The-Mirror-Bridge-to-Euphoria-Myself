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
    public class GamePad_LyricaSC02 : Dialog_GamePad
    {
        private const string tick = "√";
        public Queue<int> LinesChoose { get; } = new Queue<int>(3); 
        public GamePad_LyricaSC02(Game game) : base(game)
        {
            this.GridLeft.Loaded += GridLeft_Loaded;
        }

        private void GridLeft_Loaded(object sender, RoutedEventArgs e)
        {
            #region buttons
            var buttons = new Button[Game.Row];
            for (var i = 0; i < Game.Row; i++)
            {
                buttons[i] = new Button
                {
                    Background = GameColor.LabelBackground,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    FontSize = 22,
                    Foreground = Brushes.SaddleBrown,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                buttons[i].SetValue(Grid.RowProperty, i);
                GridLeft.Children.Add(buttons[i]);
                #region Click
                buttons[i].Click += (s, ev) =>
                {
                    var j = (int)(s as Button).GetValue(Grid.RowProperty);
                    if (LinesChoose.Contains(j)) return;
                    buttons[j].Content = tick;
                    if (LinesChoose.Count == 3)
                    {
                        var index = LinesChoose.Dequeue();
                        buttons[index].Content = string.Empty;
                        game.Characters.Where(c => c.Y == index && c != game.CurrentCharacter)
                            .Aggregate(GameColor.BaseColor,
                                (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);

                    }
                    LinesChoose.Enqueue(j);
                    foreach (var c in game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter))
                    {
                        if (game.CurrentCharacter.IsFriend(c)) c.LabelDisplay.Background = GameColor.LabelBackground;
                        else if (game.CurrentCharacter.IsEnemy(c))
                            c.LabelDisplay.Background = GameColor.LabelBackground2;
                    }
                };
                #endregion
                #region MouseEnter
                buttons[i].MouseEnter += (s, ev) =>
                {
                    var j = (int)(s as Button).GetValue(Grid.RowProperty);
                    if (LinesChoose.Contains(j)) return;
                    foreach (var c in game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter))
                    {
                        if (game.CurrentCharacter.IsFriend(c)) c.LabelDisplay.Background = GameColor.LabelBackground;
                        else if (game.CurrentCharacter.IsEnemy(c))
                            c.LabelDisplay.Background = GameColor.LabelBackground2;
                    }
                };
                #endregion
                #region MouseLeave
                buttons[i].MouseLeave += (s, ev) =>
                {
                    var j = (int)(s as Button).GetValue(Grid.RowProperty);
                    if (LinesChoose.Contains(j)) return;
                    game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter)
                        .Aggregate(GameColor.BaseColor,
                            (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);
                };
                #endregion
            }
            #endregion
        }
    }
}
