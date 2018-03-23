using System;
namespace Compiler.AST.Nodes
{
    public class ProgramNode : AbstractNode
    {
        public ProgramNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
