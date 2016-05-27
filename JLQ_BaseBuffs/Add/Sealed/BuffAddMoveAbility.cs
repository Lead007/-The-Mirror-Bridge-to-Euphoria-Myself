using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_BaseBuffs.Add.Sealed
{
    /// <summary>增加机动的buff</summary>
    public class BuffAddMoveAbility : BuffAddProperty, IControl
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="moveAbilityAdd">增加的机动值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        public BuffAddMoveAbility(Character buffee, Character buffer, int time, int moveAbilityAdd, Game game)
            : base(buffee, buffer, time, "MoveAbility", moveAbilityAdd, "灵动：机动", "笨拙：机动", game)
        {

        }

    }
}
