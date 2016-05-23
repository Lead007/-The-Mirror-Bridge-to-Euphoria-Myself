using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation.Buffs.Add.Sealed
{
    /// <summary>增加攻击力的buff</summary>
    public sealed class BuffAddAttack : BuffAddProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackAdd">增加的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddAttack(Character buffee, Character buffer, int time, int attackAdd, Game game)
            : base(buffee, buffer, time, "Attack", attackAdd, "锋利：攻击", "钝化：攻击", game)
        {

        }
    }
}
