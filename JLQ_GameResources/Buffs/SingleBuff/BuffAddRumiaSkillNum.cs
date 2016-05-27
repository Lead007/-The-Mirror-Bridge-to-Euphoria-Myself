using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;
using JLQ_GameResources.Characters.SingleCharacter;

namespace JLQ_GameResources.Buffs.SingleBuff
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
            BuffeeTurn = buffer;
        }

        private Rumia BuffeeTurn { get; }

        protected override void BuffAffect()
        {
            BuffeeTurn.SkillNum += 2;
        }

        protected override void Cancel()
        {
            BuffeeTurn.SkillNum -= 2;
            base.Cancel();
        }
    }
}
