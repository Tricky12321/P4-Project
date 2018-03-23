using System;
namespace Compiler.AST.Nodes
{
    public class ProgramNode : AbstractNode
    {
        public ProgramNode()
        {
            
        }

        public override void Accept(IAstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
