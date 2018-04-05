using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetAtriQueryNode : AbstractNode
    {
        public string Variable;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public AbstractNode WhereCondition;

        public SetAtriQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }
    }
}
