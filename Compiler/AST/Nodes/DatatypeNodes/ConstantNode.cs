using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class ConstantNode : AbstractNode
    {
        public string Type;
        public AllType Type_enum => Utilities.FindTypeFromString(Type);
        public string Value;

        public ConstantNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex) { }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
