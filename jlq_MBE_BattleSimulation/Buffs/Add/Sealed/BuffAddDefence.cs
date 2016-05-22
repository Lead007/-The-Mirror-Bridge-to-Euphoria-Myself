using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation.Buffs.Add.Sealed
{
    /// <summary>增加防御力的buff</summary>
    public sealed class BuffAddDefence : BuffAddProperty
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceAdd">增加的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddDefence(Character buffee, Character buffer, int time, int defenceAdd, Game game)
            : base(buffee, buffer, time, "Defence", defenceAdd, "坚固：防御", "破碎：防御", game)
        {
            
        }
    }
}
