namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableAttributeNode : AbstractNode
    {
        public VariableAttributeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}