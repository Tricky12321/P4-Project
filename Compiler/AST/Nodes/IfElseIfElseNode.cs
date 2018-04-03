using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class IfElseIfElseNode : AbstractNode
    {
        public AbstractNode IfCondition;
        public AbstractNode IfCodeBlock;
        public List<AbstractNode> ElseIfConditions = new List<AbstractNode>();
        public List<AbstractNode> ElseIfCodeBlocks = new List<AbstractNode>();
        public AbstractNode ElseCodeBlock;

        public IfElseIfElseNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
