using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>在每回合特定阶段执行的buff</summary>
    public abstract class BuffExecuteInSection : Buff
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="executeSection">buff执行的阶段</param>
        /// <param name="name">buff名称</param>
        /// <param name="isPositive">是否为正面buff</param>
        /// <param name="game">游戏对象</param>
        protected BuffExecuteInSection(Character buffee, Character buffer, int time, Section executeSection, string name,
            bool isPositive, Game game) : base(buffee, buffer, time, name, isPositive, game)
        {
            this.ExecuteSection = executeSection;
        }

        /// <summary>buff执行的阶段</summary>
        public readonly Section ExecuteSection;

    }
}
