using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class EnqueueQueryNode : AbstractNode
    {
        public string VariableToAdd;
        public string VariableTo;
        public AbstractNode WhereCondition;
        public EnqueueQueryNode(int LineNumber) : base (LineNumber)
        {
        }
        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
