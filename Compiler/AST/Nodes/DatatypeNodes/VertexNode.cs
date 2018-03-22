using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class VertexNode : AbstractNode
    {
        public string Name;
        public Dictionary<string, string> ValueList = new Dictionary<string, string>();
        public VertexNode()
        {
            
        }
    }
}
