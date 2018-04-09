﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class PopQueryNode : AbstractNode
    {
        public string Variable;
        public PopQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
