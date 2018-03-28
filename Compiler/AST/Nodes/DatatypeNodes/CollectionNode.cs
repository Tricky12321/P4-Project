using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class CollectionNode : AbstractNode
    {
        // test
        public string Type;
        public AbstractNode Assignment;
        public CollectionNode(int LineNumber) : base (LineNumber)
        {
        }
    }
}
