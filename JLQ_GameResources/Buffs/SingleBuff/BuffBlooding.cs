using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Characters.CharacterBeCalled;
using JLQ_GameResources.Characters.SingleCharacter;
using MoreEnumerable;

namespace JLQ_GameResources.Buffs.SingleBuff
{
    /// <summary>芙兰符卡中持续流血的buff</summary>
    public class BuffBlooding : BuffBeAttacked
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffBlooding(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, (int) (buffer.Attack*bloodGain), null, game)
        {
            
        }

        private const float bloodGain = 0.2f;
        private const float cureGain = 1.0f/3;

        protected override void BuffAffect()
        {
            base.BuffAffect();
            game.Characters.OfType<Flandre>().DoAction(c => c.Cure(_damage*cureGain));
            game.Characters.OfType<FlandreLittle>().DoAction(c => c.Cure(_damage * cureGain));
        }

        public override string ToString() => base.ToString() + "（可触发芙兰天赋）";
    }
}
