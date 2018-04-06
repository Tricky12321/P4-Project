using System;
namespace Compiler.AST.Nodes.LoopNodes
{
    public class WhileLoopNode : AbstractNode
    {
        public AbstractNode BoolCompare;
        public WhileLoopNode(int LineNumber, int CharIndex) : base (LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
