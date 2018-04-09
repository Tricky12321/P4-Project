using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class DequeueQueryNode : AbstractNode
    {
        public string Variable;
        public DequeueQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
