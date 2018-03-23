using System;
namespace Compiler.AST.Nodes.DatatypeNodes.Graph
{
    public class GraphSetQuery : AbstractNode
    {
        public string AttributeName;
        public string AttributeValue;
        public GraphSetQuery(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
