using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreEnumerable
{
    public static class EnumerableHelper
    {
        /// <summary>对枚举集合所有元素执行指定的操作</summary>
        /// <typeparam name="T">枚举集合的类型</typeparam>
        /// <param name="items">枚举集合</param>
        /// <param name="action">执行的操作</param>
        public static void DoAction<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}
