using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatroomServer
{
    public class MyQueue<T> : IEnumerable<T>
    {
        private Queue<T> _queue;

        public int Count => this._queue.Count;

        public int MaxCapacity { get; private set; }

        public MyQueue(int maxCapacity)
        {
            this.MaxCapacity = maxCapacity;
            this._queue = new Queue<T>(maxCapacity);
        }

        public void Enqueue(T item)
        {
            this._queue.Enqueue(item);

            if (this._queue.Count > this.MaxCapacity)
            {
                T removed = this.Dequeue();
                //Console.WriteLine("Delete " + removed.ToString());
            }
        }

        public void Clear() => this._queue.Clear();

        public T Dequeue() => this._queue.Dequeue();

        public T Peek() => this._queue.Peek();

        public IEnumerator<T> GetEnumerator() => this._queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._queue.GetEnumerator();
    }
}
