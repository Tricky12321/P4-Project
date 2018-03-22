using System;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    public abstract class AstVisitorBase<T>
    {
        public abstract void VisitRoot(AbstractNode root);
        public abstract T Visit(AbstractNode node);
        public abstract T Visit(FunctionNode node);
        public abstract T Visit(ProgramNode node);
        public abstract T Visit(StartNode node);
    }
}
