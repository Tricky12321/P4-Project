using System;
namespace Compiler.AST.Nodes
{
    public class FunctionParameterNode : TerminalNode
    {
        public string Type;
        public FunctionParameterNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
