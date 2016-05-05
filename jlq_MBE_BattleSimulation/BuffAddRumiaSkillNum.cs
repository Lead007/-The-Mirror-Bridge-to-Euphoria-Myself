using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation
{
    class BuffAddRumiaSkillNum : BuffExecuteImmediately
    {
        public BuffAddRumiaSkillNum(Rumia buffer, Game game)
            : base(buffer, buffer, 3*buffer.Interval, "月光：天赋标记数+2", game)
        {
            BuffAffect = (bee, ber) => buffer.SkillNum += 2;
            BuffCancels += (bee, ber) => buffer.SkillNum -= 2;
        }
    }
}
