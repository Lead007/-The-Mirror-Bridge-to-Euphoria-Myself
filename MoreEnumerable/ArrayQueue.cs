using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreEnumerable
{
    /// <summary>限制容量大小的队列，当队列内元素数量等于容量上限时继续入队则会使队首元素出队</summary>
    /// <typeparam name="T">队列内元素的类型</typeparam>
    public class ArrayQueue<T> : Queue<T>
    {
        /// <summary>队列的最大容量</summary>
        public int Capacity { get; }

        /// <summary>构造函数</summary>
        /// <param name="capacity">队列的最大容量，需大于0</param>
        public ArrayQueue(int capacity)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException("最大容量需大于0。");
            Capacity = capacity;
        }

        /// <summary>构造函数</summary>
        /// <param name="items">用于初始化队列的枚举集合，不能为空或null</param>
        public ArrayQueue(IEnumerable<T> items) : base(items)
        {
            var l = items.ToList();
            if (!l.Any()) throw new ArgumentException("初始枚举集合不能为空。");
            Capacity = l.Count();
        }

        /// <summary>使队首元素出队，同时可对此元素执行设定的方法</summary>
        /// <returns>队首元素</returns>
        public new T Dequeue()
        {
            var item = base.Dequeue();
            ItemDequeue?.Invoke(item);
            return item;
        }

        /// <summary>将元素入队，若超出队列最大容量则使队首元素出队</summary>
        /// <param name="item">入队的元素</param>
        public new void Enqueue(T item)
        {
            if (this.Count == this.Capacity)
            {
                this.Dequeue();
            }
            base.Enqueue(item);
            ItemEnqueue?.Invoke(item);
        }

        /// <summary>元素出队后触发的事件</summary>
        public event Action<T> ItemDequeue;
        /// <summary>元素入队后触发的事件</summary>
        public event Action<T> ItemEnqueue;
    }
}
