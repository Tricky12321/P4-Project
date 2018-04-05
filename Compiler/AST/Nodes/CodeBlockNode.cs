using System;
namespace Compiler.AST.Nodes
{
    public class CodeBlockNode : AbstractNode
    {
        public CodeBlockNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }
    }
}
