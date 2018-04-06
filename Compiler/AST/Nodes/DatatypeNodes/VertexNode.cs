using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class VertexNode : AbstractNode
    {
        public Dictionary<string, string> ValueList = new Dictionary<string, string>();
        public VertexNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
