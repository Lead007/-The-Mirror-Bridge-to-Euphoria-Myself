using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;
using JLQ_GameResources.Characters.SingleCharacter;

namespace JLQ_GameResources.Buffs.BuffAboutCharacter
{
    public class BuffSuikaUncontrolable : BuffExecuteImmediately
    {
        public BuffSuikaUncontrolable(Suika buffer, int time, Game game)
            : base(buffer, buffer, time, "不可控：免疫控制类buff", true, game)
        {
            BuffeeSuika = buffer;
        }

        private Suika BuffeeSuika { get; }

        protected override void BuffAffect()
        {
            BuffeeSuika.SC03IsBuffing = true;
        }

        protected override void Cancel()
        {
            base.Cancel();
            if (BuffeeSuika.BuffList.Any(b => b != this && b is BuffSuikaUncontrolable)) return;
            BuffeeSuika.SC03IsBuffing = false;
        }
    }
}
