﻿using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableNode : AbstractNode
    {
        public bool Declaretion = false;
        public AbstractNode Assignment;
        public VariableNode(int LineNumber) : base(LineNumber)
        {

        }
    }
}
