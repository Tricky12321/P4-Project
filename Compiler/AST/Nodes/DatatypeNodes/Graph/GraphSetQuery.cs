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

        public string ToExpressionString()
        {
            string returnString = string.Empty;
            foreach (var expressionPart in Attributes.Item3.ExpressionParts)
            {
                returnString += (expressionPart as ConstantNode).Value;
            }
            return returnString;
        }
    }
}
