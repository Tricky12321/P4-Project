using System;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    internal abstract class AstVisitorBase<T>
    {
        public abstract T Visit(FunctionNode node);
        public abstract T Visit(ProgramNode node);
        public abstract T Visit(StartNode node);
    }
}
