using System;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    public abstract class AstVisitorBase
    {
        public virtual void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }
        }

        public virtual void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public abstract void Visit(AbstractNode node);
        public abstract void Visit(FunctionNode node);
        public abstract void Visit(FunctionParameterNode node);
        public abstract void Visit(ProgramNode node);
        public abstract void Visit(StartNode node);
        public abstract void Visit(GraphNode node);
    }
}
