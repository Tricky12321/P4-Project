using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class GraphNode : AbstractNode
    {
        public List<GraphDeclEdgeNode> Edges = new List<GraphDeclEdgeNode>();
        public List<GraphDeclVertexNode> Vertices = new List<GraphDeclVertexNode>();

        public bool Directed = false;

        public GraphNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
