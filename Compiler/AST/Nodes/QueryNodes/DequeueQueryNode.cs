﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class DequeueQueryNode : AbstractNode
    {
        public string Variable;
        public AbstractNode WhereCondition;
        public DequeueQueryNode(int LineNumber) : base(LineNumber)
        {
            
        }
        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
