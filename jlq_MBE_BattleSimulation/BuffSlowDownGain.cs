using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增加行动间隔的buff</summary>
    public class BuffSlowDownGain : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="intervalX">行动间隔增加量，负数则为减少</param>
        /// <param name="game">游戏对象</param>
        public BuffSlowDownGain(Character buffee, Character buffer, int time, double intervalX, Game game)
            : base(buffee, buffer, time,
                intervalX >= 0.0
                    ? string.Format("缓慢：行动间隔+{0}%", (int)(intervalX*100))
                    : string.Format("速度：行动间隔-{0}%", -(int)(intervalX*100)), intervalX < 0, game)
        {
            BuffAffect += (bee, ber) => bee.IntervalX = intervalX;
            BuffCancels += (bee, ber) => bee.IntervalX = intervalX;
        }
    }
}
