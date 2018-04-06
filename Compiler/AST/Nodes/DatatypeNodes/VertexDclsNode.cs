using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    internal class VertexDclsNode : AbstractNode
    {
        public VertexDclsNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
