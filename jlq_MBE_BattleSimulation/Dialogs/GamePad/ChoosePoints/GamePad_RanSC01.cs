using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Dialogs.GamePad.ChoosePoints
{
    public sealed class GamePad_RanSC01 : GamePad_ChoosePoints
    {
        public GamePad_RanSC01(Game game) : base(12, game)
        {
            
        }

        protected override bool IsLegalClick(Point point)
        {
            return base.IsLegalClick(point) && game[point] == null;
        }

        protected override IEnumerable<Character> LegalCharacters(Point point)
        {
            return game.CurrentCharacter.Enemy.Where(c => point.Distance(c) == 1);
        }

        protected override void SetLabelBackground(Character c)
        {
            c.SetLabelBackground();
        }
    }
}
