using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SelectQueryNode : AbstractNode
    {
        public string Type;
        public AllType Type_enum => Utilities.FindTypeFromString(Type);
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
