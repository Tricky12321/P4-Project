using System;
namespace Compiler.AST.Nodes
{
    public class FunctionParameterNode : AbstractNode
    {
        public string ParameterType;
        public string ParameterName;
        public FunctionParameterNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
