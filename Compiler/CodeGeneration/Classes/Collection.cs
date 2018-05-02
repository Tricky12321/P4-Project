using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Giraph.Classes
{
    public class GraphCollection<T> : List<T>
    {

        public T Select(Func<T, Boolean> p) => this.Where(p).Single();

        public List<T> SelectAll(Func<T, Boolean> p) => this.Where(p).ToList();

        public void RemoveItem(T obj) {
            this.Remove(obj);
            if (obj is IDisposable) {
                (obj as IDisposable).Dispose();
            }
        }

        public void RemoveAll() {
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
