using System;
using System.Collections.Generic;

namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class GraphSetQuery : AbstractNode
    {
        public List<Tuple<VariableAttributeNode, string, ExpressionNode>> Attributes = new List<Tuple<VariableAttributeNode, string, ExpressionNode>>();
        public GraphSetQuery(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
