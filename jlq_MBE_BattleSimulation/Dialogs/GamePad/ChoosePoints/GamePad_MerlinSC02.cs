using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChoosePoints
{
    public sealed class GamePad_MerlinSC02 : GamePad_ChoosePoints
    {
        public GamePad_MerlinSC02(Game game) : base(2, game)
        {
            foreach (var b in GridPad.Children.OfType<Button>())
            {

            }
        }

        protected override IEnumerable<Character> LegalCharacters(Point point)
        {
            return game.Characters.Where(c => Calculate.Distance(point, c) <= 2);
        }

        protected override void SetLabelBackground(Character c)
        {
            if (game.CurrentCharacter.IsFriend(c))
                c.LabelDisplay.Background = GameColor.LabelBackground;
            else if (game.CurrentCharacter.IsEnemy(c))
                c.LabelDisplay.Background = GameColor.LabelBackground2;
        }
    }
}
