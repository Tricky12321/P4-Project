using System;
namespace Compiler.AST.Nodes
{
    public class ParameterNode : AbstractNode
    {

        public bool IsCollection = false;
        public ParameterNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase Visitor)
        {
			Visitor.Visit(this);
        }
    }
}
