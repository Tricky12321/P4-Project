using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetQueryNode : AbstractNode
    {
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public AbstractNode WhereCondition;
        public SetQueryNode(int LineNumber) : base(LineNumber)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
