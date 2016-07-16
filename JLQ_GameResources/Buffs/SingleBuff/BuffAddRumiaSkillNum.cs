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
        /// <param name="num">增加量</param>
        /// <param name="game">游戏对象</param>
        public BuffAddRumiaSkillNum(Rumia buffer, int num, Game game)
            : base(buffer, buffer, buffer.BuffTime, string.Format("月光：天赋标记数+{0}", num), true, game)
        {
            BuffeeTurn = buffer;
        }

        private Rumia BuffeeTurn { get; }

        private int Num { get; }

        protected override void BuffAffect()
        {
            BuffeeTurn.SkillNum += Num;
        }

        protected override void Cancel()
        {
            BuffeeTurn.SkillNum -= Num;
            base.Cancel();
        }
    }
}
