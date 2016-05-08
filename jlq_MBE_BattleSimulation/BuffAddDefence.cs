using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增加防御力的buff</summary>
    public class BuffAddDefence : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceAdd">增加的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddDefence(Character buffee, Character buffer, int time, int defenceAdd, Game game)
            : base(buffee, buffer, time,
                defenceAdd >= 0 ? string.Format("坚固：防御增加{0}", defenceAdd) : string.Format("破碎：防御降低{0}", -defenceAdd),
                defenceAdd > 0, game)
        {
            BuffAffect += (bee, ber) => bee._defenceAdd += defenceAdd;
            BuffCancels += (bee, ber) => bee._defenceAdd -= defenceAdd;
        }
    }
}
