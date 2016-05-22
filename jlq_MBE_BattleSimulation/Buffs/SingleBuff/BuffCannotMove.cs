using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.SingleBuff
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

        }

        protected override void BuffAffect()
        {
            Buffee.MoveAbilityAdd = -100;
        }

        protected override void Cancel()
        {
            Buffee.MoveAbilityAdd = 100;
            base.Cancel();
        }
    }
}
