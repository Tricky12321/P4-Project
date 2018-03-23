using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;

namespace Compiler.AST
{
    class AstSymbolTableCreatorVisitor : IAstVisitorBase
    {
        public void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }
        }

        public void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public void Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionParameterNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(StartNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ProgramNode node)
        {
            throw new NotImplementedException();
        }



    }
}
