using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Number
{
    /// <summary>有理数类</summary>
    public class RationalNumber:IComparable<double>,IComparable<RationalNumber>,ICloneable
    {
        #region 属性
        /// <summary>分子</summary>
        public uint numerator { get; internal set; }
        /// <summary>分母</summary>
        public uint denominator { get; internal set; }
        /// <summary>符号</summary>
        public bool isPositive { get; internal set; }
        #endregion

        #region 构造函数

        /// <summary>构造函数</summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <param name="isPositive">是否为正数</param>
        /// <param name="doFractionReduction">是否约分</param>
        public RationalNumber(uint numerator, uint denominator, bool isPositive, bool doFractionReduction = true)
        {
            if (denominator == 0)
            {
                throw new ArgumentOutOfRangeException("分母不能为0.");
            }
            this.numerator = numerator;
            this.denominator = denominator;
            this.isPositive = isPositive;
            if (doFractionReduction)
            {
                this.FractionReduction();
            }
        }

        /// <summary>构造函数</summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <param name="doFractionReduction">是否约分</param>
        public RationalNumber(int numerator, int denominator, bool doFractionReduction = true)
        {
            if (denominator == 0)
            {
                throw new ArgumentOutOfRangeException("分母不能为0");
            }
            this.numerator = (uint)Math.Abs(numerator);
            this.denominator = (uint)Math.Abs(denominator);
            this.isPositive = !((numerator < 0) ^ (denominator < 0));
            if (doFractionReduction)
            {
                this.FractionReduction();
            }
        }

        /// <summary>默认构造函数，构造一个值为0的对象</summary>
        public RationalNumber()
        {
            this.numerator = 0;
            this.denominator = 1;
            this.isPositive = true;
        }

        /// <summary>构造函数</summary>
        /// <param name="value">有理数的值</param>
        /// <param name="doFractionReduction">是否约分</param>
        public RationalNumber(double value, bool doFractionReduction = true)
        {
            double temp = value;
            this.denominator = 1;
            while (temp % 1 != 0)
            {
                temp *= 10;
                this.denominator *= 10;
            }
            this.isPositive = temp >= 0;
            this.numerator = (uint)Math.Abs(temp);
            if (doFractionReduction)
            {
                this.FractionReduction();
            }
        }
        #endregion

        /// <summary>有理数的值</summary>
        public double Value => this.Operator*(double) this.numerator/(double) this.denominator;
       
        #region 绝对值，相反数与倒数
        /// <summary>返回值等于当前对象绝对值的新对象</summary>
        public RationalNumber Abs => new RationalNumber(this.numerator, this.denominator, true, false);
        /// <summary>返回值等于number对象相反数的新对象</summary>
        public RationalNumber Opposite => new RationalNumber(this.numerator, this.denominator, !this.isPositive, false);
        /// <summary>返回值等于number对象倒数的新对象</summary>
        public RationalNumber Reciprocal
        {
            get
            {
                if (this.numerator == 0) throw new ArgumentException("不能取0的倒数。");
                return new RationalNumber(this.denominator, this.numerator, this.isPositive, false);
            }
        }
        #endregion

        //当前对象是否等于0
        public bool IsZero => this.numerator == 0;

        #region 重写Object类的方法
        public override bool Equals(object obj)
        {
            var number = obj as RationalNumber;
            if (number != null)
            {
                return this == number;
            }
            return this == (double)obj;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
            =>
                (((!isPositive) && (this.numerator != 0)) ? "-" : "") + this.numerator.ToString() +
                ((this.numerator == 0 || this.denominator == 1) ? "" : ("/" + this.denominator.ToString()));
        #endregion

        #region 运算符重载
        #region 等于与不等于
        #region 等于
        public static bool operator==(RationalNumber num1, RationalNumber num2)
        {
            if (num1 == null && num2 == null) return true;
            if (num1 == null || num2 == null) return false;
            return num1.Value == num2.Value;
        }

        public static bool operator==(RationalNumber num1,double num2)
        {
            return (num1?.Value ?? null) == num2;
        }

        public static bool operator==(double num1,RationalNumber num2)
        {
            return num1 == (num2?.Value ?? null);
        }
        #endregion
        #region 不等于
        public static bool operator!=(RationalNumber num1, RationalNumber num2)
        {
            return !(num1 == num2);
        }

        public static bool operator !=(RationalNumber num1, double num2)
        {
            return !(num1 == num2);
        }

        public static bool operator !=(double num1, RationalNumber num2)
        {
            return !(num1 == num2);
        }
        #endregion
        #endregion

        #region 大于与小于
        #region 大于
        public static bool operator>(RationalNumber num1,RationalNumber num2)
        {
            return num1.Value > num2.Value;
        }

        public static bool operator>(RationalNumber num1,double num2)
        {
            return num1.Value > num2;
        }

        public static bool operator>(double num1,RationalNumber num2)
        {
            return num1 > num2.Value;
        }
        #endregion
        #region 小于
        public static bool operator<(RationalNumber num1,RationalNumber num2)
        {
            return num1.Value < num2.Value;
        }

        public static bool operator<(RationalNumber num1,double num2)
        {
            return num1.Value < num2;
        }

        public static bool operator<(double num1,RationalNumber num2)
        {
            return num1 < num2.Value;
        }
        #endregion
        #endregion

        #region 加与减
        #region 加
        public static RationalNumber operator+ (RationalNumber num1,RationalNumber num2)
        {
            var lcm = Numeral.LCM(num1.denominator, num2.denominator);
            var numeratorNew = (int)(num1.Operator * num1.numerator * lcm / num1.denominator + num2.Operator * num2.numerator * lcm / num2.denominator);
            return new RationalNumber(numeratorNew, (int)lcm);
        }

        public static RationalNumber operator+(RationalNumber num1,double num2)
        {
            return num1 + new RationalNumber(num2);
        }

        public static RationalNumber operator+(double num1,RationalNumber num2)
        {
            return new RationalNumber(num1) + num2;
        }
        #endregion
        #region 减
        public static RationalNumber operator-(RationalNumber num1,RationalNumber num2)
        {
            var lcm = Numeral.LCM(num1.denominator, num2.denominator);
            var numeratorNew = (int)(num1.Operator * num1.numerator * lcm / num1.denominator - num2.Operator * num2.numerator * lcm / num2.denominator);
            return new RationalNumber(numeratorNew, (int)lcm);
        }

        public static RationalNumber operator-(RationalNumber num1,double num2)
        {
            return num1 - new RationalNumber(num2);
        }

        public static RationalNumber operator-(double num1,RationalNumber num2)
        {
            return new RationalNumber(num1) - num2;
        }
        #endregion
        #endregion

        #region 乘与除
        #region 乘
        public static RationalNumber operator*(RationalNumber num1,RationalNumber num2)
        {
            return new RationalNumber(num1.numerator * num2.numerator, num1.denominator * num2.denominator, !(num1.isPositive ^ num2.isPositive));
        }

        public static RationalNumber operator*(RationalNumber num1,double num2)
        {
            return num1 * new RationalNumber(num2);
        }

        public static RationalNumber operator*(double num1,RationalNumber num2)
        {
            return new RationalNumber(num1) * num2;
        }
        #endregion
        #region 除
        public static RationalNumber operator/(RationalNumber num1,RationalNumber num2)
        {
            return new RationalNumber(num1.numerator * num2.denominator, num1.denominator * num2.numerator, !(num1.isPositive ^ num2.isPositive));
        }

        public static RationalNumber operator/(RationalNumber num1,double num2)
        {
            return num1 / new RationalNumber(num2);
        }

        public static RationalNumber operator/(double num1,RationalNumber num2)
        {
            return new RationalNumber(num1) / num2;
        }
        #endregion
        #endregion
        #endregion

        #region 实现IComparable<T>接口的CompareTo方法
        public int CompareTo(double other)
        {
            return this.Value.CompareTo(other);
        }
        public int CompareTo(RationalNumber number)
        {
            if (this > number)
            {
                return 1;
            }
            else if (this == number)
            {
                return 0;
            }
            else return -1;
        }
        #endregion

        #region 实现ICloneable接口的Clone方法
        public object Clone() => new RationalNumber(this.numerator, this.denominator, this.isPositive, false);
        #endregion

        #region 私有函数
        /// <summary>求当前对象的符号系数</summary>
        private int Operator => isPositive ? 1 : (-1);
        #endregion
    }
}
