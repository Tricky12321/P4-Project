using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class GraphDeclEdgeNode : AbstractNode
    {
        public string VertexNameFrom;
        public string VertexNameTo;

        public Dictionary<string, AbstractNode> ValueList = new Dictionary<string, AbstractNode>();

        public GraphDeclEdgeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
