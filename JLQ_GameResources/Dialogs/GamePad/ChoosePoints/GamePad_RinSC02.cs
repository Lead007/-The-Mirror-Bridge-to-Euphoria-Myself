using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Characters.SingleCharacter;

namespace JLQ_GameResources.Dialogs.GamePad.ChoosePoints
{
    public sealed class GamePad_RinSC02 : GamePad_ChoosePoints
    {
        public GamePad_RinSC02(Rin sender, Game game) : base(1, game)
        {
            Sender = sender;
        }
        private Rin Sender { get; }

        protected override bool IsLegalClick(PadPoint point) => game[point] == null;

        protected override IEnumerable<Character> LegalCharacters(PadPoint point) => new List<Character>();

        protected override void SetLabelBackground(Character c)
        {

        }
    }
}
