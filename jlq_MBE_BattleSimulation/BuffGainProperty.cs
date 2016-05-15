using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>增益角色属性的Buff抽象类</summary>
    public abstract class BuffGainProperty : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="propertyName">属性名（首字母大写）</param>
        /// <param name="gain">增益</param>
        /// <param name="positiveName">正面效果名</param>
        /// <param name="negativeName">负面效果名</param>
        /// <param name="game">游戏对象</param>
        protected BuffGainProperty(Character buffee, Character buffer, int time, string propertyName, double gain,
            string positiveName, string negativeName, Game game)
            : base(buffee, buffer, time,
                string.Format((gain > 0 ? positiveName + "+" : negativeName + "-") + "{0}%", (int) (Math.Abs(gain)*100)),
                gain > 0, game)
        {
            var p = typeof(Character).GetProperty(propertyName + "X").SetMethod;
            BuffAffect += (bee, ber) => p.Invoke(bee, new object[] { gain });
            BuffCancels += (bee, ber) => p.Invoke(bee, new object[] { -gain });
        }

    }
}
