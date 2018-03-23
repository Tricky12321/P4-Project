using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableNode : AbstractNode
    {
        public string VariableName;

        public VariableNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
