using System;
namespace Compiler.AST.Nodes.DatatypeNodes.Graph
{
    public class EdgeDcl : AbstractNode
    {
        public string VertexFrom;
        public string VertexTo;

        public EdgeDcl(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }
    }
}
