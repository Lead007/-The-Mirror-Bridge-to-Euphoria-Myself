using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation.Buffs.SingleBuff
{
    /// <summary>每回合治疗一定体力值</summary>
    public class BuffCure : BuffExecuteInSection
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="hp">治疗的体力值</param>
        /// <param name="game">游戏对象</param>
        public BuffCure(Character buffee, Character buffer, int time, int hp, Game game)
            : base(buffee, buffer, time, Section.Preparing, string.Format("治疗：每回合准备阶段治疗{0}点体力", hp), true, game)
        {
            _hp = hp;
        }

        private readonly int _hp;

        protected override void BuffAffect()
        {
            Buffee.Cure(_hp);
        }
    }
}
