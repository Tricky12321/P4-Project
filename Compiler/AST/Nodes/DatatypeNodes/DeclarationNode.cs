﻿using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class DeclarationNode : AbstractNode
    {
        public string Type;
        public AbstractNode Assignment;
        public bool CollectionDcl = false;

        public DeclarationNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
