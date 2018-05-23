using System;
namespace Giraph.Classes
{
    public class Vertex
    {
		//EXTENSIONS HERE
		//*****EXTEND*****
		//EXTENSIONS ENDED

        public Vertex()
        {
            
        }
        
		public bool disposed = false;

		private void Update()
        {
            
        }

		public Vertex Get() {
			if (disposed) {
				Console.WriteLine("You are trying to reference am object which no longer exists");
				Environment.Exit(0);
				return null;
			} else {
				return this;
			}
		}
    }
}
