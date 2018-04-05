using System;
namespace Compiler.AST.Nodes.LoopNodes
{
    public class ForeachLoopNode : AbstractNode
    {
        public string VariableName;
        public string VariableType;
        public string InVariableName;
        public AbstractNode WhereCondition;

        public ForeachLoopNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }
    }
}
