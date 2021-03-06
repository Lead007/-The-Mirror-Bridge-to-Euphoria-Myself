﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JLQ_GameBase;

namespace JLQ_GameResources.Dialogs.GamePad.ChoosePoints
{
    /// <summary>梅露兰·普莉兹姆利巴 的符卡02的对话框</summary>
    public sealed class GamePad_MerlinSC02 : GamePad_ChoosePoints
    {
        public GamePad_MerlinSC02(Game game) : base(2, game)
        {

        }

        protected override IEnumerable<Character> LegalCharacters(PadPoint point)
        {
            return game.Characters.Where(c => point.Distance(c) <= 2);
        }

        protected override void SetLabelBackground(Character c)
        {
            if (game.CurrentCharacter.IsFriend(c))
                c.SetLabelBackground();
            else if (game.CurrentCharacter.IsEnemy(c))
                c.LabelDisplay.Background = GameColor.LabelBackground2;
        }
    }
}
