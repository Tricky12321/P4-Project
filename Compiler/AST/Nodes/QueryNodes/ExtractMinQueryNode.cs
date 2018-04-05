using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class ExtractMinQueryNode : AbstractNode
    {
        public string Attribute;
        public string Variable;
        public AbstractNode WhereCondition;
        public ExtractMinQueryNode(int LineNumber) : base(LineNumber)
        {
        }
        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
