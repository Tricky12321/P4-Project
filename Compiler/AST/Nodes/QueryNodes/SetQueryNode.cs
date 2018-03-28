using System;
using System.Collections.Generic;
using Compiler.AST.Nodes.DatatypeNodes;

namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetQueryNode : AbstractNode
    {
        public List<Tuple<VariableAttributeNode, string, ExpressionNode>> Attributes = new List<Tuple<VariableAttributeNode, string, ExpressionNode>>();
        public AbstractNode WhereCondition;
        public bool SetAttributes = false;
        public string InVariable;
        public string AssignmentOperator;

        public SetQueryNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
