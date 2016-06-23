using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Dialogs.GamePad
{
    public abstract class GamePad_ChooseLines : Dialog_GamePad
    {
        #region Enums
        /// <summary>选择按钮在棋盘网格的方向</summary>
        protected enum Direction
        {
            /// <summary>左</summary>
            Left,
            /// <summary>右</summary>
            Right,
            /// <summary>下</summary>
            Buttom,
            /// <summary>中</summary>
            Top
        }
        #endregion

        public ArrayQueue<int> LinesChoose { get; }
        protected Button[] buttons { get; } = new Button[9];
        protected GamePad_ChooseLines(Direction direction, int lineNum, Game game) : base(game)
        {
            this.LinesChoose = new ArrayQueue<int>(lineNum);
            #region Queue Events
            this.LinesChoose.ItemDequeue += i =>
            {
                buttons[i].Content = string.Empty;
                game.Characters.Where(
                    c => (direction > Direction.Right ? c.Column : c.Row) == i && c != game.CurrentCharacter)
                    .SetLabelBackground(GameColor.LabelDefalutBackground);
            };
            this.LinesChoose.ItemEnqueue += i =>
            {
                buttons[i].Content = tick;
                game.Characters.Where(
                    c => (direction > Direction.Right ? c.Column : c.Row) == i && c != game.CurrentCharacter)
                    .DoAction(this.SetLabelBackground);
            };
            #endregion
            this.GridPad.Loaded +=
                (s, ev) => this.game.EnemyCanAttack.SetLabelBackground(GameColor.LabelDefalutBackground);
            this.Grids[(int) direction].Loaded += (s, ev) =>
            {
                #region buttons
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
                        LinesChoose.Enqueue(j);
                    };
                    #endregion
                }
                #endregion
            };
            #region ButtonSure
            this.ButtonSure.Click += (s, ev) =>
            {
                if (!this.LinesChoose.IsFull)
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
