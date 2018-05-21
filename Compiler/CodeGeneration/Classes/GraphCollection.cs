using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Giraph.Classes
{
    public class GraphCollection<T> : Collection<T>
    {

        public new T Select(Func<T, Boolean> p) => this.Where(p).Single();

        public new List<T> SelectAll(Func<T, Boolean> p) => this.Where(p).ToList();

        public new void RemoveItem(T obj)
        {
            this.Remove(obj);
            if (obj is IDisposable)
            {
                (obj as IDisposable).Dispose();
            }
        }

        public new void RemoveAll()
        {
            foreach (var item in this)
            {
                this.Remove(item);
                if (item is IDisposable)
                {
                    (item as IDisposable).Dispose();
                }
            }
        }



    }
}
