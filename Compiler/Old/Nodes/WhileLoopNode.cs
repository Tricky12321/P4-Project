using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Nodes
{
    class WhileLoopNode : AbstractNode
    {
        BoolExprNode Condition;
        CodeBlockNode Body;
    }
}
