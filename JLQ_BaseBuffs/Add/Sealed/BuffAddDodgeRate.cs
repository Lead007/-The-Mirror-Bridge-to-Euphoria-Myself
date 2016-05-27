using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_BaseBuffs.Add.Sealed
{
    /// <summary>增加闪避率的buff</summary>
    public sealed class BuffAddDodgeRate : BuffAddProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="dodgeRateAdd">增加的闪避率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddDodgeRate(Character buffee, Character buffer, int time, int dodgeRateAdd, Game game)
            : base(buffee, buffer, time, "DodgeRate", dodgeRateAdd, "幻影：闪避率", "迟钝：闪避率", game)
        {

        }
    }
}
