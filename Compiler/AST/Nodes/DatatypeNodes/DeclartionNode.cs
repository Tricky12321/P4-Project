using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class DeclartionNode : AbstractNode
    {
        public string Type;
        public string Name;
        public AbstractNode Assignment;
        public DeclartionNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
