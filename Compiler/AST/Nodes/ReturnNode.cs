using System;
namespace Compiler.AST.Nodes
{
    public class ReturnNode : AbstractNode
    {
        public string FuncName;
        public ReturnNode(int LineNumber, int CharIndex) : base (LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
