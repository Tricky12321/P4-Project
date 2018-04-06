using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PrintQueryNode : AbstractNode
    {
        
        public PrintQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
