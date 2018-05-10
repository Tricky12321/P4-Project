using System;
namespace Giraph.Classes
{
    public class Edge : IDisposable
    {
        public Vertex From;
        public Vertex To;

        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Edge(Vertex vertexFrom, Vertex vertexTo)
        {
            From = vertexFrom;
            To = vertexTo; 
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
