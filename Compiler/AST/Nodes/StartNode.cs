using System;
namespace Compiler.AST.Nodes
{
    public class StartNode : AbstractNode
    {
        public StartNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(IAstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
