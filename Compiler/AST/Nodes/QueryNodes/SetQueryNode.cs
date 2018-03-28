using System;
using System.Collections.Generic;
using Compiler.AST.Nodes.DatatypeNodes;

namespace Compiler.AST.Nodes.QueryNodes
{
    public class SetQueryNode : AbstractNode
    {
        public Dictionary<VariableAttributeNode, ExpressionNode> Attributes = new Dictionary<VariableAttributeNode, ExpressionNode>();
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
