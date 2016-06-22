using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>用于比较时间计算下个行动角色的结构</summary>
    internal struct CharacterTimeComparable : IComparable<CharacterTimeComparable>
    {
        internal Character Character { get; }

        private Random random { get; }

        public CharacterTimeComparable(Character c, Random random)
        {
            this.Character = c;
            this.random = random;
        }

        /// <summary>显式实现IComparable接口</summary>
        /// <param name="other">另一个结构</param>
        /// <returns>比较结果</returns>
        int IComparable<CharacterTimeComparable>.CompareTo(CharacterTimeComparable other)
        {
            var cx = this.Character.CurrentTime.CompareTo(other.Character.CurrentTime);
            if (cx != 0) return cx;
            var ix = this.Character.Interval.CompareTo(other.Character.Interval);
            if (ix != 0) return -ix;
            return (random.Next(2) << 1) - 1;
        }
    }
}
