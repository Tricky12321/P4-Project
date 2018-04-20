using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class GraphDeclVertexNode : AbstractNode
    {
        public Dictionary<string, AbstractNode> ValueList = new Dictionary<string, AbstractNode>();
        public GraphDeclVertexNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
