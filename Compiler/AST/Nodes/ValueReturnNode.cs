using System;
namespace Compiler.AST.Nodes
{
    public class ValueReturnNode : AbstractNode
    {
        string Value;
        public ValueReturnNode(string value)
        {
            Value = value;
        }
    }
}
