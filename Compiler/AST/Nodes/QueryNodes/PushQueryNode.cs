﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PushQueryNode : AbstractNode
    {
        public AbstractNode VariableToAdd;
        public string variableName => VariableToAdd.Name;
        public string VariableCollection;
        public PushQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
