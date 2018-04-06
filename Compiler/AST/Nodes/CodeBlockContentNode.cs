using System;
namespace Compiler.AST.Nodes
{
    public class CodeBlockContentNode : AbstractNode
    {
        public CodeBlockContentNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
