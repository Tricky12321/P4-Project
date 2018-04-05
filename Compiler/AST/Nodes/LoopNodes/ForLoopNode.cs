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
        public bool IncrementVariable = false; // If the ToValue is a Variable
        public bool IncrementConst = false; // If the ToValue is a Constant
        public bool IncrementOperation = false; // If the ToValue is a operation (ToValueOperation is used)
        public AbstractNode IncrementValueOperation;
        public string IncrementValue;

        public ForLoopNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

    }
}
