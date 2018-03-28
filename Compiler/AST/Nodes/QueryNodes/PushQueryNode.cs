using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PushQueryNode : AbstractNode
    {
        public string VariableToAdd;
        public string VariableAddTo;
        public AbstractNode WhereCondition;
        public PushQueryNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
