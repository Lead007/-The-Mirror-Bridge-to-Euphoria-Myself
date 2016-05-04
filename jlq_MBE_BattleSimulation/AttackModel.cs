using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>攻击模型</summary>
    public struct AttackModel
    {
        /// <summary>攻击者</summary>
        public Character Attacker;
        /// <summary>攻击目标</summary>
        public Character Target;

        /// <summary>构造函数</summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">攻击目标</param>
        public AttackModel(Character attacker, Character target)
        {
            Attacker = attacker;
            Target = target;
        }
    }
}
