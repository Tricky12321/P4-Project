using System;
namespace Compiler.NewStuff
{
    internal abstract class AstVisitor<T>
    {
        public abstract T Visit(QueryNode node);
        public abstract T Visit(ReturnQueryNode node);
        public abstract T Visit(NoReturnQuery node);

        public T Visit(AbstractNode node)
        {
            if (node is QueryNode) {
                if (node is ReturnQueryNode) {
                    return Visit((ReturnQueryNode)node);
                }
                if (node is NoReturnQuery) {
                    return Visit((NoReturnQuery)node);
                }
                return Visit((QueryNode)node);
            }
            return Visit((AbstractNode)node);
        }
    }
}
