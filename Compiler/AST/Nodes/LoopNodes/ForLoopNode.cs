using System;
namespace Compiler.AST.Nodes.LoopNodes
{
    public class ForLoopNode : AbstractNode
    {
        public AbstractNode VariableDeclartion;
        public AbstractNode ToValueOperation;
        public string ToValue;
        public bool ToVariable = false; // If the ToValue is a Variable
        public bool ToConst = false; // If the ToValue is a Constant
        public bool ToOperation = false; // If the ToValue is a operation (ToValueOperation is used)
        public AbstractNode IncrementValue;
        public ForLoopNode(int LineNumber) : base(LineNumber)
        {
            
        }

    }
}
