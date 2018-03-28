using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SelectAllQueryNode : AbstractNode
    {
        public string Type;
        public string Variable;
        public AbstractNode WhereCondition;
        public SelectAllQueryNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
