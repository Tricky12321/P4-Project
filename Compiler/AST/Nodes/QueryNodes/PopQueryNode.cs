﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PopQueryNode : AbstractNode
    {
        public string Variable;
        public AbstractNode WhereCondition;

        public PopQueryNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
