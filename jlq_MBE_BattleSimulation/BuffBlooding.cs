using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>持续流血的buff</summary>
    public class BuffBlooding : BuffExecuteInSection
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffBlooding(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, Section.Preparing, "流血：准备阶段失去5点体力", false, game)
        {
            BuffAffect += (bee, ber) =>
            {
                bee.HandleBeAttacked(bloodNum, null);
            };
        }

        private const int bloodNum = 5;
    }
}
