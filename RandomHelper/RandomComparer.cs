using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomHelper
{
    /// <summary>比较器，当两元素相等时随机返回大小比较结果</summary>
    /// <typeparam name="T">待比较的类型</typeparam>
    public class RandomComparer<T> : IComparer<T> where T : IComparable<T>
    {
        private Random random { get; }

        /// <summary>构造函数</summary>
        /// <param name="random">随机数对象</param>
        public RandomComparer(Random random)
        {
            this.random = random;
        }

        public int Compare(T x, T y)
        {
            var c = x.CompareTo(y);
            return c != 0 ? c : random.NextCompareResult();
        }
    }
}
