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
    /// <summary>增加角色属性的Buff子类，含有返回操作各角色属性的Buff对象的静态函数</summary>
    public sealed class BuffAddProperty : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="propertyName">属性名（首字母大写）</param>
        /// <param name="add">增量</param>
        /// <param name="positiveName">正面效果名（未格式化）</param>
        /// <param name="negativeName">负面效果名（未格式化）</param>
        /// <param name="game">游戏对象</param>
        private BuffAddProperty(Character buffee, Character buffer, int time, string propertyName, int add,
            string positiveName, string negativeName, Game game)
            : base(buffee, buffer, time,
                string.Format((add > 0 ? positiveName + "+" : negativeName + "-") + "{0}", Math.Abs(add)), add > 0, game)
        {
            _setProperty = typeof (Character).GetProperty(propertyName + "Add").SetMethod;
            _add = add;
        }

        private readonly int _add;
        private readonly MethodInfo _setProperty;

        protected override void BuffAffect()
        {
            _setProperty.Invoke(Buffee, new object[] {_add});
        }

        protected override void Cancel()
        {
            _setProperty.Invoke(Buffee, new object[] {-_add});
            base.Cancel();
        }

        /// <summary>增减攻击的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackAdd">增加的攻击值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddAttack(Character buffee, Character buffer, int time, int attackAdd,
            Game game) => new BuffAddProperty(buffee, buffer, time, "Attack", attackAdd, "锋利：攻击", "钝化：攻击", game);

        /// <summary>增减防御的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="defenceAdd">增加的防御值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddDefence(Character buffee, Character buffer, int time, int defenceAdd,
            Game game) => new BuffAddProperty(buffee, buffer, time, "Defence", defenceAdd, "坚固：防御", "破碎：防御", game);

        /// <summary>增减命中率的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="hitRateAdd">增加的命中率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddHitRate(Character buffee, Character buffer, int time, int hitRateAdd,
            Game game) => new BuffAddProperty(buffee, buffer, time, "HitRate", hitRateAdd, "精准：命中率", "眼花：命中率", game);

        /// <summary>增减闪避率的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="dodgeRateAdd">增加的闪避率值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddDodgeRate(Character buffee, Character buffer, int time, int dodgeRateAdd,
            Game game) => new BuffAddProperty(buffee, buffer, time, "DodgeRate", dodgeRateAdd, "幻影：闪避率", "迟钝：闪避率", game);

        /// <summary>增减近战补正的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="closeAmendmentAdd">增加的近战补正值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddCloseAmemdment(Character buffee, Character buffer, int time,
            int closeAmendmentAdd, Game game) =>
                new BuffAddProperty(buffee, buffer, time, "CloseAmendment", closeAmendmentAdd, "近战：近战补正", "异斥：近战补正",
                    game);

        /// <summary>增减机动的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="moveAbilityAdd">增加的机动值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddMoveAbility(Character buffee, Character buffer, int time,
            int moveAbilityAdd, Game game) =>
                new BuffAddProperty(buffee, buffer, time, "MoveAbility", moveAbilityAdd, "灵动：机动", "笨拙：机动", game);

        /// <summary>增减攻击范围的Buff对象</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="attackRangeAdd">增加的攻击范围值，负数则为降低</param>
        /// <param name="game">游戏对象</param>
        /// <returns>生成的对象</returns>
        public static BuffAddProperty BuffAddAttackRange(Character buffee, Character buffer, int time,
            int attackRangeAdd, Game game) =>
                new BuffAddProperty(buffee, buffer, time, "AttackRange", attackRangeAdd, "远程：攻击范围", "近程：攻击范围", game);

    }
}
