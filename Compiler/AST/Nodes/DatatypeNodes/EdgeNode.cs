﻿using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class EdgeNode : AbstractNode
    {
        public string VertexNameFrom;
        public string VertexNameTo;

        public Dictionary<string, string> ValueList = new Dictionary<string, string>();

        public EdgeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
