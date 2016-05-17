using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Number
{
    /// <summary>和数论有关的一些静态方法</summary>
    public static class Numeral
    {
        #region 求最大公因数
        /// <summary>求两数的最大公因数</summary>
        /// <param name="x1">第一个数</param>
        /// <param name="x2">第二个数</param>
        /// <returns>两数的最大公因数</returns>
        public static uint GCD(uint x1, uint x2)
        {
            while (true)
            {
                var r = x1 % x2;
                if (r == 0)
                {
                    return x2;
                }
                else
                {
                    x1 = x2;
                    x2 = r;
                }
            }
        }

        /// <summary>求两数的最大公因数</summary>
        /// <param name="x1">第一个数</param>
        /// <param name="x2">第二个数</param>
        /// <returns>两数的最大公因数</returns>
        public static int GCD(int x1, int x2)
        {
            while (true)
            {
                var r = x1 % x2;
                if (r == 0)
                {
                    return x2;
                }
                else
                {
                    x1 = x2;
                    x2 = r;
                }
            }
        }
        #endregion

        #region 求最小公倍数
        /// <summary>求两数的最小公倍数</summary>
        /// <param name="x1">第一个数</param>
        /// <param name="x2">第二个数</param>
        /// <returns>两数的最小公倍数</returns>
        public static uint LCM(uint x1, uint x2)
        {
            return x1 * x2 / GCD(x1, x2);
        }

        /// <summary>求两数的最小公倍数</summary>
        /// <param name="x1">第一个数</param>
        /// <param name="x2">第二个数</param>
        /// <returns>两数的最小公倍数</returns>
        public static int LCM(int x1, int x2)
        {
            return x1 * x2 / GCD(x1, x2);
        }
        #endregion
    }
}
