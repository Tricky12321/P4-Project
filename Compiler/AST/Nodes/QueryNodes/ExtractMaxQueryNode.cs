using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class ExtractMaxQueryNode : AbstractNode
    {
        public string Attribute;
        public string Variable;
        public AbstractNode WhereCondition;
        public ExtractMaxQueryNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
        }
        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
