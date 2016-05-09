using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增加机动的buff</summary>
    public class BuffAddMoveAbility : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="moveAbilityAdd">增加的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddMoveAbility(Character buffee, Character buffer, int time, int moveAbilityAdd, Game game)
            : base(buffee, buffer, time,
                moveAbilityAdd >= 0
                    ? string.Format("灵动：机动增加{0}", moveAbilityAdd)
                    : string.Format("笨拙：机动降低{0}", -moveAbilityAdd), moveAbilityAdd > 0, game)
        {
            BuffAffect += (bee, ber) => bee._moveAbilityAdd += moveAbilityAdd;
            BuffCancels += (bee, ber) => bee._moveAbilityAdd -= moveAbilityAdd;
        }

    }
}
