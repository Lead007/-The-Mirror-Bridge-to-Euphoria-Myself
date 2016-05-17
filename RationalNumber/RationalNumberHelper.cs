using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Number
{
    internal static class RationalNumberHelper
    {
        /// <summary>约分当前有理数对象</summary>
        public static void FractionReduction(this RationalNumber number)
        {
            if (number.numerator == 0)
            {
                number.denominator = 1;
                number.isPositive = true;
                return;
            }
            uint gcd = number.GCD();
            if (gcd == 1) return;
            number.numerator /= gcd;
            number.denominator /= gcd;
        }

        #region 绝对值，相反数与倒数
        /// <summary>取有理数对象的绝对值</summary>
        /// <param name="number">有理数对象</param>
        public static void Abs(this RationalNumber number)
        {
            number.isPositive = true;
        }

        /// <summary>取有理数对象的相反数</summary>
        /// <param name="number">有理数对象</param>
        public static void Opposite(RationalNumber number)
        {
            number.isPositive = !number.isPositive;
        }

        /// <summary>取有理数对象的倒数</summary>
        /// <param name="number">有理数对象</param>
        public static void Reciprocal(RationalNumber number)
        {
            if (number.numerator == 0) throw new ArgumentException("不能取0的倒数");
            var temp = number.numerator;
            number.numerator = number.denominator;
            number.denominator = temp;
        }
        #endregion

        #region 私有函数
        /// <summary>求有理数对象分子和分母的最大公因数</summary>
        /// <param name="number">有理数对象</param>
        /// <returns>对象分子和分母的最大公因数</returns>
        internal static uint GCD(this RationalNumber number)
        {
            return (number.numerator == 0) ? 1 : Numeral.GCD(number.numerator, number.denominator);
        }
        #endregion
    }
}
