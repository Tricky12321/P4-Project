using System;
namespace Compiler.AST.Nodes.LoopNodes
{
    public class ForLoopNode : AbstractNode
    {
        public AbstractNode VariableDeclaration;
        public AbstractNode ToValueOperation;
        public AbstractNode FromValueNode;
        public AbstractNode Increment;

        public ForLoopNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
