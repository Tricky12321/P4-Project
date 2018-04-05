using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class IfElseIfElseNode : AbstractNode
    {
        public BoolComparisonNode IfCondition;
        public CodeBlockNode IfCodeBlock;
        public List<Tuple<BoolComparisonNode, CodeBlockNode>> ElseIfList = new List<Tuple<BoolComparisonNode, CodeBlockNode>>();
        public CodeBlockNode ElseCodeBlock;

        public IfElseIfElseNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
