using System;
using Compiler.AST;
using Compiler.AST.Nodes;


namespace Compiler.AST
{
    public class AstVisitor : AstVisitorBase<AbstractNode>
    {
        public override void VisitRoot(AbstractNode root)
        {
            //root.Visit(this);
        }

        public override AbstractNode Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override AbstractNode Visit(ProgramNode node)
        {
            throw new NotImplementedException();
        }

        public override AbstractNode Visit(StartNode node)
        {
            throw new NotImplementedException();
        }

        public override AbstractNode Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
