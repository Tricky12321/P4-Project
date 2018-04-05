using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST.Exceptions
{
    class WrongExpressionPartTypeFound : Exception
    {
        public WrongExpressionPartTypeFound(string message) : base(message)
        {
        }
    }
}
