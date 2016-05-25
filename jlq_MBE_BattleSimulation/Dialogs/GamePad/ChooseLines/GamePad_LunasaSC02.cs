using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChooseLines
{
    public sealed class GamePad_LunasaSC02 : GamePad_ChooseLines
    {
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
                        foreach (var c in game.Characters.Where(c => c.X == j && c != game.CurrentCharacter))
                        {
                            this.SetLabelBackground(c);
                        }
                    };
                    #endregion
                    #region MouseLeave
                    buttons[i].MouseLeave += (s, ev) =>
                    {
                        var j = (int)(s as Button).GetValue(Grid.ColumnProperty);
                        if (LinesChoose.Contains(j)) return;
                        game.Characters.Where(c => c.X == j && c != game.CurrentCharacter)
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
