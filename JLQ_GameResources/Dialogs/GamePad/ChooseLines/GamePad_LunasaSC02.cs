using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Dialogs.GamePad.ChooseLines
{
    public sealed class GamePad_LunasaSC02 : GamePad_ChooseLines
    {
        /// <summary>露娜萨·普莉兹姆利巴 的符卡02的对话框</summary>
        /// <param name="game"></param>
        public GamePad_LunasaSC02(Game game) : base(Direction.Top, 2, game)
        {
            this.GridTop.Loaded += (sender, e) =>
            {
                #region buttons
                for (var i = 0; i < Game.Column; i++)
                {
                    #region MouseEnter
                    buttons[i].MouseEnter += (s, ev) =>
                    {
                        var j = (int)(s as Button).GetValue(Grid.ColumnProperty);
                        if (LinesChoose.Contains(j)) return;
                        game.Characters.Where(c => c.Column == j && c != game.CurrentCharacter)
                            .DoAction(this.SetLabelBackground);
                    };
                    #endregion
                    #region MouseLeave
                    buttons[i].MouseLeave += (s, ev) =>
                    {
                        var j = (int)(s as Button).GetValue(Grid.ColumnProperty);
                        if (LinesChoose.Contains(j)) return;
                        game.Characters.Where(c => c.Column == j && c != game.CurrentCharacter)
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
