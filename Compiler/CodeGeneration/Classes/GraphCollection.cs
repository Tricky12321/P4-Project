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
