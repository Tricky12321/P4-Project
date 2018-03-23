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
        public AbstractNode NextNode;

        public bool NextNodeBool = false;

        public BoolComparisonNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
