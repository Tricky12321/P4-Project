using System;
namespace Compiler.AST.Nodes
{
    public class PredicateCall : AbstractNode
    {
        
        public PredicateCall(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
