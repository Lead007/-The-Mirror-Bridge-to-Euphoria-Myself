using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;
using JLQ_GameResources.Characters.SingleCharacter;

namespace JLQ_GameResources.Dialogs.GamePad.ChoosePoints
{
    /// <summary>爱丽丝·玛格特洛依德 的符卡02的对话框</summary>
    public sealed class GamePad_AliceSC02 : GamePad_ChoosePoints
    {
        public GamePad_AliceSC02(Alice sender, Game game) : base(1, game)
        {
            Sender = sender;
        }

        private Alice Sender { get; }

        protected override bool IsLegalClick(Point point)
            => base.IsLegalClick(point) && point.IsInRange(Sender, 4) && game[point] == null;

        protected override IEnumerable<Character> LegalCharacters(Point point) => new List<Character>();

        protected override void SetLabelBackground(Character c)
        {
            
        }
    }
}
