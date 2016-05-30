using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_BaseBuffs
{
    public enum BuffKinds
    {
        /// <summary>控制类buff</summary>
        Control
    }
    /// <summary>Buff类型</summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
    public class BuffKindAttribute : Attribute
    {
        public BuffKinds Kind { get; }

        public BuffKindAttribute(BuffKinds kind)
        {
            Kind = kind;
        }
    }
}
