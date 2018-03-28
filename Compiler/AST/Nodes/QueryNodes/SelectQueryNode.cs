using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class SelectQueryNode : AbstractNode
    {
        public string Type;
        public string Variable;
        public AbstractNode WhereCondition;

        public SelectQueryNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
