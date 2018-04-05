using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class BoolComparisonNode : AbstractNode
    {
        // If there is a left and right
        public AbstractNode Left;
        public string ComparisonOperator;
        public AbstractNode Right;
        // If there is a ref, to the next node
        public string Prefix;
        public string Suffix;


        public bool InsideParentheses;
        public BoolComparisonNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
