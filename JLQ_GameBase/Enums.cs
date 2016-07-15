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

    /// <summary>
    /// 符卡/技能等级
    /// </summary>
    public enum Level
    {
        /// <summary>一级</summary>
        Easy,
        /// <summary>二级</summary>
        Normal,
        /// <summary>三级</summary>
        Hard,
        /// <summary>四级</summary>
        Lunatic
    }

    /// <summary>相对方向</summary>
    public enum Direction
    {
        /// <summary>上</summary>
        Up,
        /// <summary>右上</summary>
        UpRight,
        /// <summary>右</summary>
        Right,
        /// <summary>右下</summary>
        DownRight,
        /// <summary>下</summary>
        Down,
        /// <summary>左下</summary>
        DownLeft,
        /// <summary>左</summary>
        Left,
        /// <summary>左上</summary>
        UpLeft
    }
}
