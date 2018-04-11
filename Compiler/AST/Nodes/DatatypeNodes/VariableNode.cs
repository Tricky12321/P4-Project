using System;
using System.Collections.Generic;

namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableNode : VariableAttributeNode
    {
        public bool Declaration = false;
        public AbstractNode Assignment;
        public List<String> variableParts = new List<string>();
        public VariableNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {

        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
