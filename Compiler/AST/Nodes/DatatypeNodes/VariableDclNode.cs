using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableDclNode : AbstractNode
    {
        public string Type;
        public VariableDclNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
