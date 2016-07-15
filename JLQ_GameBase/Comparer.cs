using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>用于比较两个攻受模型目标是否相同</summary>
    public class AttackModelEqualityComparer : IEqualityComparer<AttackModel>
    {
        public bool Equals(AttackModel x, AttackModel y) => x.Target == y.Target;

        public int GetHashCode(AttackModel obj) => obj.Target.ID;
    }
}
