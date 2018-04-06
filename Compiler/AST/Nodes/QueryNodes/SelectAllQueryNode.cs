using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SelectAllQueryNode : AbstractNode
    {
        public string Type;
        public string Variable;
        public AbstractNode WhereCondition;
        public SelectAllQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
