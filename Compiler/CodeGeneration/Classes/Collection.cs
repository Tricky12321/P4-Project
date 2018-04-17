using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Compiler.CodeGeneration.Classes
{
    public class Collection<T>
    {
        public List<T> _storage = new List<T>();

        public T Pop => _storage.Take(1).Last();

        public void Push(T Item) => _storage.Add(Item);

        public T ExtractMin => _storage.Take(1).Min();

        public T ExtractMax() => _storage.Take(1).Max();

        public T Dequeue() => _storage.Take(1).Last();

        public void Enqueue() => _storage.Take(1).ElementAt(0);

        public T Select(Func<T, Boolean> p) => _storage.Where(p).Single();

        public List<T> SelectAll(Func<T, Boolean> p) => _storage.Where(p).ToList();
    }
}
