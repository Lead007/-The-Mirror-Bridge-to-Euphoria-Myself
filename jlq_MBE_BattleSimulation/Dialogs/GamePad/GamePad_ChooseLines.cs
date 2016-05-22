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
    public abstract class GamePad_ChooseLines : Dialog_GamePad
    {
        protected int LineNum { get; }
        public Queue<int> LinesChoose { get; } = new Queue<int>();
        protected GamePad_ChooseLines(Direction direction, int lineNum, Game game) : base(game)
        {
            this.LineNum = lineNum;
            this.GridPad.Loaded += (s, ev) => game.EnemyCanAttack.Aggregate(GameColor.BaseColor,
                (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);
            this.Grids[(int) direction].Loaded += (s, ev) =>
            {
                #region buttons
                var buttons = new Button[9];
                for (var i = 0; i < 9; i++)
                {
                    buttons[i] = new Button
                    {
                        Background = GameColor.LabelBackground,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2),
                        FontSize = 22,
                        Foreground = GameColor.DialogButtonBrush,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    buttons[i].SetValue(direction > Direction.Right ? Grid.ColumnProperty : Grid.RowProperty, i);
                    Grids[(int) direction].Children.Add(buttons[i]);
                    #region Click
                    buttons[i].Click += (sender, args) =>
                    {
                        var j = (int)(sender as Button).GetValue(direction > Direction.Right
                            ? Grid.ColumnProperty
                            : Grid.RowProperty);
                        if (LinesChoose.Contains(j)) return;
                        buttons[j].Content = tick;
                        if (LinesChoose.Count == LineNum)
                        {
                            var index = LinesChoose.Dequeue();
                            buttons[index].Content = string.Empty;
                            game.Characters.Where(
                                c => (direction > Direction.Right ? c.X : c.Y) == index && c != game.CurrentCharacter)
                                .Aggregate(GameColor.BaseColor,
                                    (cu, c) => c.LabelDisplay.Background = GameColor.LabelDefalutBackground);
                        }
                        LinesChoose.Enqueue(j);
                        foreach (var c in
                            game.Characters.Where(
                                c => (direction > Direction.Right ? c.X : c.Y) == j && c != game.CurrentCharacter))
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
                if (this.LinesChoose.Count != LineNum)
                {
                    Game.IllegalMessageBox("选择行数不够！");
                    return;
                }
                this.DialogResult = true;
                this.Close();
            };
            #endregion
        }

        protected abstract void SetLabelBackground(Character c);
    }
}
