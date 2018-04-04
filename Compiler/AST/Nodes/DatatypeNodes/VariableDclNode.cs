using System;
namespace Compiler.AST.Nodes.DatatypeNodes
{
    public class VariableDclNode : AbstractNode
    {
        public string Type;
        public VariableDclNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
