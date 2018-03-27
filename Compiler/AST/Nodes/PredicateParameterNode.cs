using System;
namespace Compiler.AST.Nodes
{
    public class PredicateParameterNode : TerminalNode
    {
        public string Type;
        public PredicateParameterNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
