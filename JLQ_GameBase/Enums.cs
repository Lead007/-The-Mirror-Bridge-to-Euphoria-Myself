using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>阵营</summary>
    public enum Group
    {
        /// <summary>中立</summary>
        Middle,
        /// <summary>友军</summary>
        Friend,
        /// <summary>敌军</summary>
        Enemy = -1
    }

    /// <summary>游戏阶段</summary>
    public enum Section
    {
        /// <summary>准备阶段</summary>
        Preparing,
        /// <summary>行动阶段</summary>
        Round,
        /// <summary>结束阶段</summary>
        End
    }
}
