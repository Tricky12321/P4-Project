using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class EdgeDclsNode : AbstractNode
    {
        public EdgeDclsNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
