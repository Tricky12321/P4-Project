using System;
namespace Compiler.AST.Exceptions
{
    public class NoAttributeByThisNameException : Exception
    {
        public NoAttributeByThisNameException(string Message) : base (Message)
        {
        }
    }
}
