using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class ExtractMaxQueryNode : AbstractNode
    {
        public string Attribute;
        public string Variable;
        public AbstractNode WhereCondition;
        public ExtractMaxQueryNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
