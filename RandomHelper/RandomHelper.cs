using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomHelper
{
    public static class RandomHelper
    {
        /// <summary>获取枚举集合中的随机一个元素</summary>
        /// <typeparam name="T">枚举集合的元素类型</typeparam>
        /// <param name="random">随机数对象</param>
        /// <param name="list">待操作的枚举集合</param>
        /// <returns>随机元素</returns>
        public static T RandomElement<T>(this Random random, IEnumerable<T> list)
        {
            var l = list.ToList();
            var index = random.Next(l.Count);
            return l[index];
        }

        /// <summary>获取枚举集合中的随机若干个元素</summary>
        /// <typeparam name="T">枚举集合的元素类型</typeparam>
        /// <param name="random">随机数对象</param>
        /// <param name="count">获取的元素数量</param>
        /// <param name="list">待操作的枚举集合</param>
        /// <returns>获取的若干个元素</returns>
        public static IEnumerable<T> RandomElements<T>(this Random random, int count, IEnumerable<T> list)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("选取长度不能为负数");
            }
            var result = new List<T>();
            var l = list.ToList();
            if (count > l.Count)
            {
                throw new ArgumentOutOfRangeException("选取长度超过了枚举集合的长度极限");
            }
            for (var i = 0; i < count; i++)
            {
                var c = l.Count;
                var index = random.Next(c);
                result.Add(l[index]);
                l.RemoveAt(index);
            }
            return result;
        }
    }
}
