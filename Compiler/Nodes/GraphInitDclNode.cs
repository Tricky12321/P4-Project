using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Nodes
{
    class GraphInitDclNode : AbstractNode
    {
        String Name;
        List<VertexDclsNode> VertexDcls;
        List<EdgeDclsNode> EdgeDcls;
        List<SetQueryNode> SetDirecteds;
    }
}
