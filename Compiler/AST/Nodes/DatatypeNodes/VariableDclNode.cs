using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableDclNode : AbstractNode
    {
		public bool Global = false;
        public VariableDclNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
