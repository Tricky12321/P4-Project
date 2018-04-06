using System;
namespace Compiler.AST.Nodes
{
    public class StartNode : AbstractNode
    {
        public StartNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
