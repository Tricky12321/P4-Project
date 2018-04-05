﻿using System;
using System.Collections.Generic;
using Compiler;

namespace Compiler.AST.Nodes
{
    public class ExpressionNode : AbstractNode
    {
        public List<KeyValuePair<ExpressionPartType, string>> ExpressionParts = new List<KeyValuePair<ExpressionPartType, string>>();

        public ExpressionNode(int LineNumber) : base(LineNumber)
        {

        }

        public override string ToString()
        {
            string placeholderString = string.Empty;
            foreach (KeyValuePair<ExpressionPartType, string> part in ExpressionParts)
            {
                placeholderString += part.Value;
            }
            return placeholderString;
        }
    }
}
