using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Nodes
{
    class IfNode : AbstractNode
    {
        BoolExprNode Condition;
        CodeBlockNode ThenPart;
        CodeBlockNode ElsePart; 
    }
}
