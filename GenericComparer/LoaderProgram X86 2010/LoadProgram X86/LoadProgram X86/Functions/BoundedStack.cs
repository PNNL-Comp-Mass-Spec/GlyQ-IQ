using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class BoundedStack<T>
    {
        private readonly int limit;
        private LinkedList<T> list;

        public BoundedStack(int limit)
        {
            this.limit = limit;
            list = new LinkedList<T>();
        }

        public void Push(T item)
        {
            if (list.Count == limit) list.RemoveFirst();
            list.AddLast(item);
        }

        public T Pop()
        {
            if (list.Count == 0)
                throw new IndexOutOfRangeException("No items on the stack");

            var item = list.Last.Value;
            list.RemoveLast();

            return item;
        }

        public int Count()
        {
            return list.Count;
        }
    } 
}
