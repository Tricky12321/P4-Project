using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class IfElseIfElseNode : AbstractNode
    {
        public AbstractNode IfCondition;
        public List<AbstractNode> ElseIfConditions = new List<AbstractNode>();
        public AbstractNode ElseCondition;



        public IfElseIfElseNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
