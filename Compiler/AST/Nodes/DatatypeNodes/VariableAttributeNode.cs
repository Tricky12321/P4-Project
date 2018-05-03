using System;

namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableAttributeNode : AbstractNode
    {
        public AllType ClassType;
        public string ClassVariableName;
        public bool IsAttribute = false;
        public bool IsCollection = false;
        public VariableAttributeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}