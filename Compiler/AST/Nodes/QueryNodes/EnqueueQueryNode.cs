using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class EnqueueQueryNode : AbstractNode
    {
        public AbstractNode VariableToAdd;
        public string variableName => VariableToAdd.Name;
        public string VariableTo;
        public EnqueueQueryNode(int LineNumber, int CharIndex) : base (LineNumber, CharIndex)
        {
        }
        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
