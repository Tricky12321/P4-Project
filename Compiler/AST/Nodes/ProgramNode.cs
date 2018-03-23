using System;
namespace Compiler.AST.Nodes
{
    public class ProgramNode : AbstractNode
    {
        public ProgramNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(IAstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
