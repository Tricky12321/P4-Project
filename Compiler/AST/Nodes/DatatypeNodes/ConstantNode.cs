using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class ConstantNode : AbstractNode
    {
		private string _value;

        public string Value
        {
            get
            {
                if (this.Type_enum == AllType.DECIMAL)
                {
                    return _value + "m";
                }
                else
                {
                    return _value;
                }
            }

            set
            {
                _value = value;
            }
        }

        public ConstantNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex) { }



        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
