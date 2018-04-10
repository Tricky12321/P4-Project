﻿using System;
using System.Collections.Generic;
using Compiler;

namespace Compiler.AST.Nodes
{
    public class ExpressionNode : AbstractNode
    {
        //public List<KeyValuePair<ExpressionPartType, string>> ExpressionParts = new List<KeyValuePair<ExpressionPartType, string>>();
        public List<AbstractNode> ExpressionParts = new List<AbstractNode>();

        public ExpressionNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex) { }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }

        public string ExpressionString()
        {
            string placeholderString = string.Empty;
            /*foreach (KeyValuePair<ExpressionPartType, string> part in ExpressionParts)
            {
                placeholderString += part.Value.ToString();
            }*/

            foreach (AbstractNode part in ExpressionParts)
            {
                placeholderString += part.Name;
            }
            return placeholderString;
        }

    }
}
