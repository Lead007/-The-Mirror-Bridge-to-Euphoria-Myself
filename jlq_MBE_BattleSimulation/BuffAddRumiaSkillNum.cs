using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Localizer;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>针对Rumia的buff，增加其天赋标记数</summary>
    public class BuffAddRumiaSkillNum : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffer">buff发出者</param>
        /// <param name="game">游戏对象</param>
        public BuffAddRumiaSkillNum(Rumia buffer, Game game)
            : base(buffer, buffer, buffer.BuffTime, "月光：天赋标记数+2", true, game)
        {
            BuffAffect += (bee, ber) => buffer.SkillNum += 2;
            BuffCancels += (bee, ber) => buffer.SkillNum -= 2;
        }
    }
}
