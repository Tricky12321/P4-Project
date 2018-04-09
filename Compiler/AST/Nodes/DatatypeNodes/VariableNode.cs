using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    
    public class VariableNode : VariableAttributeNode
    {
        public string Type;
        public bool Declaration = false;
        public AbstractNode Assignment;
        public VariableNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {

        }
    }
    
}
