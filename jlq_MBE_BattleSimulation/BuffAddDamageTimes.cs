using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增加普通次数的buff</summary>
    public class BuffAddDamageTimes : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="damageTimesAdd">增加的普攻次数</param>
        /// <param name="game">游戏对象</param>
        public BuffAddDamageTimes(CharacterMayRepeatedlyDoDamage buffer, int time, int damageTimesAdd, Game game)
            : base(buffer, buffer, time, string.Format("幻影：普攻次数+{0}", damageTimesAdd), true, game)
        {
            BuffAffect += (bee, ber) => buffer.DamageTimes += damageTimesAdd;
            BuffCancels += (bee, ber) => buffer.DamageTimes -= damageTimesAdd;
        }
    }
}
