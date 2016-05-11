using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>使攻击者获得流血buff的buff</summary>
    public class BuffLetBloodingWhenBeAttacked : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="bloodingTime">流血时间</param>
        /// <param name="game">游戏对象</param>
        public BuffLetBloodingWhenBeAttacked(Character buffee, Character buffer, int time, int bloodingTime, Game game)
            : base(buffee, buffer, time, "禁弹：对其攻击者进入流血状态", true, game)
        {
            BuffAffect += (bee, ber) =>
            {
                _temp = (DBeAttacked) bee.HandleBeAttacked.Clone();
                bee.HandleBeAttacked = (damage, attacker) =>
                {
                    _temp(damage, attacker);
                    var buff = new BuffBlooding(attacker, bee, bloodingTime, game);
                };
            };
            BuffCancels += (bee, ber) => bee.HandleBeAttacked = _temp;
        }

        private DBeAttacked _temp;
    }
}
