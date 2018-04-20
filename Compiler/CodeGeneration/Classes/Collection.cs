using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Giraph.Classes
{
    public class Collection<T> : List<T>
    {

        public T Pop => this.Take(1).Last();

        public void Push(T Item) => this.Add(Item);

        public T ExtractMin => this.Take(1).Min();

        public T ExtractMax() => this.Take(1).Max();

        public T Dequeue() => this.Take(1).Last();

        public void Enqueue(T Item) => this.Insert(0, Item);
		
        public T Peek() => this.Last();

        public T Select(Func<T, Boolean> p) => this.Where(p).Single();

        public List<T> SelectAll(Func<T, Boolean> p) => this.Where(p).ToList();

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

    }
}
