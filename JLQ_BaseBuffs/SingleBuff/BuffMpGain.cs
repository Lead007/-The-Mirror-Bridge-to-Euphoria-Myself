using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_BaseBuffs.SingleBuff
{
    /// <summary>每回合恢复一定灵力值</summary>
    public class BuffMpGain : BuffExecuteInSection
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="mp">恢复的灵力值</param>
        /// <param name="game">游戏对象</param>
        public BuffMpGain(Character buffee, Character buffer, int time, int mp, Game game)
            : base(buffee, buffer, time, Section.Preparing, string.Format("回蓝：每回合准备阶段恢复{0}点灵力", mp), true, game)
        {
            _mp = mp;
        }

        private readonly int _mp;

        protected override void BuffAffect()
        {
            Buffee.MpGain(_mp);
        }
    }
}
