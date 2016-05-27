using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JLQ_GameBase;

namespace JLQ_GameResources.Dialogs.GamePad.ChooseLines
{
    public sealed class GamePad_LyricaSC02 : GamePad_ChooseLines
    {
        public GamePad_LyricaSC02(Game game) : base(Direction.Left, 3, game)
        {
            this.GridLeft.Loaded += (sender, e) =>
            {
                #region buttons
                for (var i = 0; i < Game.Row; i++)
                {
                    #region MouseEnter
                    buttons[i].MouseEnter += (s, ev) =>
                    {
                        var j = (int) (s as Button).GetValue(Grid.RowProperty);
                        if (LinesChoose.Contains(j)) return;
                        foreach (var c in game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter))
                        {
                            this.SetLabelBackground(c);
                        }
                    };
                    #endregion
                    #region MouseLeave
                    buttons[i].MouseLeave += (s, ev) =>
                    {
                        var j = (int) (s as Button).GetValue(Grid.RowProperty);
                        if (LinesChoose.Contains(j)) return;
                        game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter)
                            .SetLabelBackground(GameColor.LabelDefalutBackground);
                    };
                    #endregion
                }
                #endregion
            };
        }

        protected override void SetLabelBackground(Character c)
        {
            if (game.CurrentCharacter.IsFriend(c))
                c.SetLabelBackground();
            else if (game.CurrentCharacter.IsEnemy(c))
                c.SetLabelBackground(GameColor.LabelBackground2);
        }
    }
}
