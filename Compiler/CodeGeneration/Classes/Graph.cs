using System;
namespace Giraph.Classes
{
    public class Graph
    {
        public GraphCollection<Vertex> Vertices = new GraphCollection<Vertex>();
        public GraphCollection<Edge> Edges = new GraphCollection<Edge>();
        public bool Directed = false;

        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Graph()
        {
            
        }
    }
}
