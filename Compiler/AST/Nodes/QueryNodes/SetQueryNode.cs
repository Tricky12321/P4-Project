using System;
using System.Collections.Generic;

namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetQueryNode : AbstractNode
    {
        public List<Tuple<string, string, string>> Attributes;
        public AbstractNode WhereCondition;
        public bool SetAtributes = false;
        public string InVaraible;

        public SetQueryNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
