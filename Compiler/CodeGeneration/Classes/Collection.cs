using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Compiler.CodeGeneration.Classes
{
    public class Collection<T>
    {
        private List<T> _storage = new List<T>();

        public T Pop => _storage.Take(1).Last();

        public void Push(T Item) => _storage.Add(Item);

        public T ExtractMin => _storage.Take(1).Min();

        public T ExtractMax() => _storage.Take(1).Max();

        public T Dequeue() => _storage.Take(1).Last();

        public void Enqueue(T Item) => _storage.Insert(0, Item);
		
        public T Peek() => _storage.Last();

        public T Select(Func<T, Boolean> p) => _storage.Where(p).Single();

        public List<T> SelectAll(Func<T, Boolean> p) => _storage.Where(p).ToList();

        public List<T> Remove(Func<T, Boolean> p)
        {
            // Select all the items
            var output = _storage.Where(p).ToList();
            // Now remove them form the list
            foreach (var item in output)
            {
                _storage.Remove(item);
            }
            return output;
        }
    }
}
