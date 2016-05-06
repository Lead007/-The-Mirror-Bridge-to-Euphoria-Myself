using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation
{
    class BuffAddDamage : BuffExecuteImmediately
    {
        public BuffAddDamage(Character buffee, Character buffer, int time, float damageGain, Game game)
            : base(buffee, buffer, time, string.Format("虚弱：受伤增加{0}%", (int) (damageGain*100)), game)
        {
            BuffAffect = (bee, ber) =>
            {
                temp = (DBeAttacked) bee.HandleBeAttacked.Clone();
                bee.HandleBeAttacked = (damage, attacker) => temp((int) (damage*(1 + damageGain)), attacker);
            };
            BuffCancels += (bee, ber) => bee.HandleBeAttacked = temp;
        }

        private DBeAttacked temp;
    }
}
