using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    public abstract class GameBase
    {
        /// <summary>当前回合所在的阶段</summary>
        public virtual Section? GameSection { get; protected set; }

        /// <summary>加人模式的当前ID</summary>
        public virtual int ID { get; set; } = 1;
    }
}
