using System;
using Giraph;
using Giraph.Classes;
namespace Giraph
{
    class Program
    {

        public static void Main(string[] args)
        {

            Graph graph1 = new Graph();

            Vertex _newVertexgraph1;
            Vertex va = new Vertex();
            graph1.Vertices.Add(va);

            Vertex vb = new Vertex();
            graph1.Vertices.Add(vb);

            Vertex vc = new Vertex();
            graph1.Vertices.Add(vc);

            Vertex vd = new Vertex();
            graph1.Vertices.Add(vd);

            Vertex ve = new Vertex();
            graph1.Vertices.Add(ve);

            Edge _newEdgegraph1;
            _newEdgegraph1 = new Edge();
            graph1.Edges.Add(_newEdgegraph1);

            _newEdgegraph1 = new Edge();
            graph1.Edges.Add(_newEdgegraph1);

            _newEdgegraph1 = new Edge();
            graph1.Edges.Add(_newEdgegraph1);

            graph1.Directed = false;


            bool First =
            IsAdjacent(graph1, va, vb);
            ;

            bool Second =
            IsAdjacent(graph1, va, vc);
            ;

            Console.WriteLine(First);

            Console.WriteLine(Second);

        }

        public static bool IsAdjacent(Graph g, Vertex fromVert, Vertex toVert)
        {

            bool CheckConnections(Edge ed, Vertex vert1, Vertex vert2)
            {
                return ed.From == vert1 && ed.To == vert2;
            }

            foreach (Edge ed in g.Edges)
            {
                if ((CheckConnections(edfromVerttoVert)))
                {
                    return true;

                }

                else if ((CheckConnections(edtoVertfromVert)))
                {
                    return true;

                }

                else
                {
                    return false;

                }
            }

        }
    }
}
