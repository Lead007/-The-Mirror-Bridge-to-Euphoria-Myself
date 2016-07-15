using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_BaseBuffs
{
    /// <summary>增益角色属性的Buff子类，含有返回操作各角色属性的Buff对象的静态函数</summary>
    public sealed class BuffGainProperty : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="propertyName">属性名（首字母大写）</param>
        /// <param name="gain">增益</param>
        /// <param name="positiveName">正面效果名</param>
        /// <param name="negativeName">负面效果名</param>
        /// <param name="game">游戏对象</param>
        private BuffGainProperty(Character buffee, Character buffer, int time, string propertyName, double gain,
            string positiveName, string negativeName, Game game)
            : base(buffee, buffer, time,
                string.Format((gain > 0 ? positiveName + "+" : negativeName + "-") + "{0}%", (int) (Math.Abs(gain)*100)),
                gain > 0, game)
        {
            _setProperty = typeof(Character).GetProperty(propertyName + "X").SetMethod;
            _gain = gain;
        }

        private readonly double _gain;
        private readonly MethodInfo _setProperty;

        protected override void BuffAffect()
        {
            _setProperty.Invoke(Buffee, new object[] { _gain });
        }

        protected override void Cancel()
        {
            _setProperty.Invoke(Buffee, new object[] { -_gain });
            base.Cancel();
        }

        /// <summary>增益攻击的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackGain">增益的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainAttack(Character buffee, Character buffer, int time, float attackGain,
            Game game) => new BuffGainProperty(buffee, buffer, time, "Attack", attackGain, "锋利：攻击", "钝化：攻击", game);

        /// <summary>增益防御的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceGain">增益的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainDefence(Character buffee, Character buffer, int time, float defenceGain,
            Game game) => new BuffGainProperty(buffee, buffer, time, "Defence", defenceGain, "坚固：防御", "破碎：防御", game);

        /// <summary>增益命中率的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="hitRateGain">增益的命中率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainHitRate(Character buffee, Character buffer, int time, float hitRateGain,
            Game game) => new BuffGainProperty(buffee, buffer, time, "HitRate", hitRateGain, "精准：命中率", "眼花：命中率", game);

        /// <summary>增益闪避率的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="dodgeRateGain">增益的闪避率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainDodgeRate(Character buffee, Character buffer, int time, float dodgeRateGain,
            Game game) => new BuffGainProperty(buffee, buffer, time, "DodgeRate", dodgeRateGain, "幻影：闪避率", "迟钝：闪避率", game);

        /// <summary>增益近战补正的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="closeAmendmentGain">增益的近战补正值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainCloseAmemdment(Character buffee, Character buffer, int time,
            float closeAmendmentGain, Game game) =>
                new BuffGainProperty(buffee, buffer, time, "CloseAmendment", closeAmendmentGain, "近战：近战补正", "异斥：近战补正",
                    game);

        /// <summary>增益机动的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="moveAbilityGain">增益的机动值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainMoveAbility(Character buffee, Character buffer, int time,
            float moveAbilityGain, Game game) =>
                new BuffGainProperty(buffee, buffer, time, "MoveAbility", moveAbilityGain, "灵动：机动", "笨拙：机动", game);

        /// <summary>增益攻击范围的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackRangeGain">增益的攻击范围值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffGainProperty BuffGainAttackRange(Character buffee, Character buffer, int time,
            float attackRangeGain, Game game) =>
                new BuffGainProperty(buffee, buffer, time, "AttackRange", attackRangeGain, "远程：攻击范围", "近程：攻击范围", game);

    }
}
