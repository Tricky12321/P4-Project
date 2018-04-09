using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST.Exceptions
{
    class WrongExpressionPartTypeFoundException : Exception
    {
        public WrongExpressionPartTypeFoundException(string message) : base(message)
        {
        }
    }
}
