using System;
namespace Giraph.Classes
{
    public class Edge : IDisposable
    {
        public Vertex VertexFrom;
        public Vertex VertexTo;

        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Edge(Vertex vertexFrom, Vertex vertexTo)
        {
            VertexFrom = vertexFrom;
            VertexTo = vertexTo; 
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
