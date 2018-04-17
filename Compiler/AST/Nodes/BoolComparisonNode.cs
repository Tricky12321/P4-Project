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
        public AllType LeftType;
        public AllType RightType;
        public List<AllType> TypeList = new List<AllType>();
        public bool InsideParentheses;
        public BoolComparisonNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }

        public void AddType(AllType type) {
             TypeList.Add(type);
        }



    }
}
