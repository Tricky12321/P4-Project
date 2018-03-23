using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST.Nodes
{
    public class TerminalNode : AbstractNode
    {
        public TerminalNode(int LineNumber) : base(LineNumber)
        {
            
        }
    }
}
