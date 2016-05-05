using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    class BuffCannotMove : BuffExecuteImmediately
    {
        public BuffCannotMove(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, "冰冻：无法移动", game)
        {
            BuffAffect = (bee, ber) =>
            {
                mA = bee._moveAbilityX;
                bee._moveAbilityX = -bee.MoveAbility;
            };
            BuffCancels = (bee, ber) => bee._moveAbilityX = mA;
        }

        private int mA;
    }
}
