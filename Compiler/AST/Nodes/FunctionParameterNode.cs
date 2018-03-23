using System;
namespace Compiler.AST.Nodes
{
    public class FunctionParameterNode : TerminalNode
    {
        public string ParameterType;
        public string ParameterName;
        public FunctionParameterNode()
        {
            
        }

        public override void Accept(IAstVisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
