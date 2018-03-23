using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class EdgeNode : AbstractNode
    {
        public string Name;

        public string VertexNameFrom;
        public string VertexNameTo;

        public Dictionary<string, string> ValueList = new Dictionary<string, string>();

        public EdgeNode(int LineNumber) : base(LineNumber)
        {
        }
    }
}
