using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class WhereNode : AbstractNode
    {
        public WhereNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
