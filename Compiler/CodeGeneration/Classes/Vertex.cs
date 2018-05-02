using System;
namespace Giraph.Classes
{
    public class Vertex : IDisposable
    {
        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Vertex()
        {
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this); 
        }
    }
}
