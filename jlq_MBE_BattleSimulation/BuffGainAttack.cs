using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益攻击值的buff</summary>
    public sealed class BuffGainAttack : BuffGainProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackGain">增益的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffGainAttack(Character buffee, Character buffer, int time, double attackGain, Game game)
            : base(buffee, buffer, time, "Attack", attackGain, "锋利：攻击", "钝化：攻击", game)
        {
            
        }
    }
}
