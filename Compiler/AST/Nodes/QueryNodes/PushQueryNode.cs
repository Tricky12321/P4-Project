using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PushQueryNode : AbstractNode
    {
        public AbstractNode VariableToAdd;
        public string VariableAddTo;
        public AbstractNode WhereCondition;
        public PushQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
