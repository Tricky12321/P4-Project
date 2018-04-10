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
        public bool SetVariables = false;
        public string InVariable;
        public string AssignmentOperator;

        public SetQueryNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
