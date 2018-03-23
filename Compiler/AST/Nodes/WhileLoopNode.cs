using System;
namespace Compiler.AST.Nodes
{
    public class WhileLoopNode : AbstractNode
    {
        public AbstractNode BoolCompare;
        public WhileLoopNode(int LineNumber) : base (LineNumber)
        {
        }
    }
}
