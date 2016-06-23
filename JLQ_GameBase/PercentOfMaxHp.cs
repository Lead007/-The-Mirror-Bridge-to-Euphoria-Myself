using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>总体力上限的占比，回血时用</summary>
    public struct PercentOfMaxHp
    {
        /// <summary>总体力上限的占比</summary>
        public float Percent { get; set; }

        public PercentOfMaxHp(float percent)
        {
            this.Percent = percent;
        }
    }
}
