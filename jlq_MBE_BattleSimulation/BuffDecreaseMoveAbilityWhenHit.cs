using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>Cirno的SC02Buff</summary>
    public class BuffDecreaseMoveAbilityWhenHit : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="moveAbilityDecrease">机动减少量</param>
        /// <param name="game">游戏对象</param>
        public BuffDecreaseMoveAbilityWhenHit(Character buffee, Character buffer, int time, int moveAbilityDecrease,
            Game game) : base(buffee, buffer, time, string.Format("吹冰：普攻命中敌人后使敌人一回合内机动-{0}", moveAbilityDecrease), true, game)
        {
            BuffAffect += (bee, ber) =>
            {
                _temp = (DDoAttack) bee.HandleDoingAttack.Clone();
                bee.HandleDoingAttack = (target, times) =>
                {
                    var b = _temp(target, times);
                    var buff = new BuffAddMoveAbility(target, bee, bee.Interval, moveAbilityDecrease, game);
                    buff.BuffTrigger();
                    return b;
                };
            };
            BuffCancels += (bee, ber) => bee.HandleDoingAttack = _temp;
        }

        private DDoAttack _temp;
    }
}
