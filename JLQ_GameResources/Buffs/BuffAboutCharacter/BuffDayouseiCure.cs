using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;
using JLQ_GameResources.Characters.SingleCharacter;

namespace JLQ_GameResources.Buffs.BuffAboutCharacter
{
    public class BuffDayouseiCure : BuffExecuteImmediately
    {
        public BuffDayouseiCure(Character buffee, Daiyousei buffer, int time, Game game)
            : base(buffee, buffer, time, "自然汲取：被指定为大妖精符卡01的额外目标", true, game)
        {
            
        }

        protected override void BuffAffect()
        {
            
        }
    }
}
