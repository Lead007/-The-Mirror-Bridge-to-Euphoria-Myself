using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows;
using Data;

namespace JLQ_MBE_BattleSimulation
{
    static class Calculate
    {
        /// <summary>计算命中率</summary>
        /// <param name="relativeHitRate">攻击者对防御者的相对命中率</param>
        /// <param name="distance">攻击者对防御者的相对距离</param>
        /// <returns>命中率</returns>
        private static double HitRate(int relativeHitRate, int distance)
        {
            var p = 1.0 / (1 + Math.Pow(0.93, relativeHitRate));
            return (p > 0.95 ? 0.95 : (p < 0.05 ? 0.05 : p)) * (1.0f - 0.05f * distance);
        }

        /// <summary>计算命中率</summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">攻击目标</param>
        /// <returns>命中率</returns>
        public static double HitRate(Character attacker, Character target)
        {
            return HitRate(attacker.HitRate - target.DodgeRate, Distance(attacker, target));
        }

        //伤害公式
        /// <summary>计算伤害值</summary>
        /// <param name="attack">攻击者的攻击值</param>
        /// <param name="defence">防御者的防御值</param>
        /// <returns>伤害值</returns>
        public static int Damage(int attack, int defence)
        {
            return attack * attack / (attack + defence);
        }

        /// <summary>储存角色列表中所有角色的原始数据</summary>
        public static List<CharacterData> CharacterDataList = new List<CharacterData>();

        /// <summary>求两点距离，参数可交换</summary>
        /// <param name="point1">点1</param>
        /// <param name="point2">点2</param>
        /// <returns>距离值</returns>
        public static int Distance(Point point1, Point point2)
        {
            return (int) (Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y));
        }
        /// <summary>求一点和一角色的距离</summary>
        /// <param name="point1">点1</param>
        /// <param name="character1">角色1</param>
        /// <returns></returns>
        public static int Distance(Point point1, Character character1)
        {
            return Distance(point1, character1.Position);
        }
        /// <summary>求两角色的距离</summary>
        /// <param name="character1">角色1</param>
        /// <param name="character2">角色2</param>
        /// <returns></returns>
        public static int Distance(Character character1, Character character2)
        {
            return Distance(character1.Position, character2);
        }

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

        /// <summary>向下取整</summary>
        /// <param name="number">待取整的值</param>
        /// <returns>取整结果</returns>
        public static int Floor(double number)
        {
            return (int) Math.Floor(number);
        }

        /// <summary>目标点是否在源点3*3范围内</summary>
        /// <param name="origin">源点</param>
        /// <param name="point">待测点</param>
        /// <returns>是否在范围内</returns>
        public static bool IsIn33(Point origin, Point point)
        {
            return Math.Abs(origin.X - point.X) <= 1 && Math.Abs(origin.Y - point.Y) <= 1;
        }
        
    }
}
