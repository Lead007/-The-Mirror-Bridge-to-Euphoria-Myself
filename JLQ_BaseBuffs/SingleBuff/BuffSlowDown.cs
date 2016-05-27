using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_BaseBuffs.SingleBuff
{
    /// <summary>增加行动间隔的buff</summary>
    public class BuffSlowDown : BuffExecuteImmediately, IControl
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="intervalAdd">行动间隔增加量，负数则为减少</param>
        /// <param name="game">游戏对象</param>
        public BuffSlowDown(Character buffee, Character buffer, int time, int intervalAdd, Game game)
            : base(buffee, buffer, time,
                intervalAdd >= 0
                    ? string.Format("缓慢：行动间隔+{0}", intervalAdd)
                    : string.Format("速度：行动间隔-{0}", -intervalAdd), intervalAdd < 0, game)
        {
            _intervalAdd = intervalAdd;
        }

        private readonly int _intervalAdd;

        protected override void BuffAffect()
        {
            Buffee.IntervalAdd = _intervalAdd;
        }

        protected override void Cancel()
        {
            Buffee.IntervalAdd = -_intervalAdd;
            base.Cancel();
        }
    }
}
