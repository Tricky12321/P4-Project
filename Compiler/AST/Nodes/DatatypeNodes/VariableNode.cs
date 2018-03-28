using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    
    public class VariableNode : VariableAttributeNode
    {
        public bool Declaration = false;
        public AbstractNode Assignment;
        public VariableNode(int LineNumber) : base(LineNumber)
        {

        }
    }
    
}
