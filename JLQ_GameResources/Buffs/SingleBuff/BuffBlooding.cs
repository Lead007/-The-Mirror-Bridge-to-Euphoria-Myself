using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;
using JLQ_GameResources.Characters.SingleCharacter;
using MoreEnumerable;

namespace JLQ_GameResources.Buffs.SingleBuff
{
    /// <summary>持续流血的buff</summary>
    public class BuffBlooding : BuffExecuteInSection
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffBlooding(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, Section.Preparing, string.Format("流血：准备阶段失去{0}点体力", bloodNum), false, game)
        {

        }

        private const int bloodNum = 10;

        protected override void BuffAffect()
        {
            Buffee.HandleBeAttacked(bloodNum, null);
            game.Characters.OfType<Flandre>().DoAction(c => c.Cure(bloodNum));
        }
    }
}
