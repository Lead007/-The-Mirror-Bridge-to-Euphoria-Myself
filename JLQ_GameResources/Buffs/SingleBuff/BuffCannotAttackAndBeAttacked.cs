using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_GameResources.Buffs.SingleBuff
{
    /// <summary>不能攻击也不能被攻击的buff</summary>
    public sealed class BuffCannotAttackAndBeAttacked : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffCannotAttackAndBeAttacked(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, "隐身：不可攻击也不受伤害", null, game)
        {
            
        }

        private DDoAttack _temp1;
        private DBeAttacked _temp2;

        protected override void BuffAffect()
        {
            _temp1 = Buffee.HandleDoingAttack.Clone() as DDoAttack;
            Buffee.HandleDoingAttack = (target, times) => false;
            _temp2 = Buffee.HandleBeAttacked.Clone() as DBeAttacked;
            Buffee.HandleBeAttacked = (damage, attacker) => { };
        }

        protected override void Cancel()
        {
            base.Cancel();
            Buffee.HandleDoingAttack = _temp1;
            Buffee.HandleBeAttacked = _temp2;
        }
    }
}
