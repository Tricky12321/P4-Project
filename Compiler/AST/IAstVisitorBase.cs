using System;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    public interface IAstVisitorBase
    {
        void VisitChildren(AbstractNode node);
        void VisitRoot(AbstractNode root);
        void Visit(AbstractNode node);
        void Visit(FunctionNode node);
        void Visit(FunctionParameterNode node);
        void Visit(ProgramNode node);
        void Visit(StartNode node);
    }
}
