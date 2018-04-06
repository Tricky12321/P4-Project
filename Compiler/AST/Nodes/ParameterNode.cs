using System;
namespace Compiler.AST.Nodes
{
    public class ParameterNode : TerminalNode
    {
        public string Type;
        public ParameterNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase visitor)
        {
            visitor.Visit(this);
        }
    }
}
