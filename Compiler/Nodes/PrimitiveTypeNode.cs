using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Nodes
{
    class PrimitiveTypeNode : TypeNode
    {
        PrimitiveType Type;

        public PrimitiveTypeNode(PrimitiveType type)
        {
            Type = type;
        }
    }
}
