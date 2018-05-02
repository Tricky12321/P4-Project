﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class RemoveQueryNode : AbstractNode
    {
        public string Variable;
        public AbstractNode WhereCondition;
        public RemoveQueryNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
