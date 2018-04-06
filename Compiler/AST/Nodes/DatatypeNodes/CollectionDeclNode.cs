using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class CollectionDeclNode : AbstractNode
    {
        // test
        public string Type;
        public AbstractNode Assignment;
        public CollectionDeclNode(int LineNumber, int CharIndex) : base (LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
