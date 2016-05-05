﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    class BuffSlowDown : BuffExecuteImmediately 
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">buff持续时间</param>
        /// <param name="add">行动间隔添加量</param>
        /// <param name="game">游戏对象</param>
        public BuffSlowDown(Character buffee, Character buffer, int time, int add, Game game)
            : base(buffee, buffer, time, string.Format("缓慢：行动间隔+{0}", add), game)
        {
            BuffAffect = (bee, ber) => bee._intervalAdd += 10;
            BuffCancels += (bee, ber) => bee._intervalAdd -= 10;
        }
    }
}
