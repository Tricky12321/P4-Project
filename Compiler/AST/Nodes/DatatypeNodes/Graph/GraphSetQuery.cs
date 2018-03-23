using System;
namespace Compiler.AST.Nodes.DatatypeNodes
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
