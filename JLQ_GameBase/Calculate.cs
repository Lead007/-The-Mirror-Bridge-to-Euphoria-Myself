using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Data;

namespace JLQ_GameBase
{
    public static class Calculate
    {
        /// <summary>计算命中率</summary>
        /// <param name="relativeHitRate">攻击者对防御者的相对命中率</param>
        /// <param name="distance">攻击者对防御者的相对距离</param>
        /// <returns>命中率</returns>
        private static double HitRate(int relativeHitRate, int distance)
            => 1/(1 + Math.Pow(0.93, relativeHitRate - Math.Max(0, 4*(distance - 2))));

        /// <summary>计算命中率</summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">攻击目标</param>
        /// <returns>命中率</returns>
        public static double HitRate(this Character attacker, Character target)
            => HitRate(attacker.HitRate - target.DodgeRate, attacker.Distance(target));

        //伤害公式
        /// <summary>计算伤害值</summary>
        /// <param name="attack">攻击者的攻击值</param>
        /// <param name="defence">防御者的防御值</param>
        /// <returns>伤害值</returns>
        public static int Damage(int attack, int defence) => attack*attack/(attack + defence);

        #region Distance
        /// <summary>求两点距离，参数可交换</summary>
        /// <param name="point1">点1</param>
        /// <param name="point2">点2</param>
        /// <returns>距离值</returns>
        public static int Distance(this Point point1, Point point2)
            => (int)(Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y));
        /// <summary>求一点和一角色的距离</summary>
        /// <param name="point1">点1</param>
        /// <param name="character1">角色1</param>
        /// <returns></returns>
        public static int Distance(this Point point1, Character character1) => point1.Distance(character1.Position);
        /// <summary>求两角色的距离</summary>
        /// <param name="character1">角色1</param>
        /// <param name="character2">角色2</param>
        /// <returns></returns>
        public static int Distance(this Character character1, Character character2)
            => character1.Position.Distance(character2);
        #endregion

        #region Convert
        /// <summary>将Section转化为中文显示</summary>
        /// <param name="value">当前游戏阶段</param>
        /// <returns>中文显示</returns>
        public static string Convert(Section? value)
        {
            switch (value)
            {
                case Section.Preparing:
                    return "准备阶段";
                case Section.Round:
                    return "行动阶段";
                case Section.End:
                    return "结束阶段";
                default:
                    return "游戏还未开始";
            }
        }

        /// <summary>将阵营转化为中文名</summary>
        /// <param name="group">待转化的阵营</param>
        /// <returns>阵营中文名</returns>
        public static string Convert(Group group)
        {
            switch (group)
            {
                case Group.Friend:
                    return "己方";
                case Group.Enemy:
                    return "敌方";
                default:
                    return "中立方";
            }
        }
        #endregion

        /// <summary>向下取整</summary>
        /// <param name="number">待取整的值</param>
        /// <returns>取整结果</returns>
        public static int Floor(this double number) => (int)Math.Floor(number);

        /// <summary>目标点是否在源点3*3范围内</summary>
        /// <param name="origin">源点</param>
        /// <param name="point">待测点</param>
        /// <returns>是否在范围内</returns>
        public static bool IsIn33(this Point origin, Point point) => origin.IsInSquare(point, 3);

        /// <summary>目标角色是否在源点3*3范围内</summary>
        /// <param name="origin">源点</param>
        /// <param name="c">待测角色</param>
        /// <returns>是否在范围内</returns>
        public static bool IsIn33(this Point origin, Character c) => origin.IsIn33(c.Position);

        /// <summary>目标点是否在以源点为中心的正方形内</summary>
        /// <param name="origin">源点</param>
        /// <param name="point">待测点</param>
        /// <param name="length">正方形边长</param>
        /// <returns>是否在范围内</returns>
        public static bool IsInSquare(this Point origin, Point point, int length)
        {
            var i = (length - 1)/2;
            return Math.Abs(origin.X - point.X) <= i && Math.Abs(origin.Y - point.Y) <= i;
        }
    }
}
