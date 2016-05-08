using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>无法移动的buff</summary>
    public class BuffCannotMove : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffCannotMove(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, "冰冻：无法移动", false, game)
        {
            BuffAffect += (bee, ber) =>
            {
                _mA = bee._moveAbilityAdd;
                bee._moveAbilityAdd = -bee.MoveAbility;
            };
            BuffCancels = (bee, ber) => bee._moveAbilityAdd = _mA;
        }

        private int _mA;
    }
}
