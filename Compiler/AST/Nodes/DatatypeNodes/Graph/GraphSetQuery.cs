using System;
using System.Collections.Generic;

namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class GraphSetQuery : AbstractNode
    {
        public Tuple<VariableAttributeNode, string, ExpressionNode> Attributes;
        public GraphSetQuery(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
