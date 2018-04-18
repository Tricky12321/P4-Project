using System;
namespace Giraph.Classes
{
    public class Graph
    {
        public Collection<Vertex> Vertices = new Collection<Vertex>();
        public Collection<Edge> Edges = new Collection<Edge>();
        public bool Directed;

        public Graph()
        {
            
        }
    }
}
