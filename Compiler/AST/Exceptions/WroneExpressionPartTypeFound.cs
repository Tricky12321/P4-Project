using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST.Exceptions
{
    class WroneExpressionPartTypeFound : Exception
    {
        public WroneExpressionPartTypeFound(string message) : base(message)
        {
        }
    }
}
