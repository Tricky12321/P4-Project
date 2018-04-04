﻿using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;

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

        public abstract void Visit(DeclarationNode node);
        public abstract void Visit(AbstractNode node);
        public abstract void Visit(FunctionNode node);
        public abstract void Visit(FunctionParameterNode node);
        public abstract void Visit(ProgramNode node);
        public abstract void Visit(StartNode node);
        public abstract void Visit(GraphNode node);
        public abstract void Visit(VertexNode node);
        public abstract void Visit(EdgeNode node);
        public abstract void Visit(GraphSetQuery node);
        public abstract void Visit(SetQueryNode node);
        public abstract void Visit(WhereNode node);
        public abstract void Visit(ExtendNode node);
        public abstract void Visit(CollectionNode node);
        public abstract void Visit(IfElseIfElseNode node);
        

        public abstract void Visit(PredicateNode node);
        public abstract void Visit(PredicateParameterNode node);
        #region CollOPSvisits
        public abstract void Visit(DequeueQueryNode node);
        public abstract void Visit(EnqueueQueryNode node);
        public abstract void Visit(ExtractMaxQueryNode node);
        public abstract void Visit(ExtractMinQueryNode node);
        public abstract void Visit(PopQueryNode node);
        public abstract void Visit(PushQueryNode node);
        public abstract void Visit(SelectAllQueryNode node);
        public abstract void Visit(SelectQueryNode node);
        #endregion
    }
}
