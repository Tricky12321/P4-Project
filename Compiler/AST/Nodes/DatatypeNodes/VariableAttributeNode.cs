﻿using System;

namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableAttributeNode : AbstractNode
    {
        public string Type;
        public AllType Type_enum => Utilities.FindTypeFromString(Type);
        public VariableAttributeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            throw new NotImplementedException("Skal også kende klasse");
            astVisitor.Visit(this);
        }
    }
}