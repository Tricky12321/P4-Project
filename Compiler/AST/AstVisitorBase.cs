using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.Nodes.LoopNodes;

namespace Compiler.AST
{
    public abstract class AstVisitorBase
    {
        public virtual void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                if (child != null)
                {
                    child.Accept(this);
                }
            }
        }

        public virtual void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public abstract void Visit(DeclarationNode node);
        public abstract void Visit(FunctionNode node);
        public abstract void Visit(ReturnNode node);
        public abstract void Visit(ParameterNode node);
        public abstract void Visit(StartNode node);
        public abstract void Visit(GraphNode node);
        public abstract void Visit(VariableDclNode node);
        public abstract void Visit(GraphDeclVertexNode node);
        public abstract void Visit(GraphDeclEdgeNode node);
        public abstract void Visit(GraphSetQuery node);
        public abstract void Visit(SetQueryNode node);
        public abstract void Visit(WhereNode node);
        public abstract void Visit(ExtendNode node);
        public abstract void Visit(IfElseIfElseNode node);
        public abstract void Visit(BoolComparisonNode node);
        public abstract void Visit(ExpressionNode node);
        public abstract void Visit(CodeBlockNode node);
        public abstract void Visit(AddQueryNode node);

        public abstract void Visit(PredicateNode node);
        public abstract void Visit(DequeueQueryNode node);
        public abstract void Visit(EnqueueQueryNode node);
        public abstract void Visit(ExtractMaxQueryNode node);
        public abstract void Visit(ExtractMinQueryNode node);
        public abstract void Visit(PopQueryNode node);
        public abstract void Visit(PushQueryNode node);
        public abstract void Visit(SelectAllQueryNode node);
        public abstract void Visit(SelectQueryNode node);
        public abstract void Visit(ForLoopNode node);
        public abstract void Visit(ForeachLoopNode node);
        public abstract void Visit(WhileLoopNode node);
        public abstract void Visit(VariableAttributeNode node);
        public abstract void Visit(VariableNode node);
        public abstract void Visit(AbstractNode node);
        public abstract void Visit(OperatorNode node);
        public abstract void Visit(ConstantNode node);
        public abstract void Visit(PrintQueryNode node);
        public abstract void Visit(RunQueryNode node);
    }
}
