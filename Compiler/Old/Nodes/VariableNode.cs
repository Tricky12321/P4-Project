﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Nodes
{
    class VariableNode : AbstractNode
    {
        string Name;
        TypeNode Type;
        ValueNode Value;
    }
}