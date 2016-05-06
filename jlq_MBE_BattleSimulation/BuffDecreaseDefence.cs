using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation
{
    class BuffDecreaseDefence : BuffExecuteImmediately
    {
        public BuffDecreaseDefence(Character buffee, Character buffer, int time, int defenceDecrease, Game game)
            : base(buffee, buffer, time, string.Format("破碎：防御降低{0}", defenceDecrease), game)
        {
            BuffAffect = (bee, ber) => bee._defenceAdd -= 20;
            BuffCancels += (bee, ber) => bee._defenceAdd += 20;
        }
    }
}
