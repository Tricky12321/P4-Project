using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetQueryNode : AbstractNode
    {
        public Dictionary<string, KeyValuePair<string, string>> Attributes = new Dictionary<string, KeyValuePair<string, string>>();
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
