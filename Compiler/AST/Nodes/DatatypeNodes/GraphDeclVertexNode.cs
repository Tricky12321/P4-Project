using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class GraphDeclVertexNode : AbstractNode
    {
        public Dictionary<string, string> ValueList = new Dictionary<string, string>();
        public GraphDeclVertexNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
