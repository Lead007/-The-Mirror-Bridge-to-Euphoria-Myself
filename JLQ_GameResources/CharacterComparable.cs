using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using RandomHelper;

namespace JLQ_GameResources
{
    /// <summary>Dayousei技能中比较体力百分比的对象</summary>
    public class CharacterHpPercentComparable : CharacterComparableBase, IComparable<CharacterHpPercentComparable>
    {
        private Random random { get; }

        public CharacterHpPercentComparable(Character c, Random random) : base(c)
        {
            this.random = random;
        }

        int IComparable<CharacterHpPercentComparable>.CompareTo(CharacterHpPercentComparable other)
        {
            var c = HpPercent(this.Character).CompareTo(HpPercent(other.Character));
            return c != 0 ? c : random.NextCompareResult();
        }

        private float HpPercent(Character c) => ((float)c.Hp) / c.MaxHp;
    }
}
