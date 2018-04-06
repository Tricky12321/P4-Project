﻿using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class DeclarationNode : AbstractNode
    {
        public string Type;
        public AllType Type_enum => Utilities.FindTypeFromString(Type);
        public AbstractNode Assignment;
        public bool CollectionDcl = false;

        public DeclarationNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
