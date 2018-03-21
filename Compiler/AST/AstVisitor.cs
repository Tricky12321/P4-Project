using System;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    internal abstract class AstVisitor<T>
    {
        public abstract T Visit(FunctionNode node);
        public abstract T Visit(ProgramNode node);
        public abstract T Visit(StartNode node);

        public T Visit(AbstractNode node)
        {
            return Visit((dynamic)node);
        }
    }
}
