using System;
namespace Compiler.AST.Nodes
{
    public class ProgramNode : AbstractNode
    {
        public ProgramNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
