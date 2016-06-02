using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Dialogs.GamePad.ChoosePoints
{
    public sealed class GamePad_RanSC01 : GamePad_ChoosePoints
    {
        /// <summary>八云蓝 的符卡01的对话框</summary>
        /// <param name="game"></param>
        public GamePad_RanSC01(Game game) : base(12, game)
        {
            
        }

        protected override bool IsLegalClick(Point point)
        {
            return base.IsLegalClick(point) && game[point] == null;
        }

        protected override IEnumerable<Character> LegalCharacters(Point point)
        {
            return game.CurrentCharacter.Enemies.Where(c => point.Distance(c) == 1);
        }

        protected override void SetLabelBackground(Character c)
        {
            c.SetLabelBackground();
        }
    }
}
