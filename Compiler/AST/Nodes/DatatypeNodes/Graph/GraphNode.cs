using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class GraphNode : AbstractNode
    {
        public List<EdgeNode> Edges = new List<EdgeNode>();
        public List<VertexNode> Vertices = new List<VertexNode>();

        public bool Directed = false;

        public GraphNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
