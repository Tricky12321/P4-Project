using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Giraph.Classes
{
    public class Collection<T> : List<T>
    {

        public T Pop => this.RemoveLast();

        public void Push(T Item) => this.Insert(this.Count, Item);

        public T ExtractMin => RemoveMin();

        public T ExtractMax() => RemoveMax();

        public T Dequeue => this.RemoveLast();

        public void Enqueue(T Item) => this.Insert(0, Item);
              
        public T Select(Func<T, Boolean> p) => this.Where(p).Single();

        public List<T> SelectAll(Func<T, Boolean> p) => this.Where(p).ToList();

        /*
        public List<T> Remove(Func<T, Boolean> p)
        {
            // Select all the items
            var output = this.Where(p).ToList();
            // Now remove them form the list
            foreach (var item in output)
            {
                this.Remove(item);
            }
            return output;
        }
        */

		public T First() {
			return this[0];
		}

        private T RemoveLast()
        {
            T LastElement = this.Last();
            this.RemoveAt(this.Count - 1);
            return LastElement;
        }

        private T RemoveFirst()
        {
            T FirstElement = this.First();
            this.RemoveAt(0);
            return FirstElement;
        }

        private T RemoveMin()
        {
            T min = this.Min();
            this.Remove(min);
            return min;
        }

        private T RemoveMax()
        {
            T max = this.Max();
            this.Remove(max);
            return max;
        }

		public new bool Remove(T obj)
        {
            if (typeof(T) == typeof(Vertex))
            {
                (obj as Vertex).disposed = true;

            }
            else if (typeof(T) == typeof(Edge))
            {
                (obj as Edge).disposed = true;
            }
            return base.Remove(obj);
        }

        public new void RemoveAll()
        {
            foreach (var item in this.ToList())
            {
                Remove(item);
            }
        }



    }
}
