using System;
namespace Compiler.AST.Nodes
{
    public class StartNode : AbstractNode
    {
        public StartNode()
        {
            
        }

        public override void Accept(IAstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
