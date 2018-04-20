using System;
using Giraph;
using Giraph.Classes;
namespace Giraph
{
    class Program
    {

        static void Main(string[] args)
        {

            Graph g1 = new Graph();

            Vertex _newVertexg1;
            Vertex va = new Vertex();
            g1.Vertices.Add(va);

            Vertex vb = new Vertex();
            g1.Vertices.Add(vb);

            Vertex vc = new Vertex();
            g1.Vertices.Add(vc);

            Vertex vd = new Vertex();
            g1.Vertices.Add(vd);

            Vertex ve = new Vertex();
            g1.Vertices.Add(ve);

            Edge _newEdgeg1;
            Edge x = new Edge(vb, vd);
            x.v = 4;
            g1.Edges.Add(x);

            Edge y = new Edge(vd, ve);
            y.v = 7;
            g1.Edges.Add(y);

            Edge z = new Edge(va, vc);
            z.v = 10;
            g1.Edges.Add(z);

            Graph.Directed = False;


            Graph g2 = new Graph();

            Vertex _newVertexg2;
            _newVertexg2 = new Vertex();
            g2.Vertices.Add(_newVertexg2);

            _newVertexg2 = new Vertex();
            g2.Vertices.Add(_newVertexg2);

            _newVertexg2 = new Vertex();
            g2.Vertices.Add(_newVertexg2);

            _newVertexg2 = new Vertex();
            g2.Vertices.Add(_newVertexg2);

            _newVertexg2 = new Vertex();
            g2.Vertices.Add(_newVertexg2);

            Edge _newEdgeg2;
            Graph.Directed = False;

            bool h = true;
            int asdf = 5;
            if (h == (h))
            {
            }
            for (int i = 0;
         i < 1000; i += 1)
            {
                Console.WriteLine((i));

            }
            Test("test", 1);
        }
        public static void Test(string test, int blaah)
        {
            if (test == "test")
            {
                Console.WriteLine(("DU HAR RAMT TEST I DIN IF"));

            }
            else if (blaah == 2)
            {
                Console.WriteLine(("DU HAR RAMT BLAAH I DIN ELSEIF"));

            }
        }
    }
}
