using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_MBE_BattleSimulation.Characters.SingleCharacter;

namespace JLQ_MBE_BattleSimulation.Buffs.SingleBuff
{
    public class BuffSuikaUncontrolable : BuffExecuteImmediately
    {
        public BuffSuikaUncontrolable(Suika buffer, int time, Game game)
            : base(buffer, buffer, time, "不可控：免疫控制类buff", true, game)
        {
            
        }

        private Suika BuffeeSuika => Buffee as Suika;

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
