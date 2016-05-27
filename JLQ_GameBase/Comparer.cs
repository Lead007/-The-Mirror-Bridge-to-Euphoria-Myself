using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>整数比较器，当相等时随机</summary>
    public class IntRandomComparer : IComparer<int>
    {
        private readonly Random random;
        /// <summary>构造函数</summary>
        /// <param name="random">随机数对象</param>
        public IntRandomComparer(Random random)
        {
            this.random = random;
        }
        /// <summary>实现IComparer接口</summary>
        /// <param name="x">第一个整数</param>
        /// <param name="y">第二个整数</param>
        /// <returns>比较结果</returns>
        public int Compare(int x, int y)
        {
            if (x != y) return x.CompareTo(y);
            return random.Next(2) == 1 ? 1 : -1;
        }
    }
}
