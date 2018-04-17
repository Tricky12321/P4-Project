using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SelectQueryNode : AbstractNode
    {
        public string Variable;
        public AbstractNode WhereCondition;
        public SelectQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
