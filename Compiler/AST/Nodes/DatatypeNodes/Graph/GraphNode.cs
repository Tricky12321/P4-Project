using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class GraphNode : AbstractNode
    {
        public string Name;
        public List<EdgeNode> Edges;
        public List<VertexNode> Vertices;

        public bool Directed = false;

        public GraphNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
