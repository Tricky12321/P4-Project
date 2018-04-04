using System;
using System.Collections.Generic;
using Compiler;

namespace Compiler.AST.Nodes
{
    public class ExpressionNode : AbstractNode
    {
        public List<Tuple<string, AllType, string>> expressionParts = new List<Tuple<string, AllType, string>>();

        public ExpressionNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
