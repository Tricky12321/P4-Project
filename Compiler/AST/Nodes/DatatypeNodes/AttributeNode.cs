using Compiler.AST.Nodes.DatatypeNodes;

namespace Compiler.AST
{
    internal class AttributeNode : VariableAttributeNode
    {
        public AttributeNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }
    }
}