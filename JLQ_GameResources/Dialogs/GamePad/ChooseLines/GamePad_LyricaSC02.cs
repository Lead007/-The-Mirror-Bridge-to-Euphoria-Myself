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

namespace JLQ_GameResources.Dialogs.GamePad.ChooseLines
{
    /// <summary>莉莉卡·普莉兹姆利巴 的符卡02的对话框</summary>
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
                        game.Characters.Where(c => c.Y == j && c != game.CurrentCharacter)
                            .DoAction(this.SetLabelBackground);
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
