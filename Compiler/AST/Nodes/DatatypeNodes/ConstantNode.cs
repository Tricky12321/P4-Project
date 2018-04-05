using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class ConstantNode : AbstractNode
    {
        public string Value;

        public ConstantNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }
    }
}
