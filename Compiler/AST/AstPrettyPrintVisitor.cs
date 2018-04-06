using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using System.Collections.Generic;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
using Compiler.AST.Nodes.LoopNodes;
using System.Diagnostics;

namespace Compiler.AST
{
    public class AstPrettyPrintVisitor : AstVisitorBase
    {
        public StringBuilder ProgramCode = new StringBuilder();
        private int _currentLineNumber;

        public override void VisitRoot(AbstractNode root)
        {
            _currentLineNumber = root.LineNumber;
            base.VisitRoot(root);
        }

        private void InsertComma(ref int i)
        {
            if (i > 0)
            {
                ProgramCode.Append(", ");
            }
            i++;
        }

        public override void Visit(FunctionNode node)
        {
            
            Debug.Print("FunctionNode");
            ProgramCode.Append($"{node.Name} -> {node.ReturnType}(");
            int i = 0;
            foreach (FunctionParameterNode Param in node.Parameters)
            {
                InsertComma(ref i);
                Param.Accept(this);
            }
            ProgramCode.Append($")\n{{\n");
            VisitChildren(node);
            ProgramCode.Append("}\n");
        }

        public override void Visit(FunctionParameterNode node)
        {
            ProgramCode.Append($"{node.Type} {node.Name}");
        }

        public override void Visit(ProgramNode node)
        {
            Debug.Print("ProgramNode");
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            Debug.Print("StartNode");
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            Debug.Print("GraphNode");
            ProgramCode.Append($"GRAPH {node.Name}\n{{\n");

            if (node.Vertices.Count != 0)
            {
                ProgramCode.Append($"VERTEX ");
                int i = 0;
                foreach (VertexNode vertex in node.Vertices)
                {
                    InsertComma(ref i);
                    vertex.Accept(this);
                }
                ProgramCode.Append(");\n");
            }
            if (node.Edges.Count != 0)
            {
                ProgramCode.Append($"EDGE ");
                int i = 0;
                foreach (EdgeNode edge in node.Edges)
                {
                    InsertComma(ref i);
                    edge.Accept(this);
                }
                ProgramCode.Append(");\n");
            }
            if (node.LeftmostChild != null)
            {
                VisitChildren(node);
            }
            ProgramCode.Append($"}}\n");
        }

        public override void Visit(VertexNode node)
        {
            Debug.Print("VertexNode");
            ProgramCode.Append($"{node.Name}(");
            int i = 0;
            foreach (KeyValuePair<string, string> item in node.ValueList)
            {
                InsertComma(ref i);
                ProgramCode.Append($"{item.Key} = {item.Value}");
            }
            ProgramCode.Append(")");
        }

        public override void Visit(EdgeNode node)
        {
            Debug.Print("EdgeNode");
            ProgramCode.Append($"{node.Name}(");
            int i = 0;
            ProgramCode.Append($"{node.VertexNameFrom}, {node.VertexNameTo}");
            if (node.ValueList.Count > 0)
            {
                ProgramCode.Append(", ");
            }

            foreach (KeyValuePair<string, string> item in node.ValueList)
            {
                InsertComma(ref i);
                ProgramCode.Append($"{item.Key} = {item.Value}");
            }
            ProgramCode.Append(")");
        }

        public override void Visit(GraphSetQuery node)
        {
            Debug.Print("GraphSetQueryNode");
            ProgramCode.Append($"SET {node.Attributes.Item1.Name} = {node.Attributes.Item3.ExpressionString()};\n");
        }

        public override void Visit(SetQueryNode node)
        {
            Debug.Print("SetQueryNode");
            ProgramCode.Append("SET ");
            int i = 0;

            foreach (var attribute in node.Attributes)
            {
                InsertComma(ref i);
                ProgramCode.Append($"'{attribute.Item1.Name}' = {attribute.Item3.ExpressionString()}");
            }
            ProgramCode.Append($" IN {node.InVariable}");
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode.Append(");\n");
        }

        public override void Visit(WhereNode node)
        {
            Debug.Print("WhereNode");
            ProgramCode.Append(" WHERE ");
            VisitChildren(node);
        }

        public override void Visit(PushQueryNode node)
        {
            Debug.Print("PushNode");
            ProgramCode.Append($"PUSH {node.VariableToAdd} TO {node.VariableAddTo}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(PopQueryNode node)
        {
            Debug.Print("PopNode");
            ProgramCode.Append($"POP FROM {node.Variable}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(IfElseIfElseNode node)
        {
            Debug.Print("IfElseIfElseNode");
            ProgramCode.Append("IF (");
            node.IfCondition.Accept(this);
            ProgramCode.Append(")\n{\n");
            node.IfCodeBlock.Accept(this);
            ProgramCode.Append("}\n");
            for (int i = 0; i < node.ElseIfList.Count; i++)
            {
                ProgramCode.Append("ELSEIF (");
                node.ElseIfList[i].Item1.Accept(this);
                ProgramCode.Append(")\n{\n");
                node.ElseIfList[i].Item2.Accept(this);
                ProgramCode.Append("}\n");
            }
            if (node.ElseCodeBlock != null)
            {
                ProgramCode.Append("ELSE\n{\n");
                node.ElseCodeBlock.Accept(this);
                ProgramCode.Append("}\n");
            }
        }

        public override void Visit(CodeBlockNode node)
        {
            Debug.Print("CodeBlockNode");
            VisitChildren(node);
        }

        public override void Visit(BoolComparisonNode node)
        {
            Debug.Print("BoolComparisonNode");
            if (node.LeftmostChild != null)
            {
                VisitChildren(node);
            }
            else
            {
                if (node.Left != null)
                {
                    node.Left.Accept(this);
                    ProgramCode.Append($" {node.ComparisonOperator} ");
                    node.Right.Accept(this);
                }
            }
        }

        public override void Visit(ExpressionNode node)
        {
            ProgramCode.Append(node.ExpressionString());
        }

        #region CollOPSvisits
        public override void Visit(ExtendNode node)
        {
            Debug.Print("ExtendNode");
            ProgramCode.Append("EXTEND ");
            ProgramCode.Append($"{node.ClassToExtend} {node.ExtendWithType} ");
            ProgramCode.Append($"'{node.ExtensionName}'");
            if (node.ExtensionShortName != null)
            {
                ProgramCode.Append($":'{node.ExtensionShortName}'");
            }
            if (node.ExtensionDefaultValue != null)
            {
                ProgramCode.Append($"= {node.ExtensionDefaultValue}");
            }
            ProgramCode.Append(");\n");
        }

        public override void Visit(DequeueQueryNode node)
        {
            Debug.Print("DequeueQueryNode");
            ProgramCode.Append("DEQUEUE FROM ");
            ProgramCode.Append($"{node.Variable}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(EnqueueQueryNode node)
        {
            Debug.Print("EnqueueQueryNode");
            ProgramCode.Append("ENQUEUE ");
            ProgramCode.Append($"{node.VariableToAdd} TO {node.VariableTo}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            Debug.Print("ExtractMaxQueryNode");
            ProgramCode.Append("EXTRACTMAX ");
            if (node.Attribute != null)
            {
                ProgramCode.Append($"{node.Attribute} ");
            }
            ProgramCode.Append($"FROM {node.Variable}");
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode.Append(");\n");
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            Debug.Print("ExtractMinQueryNode");
            ProgramCode.Append("EXTRACTMIN ");
            if (node.Attribute != null)
            {
                ProgramCode.Append($"{node.Attribute} ");
            }
            ProgramCode.Append($"FROM {node.Variable}");
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode.Append(");\n");

        }

        public override void Visit(SelectAllQueryNode node)
        {
            ProgramCode.Append("SELECTALL ");
            ProgramCode.Append(node.Type);
            ProgramCode.Append("FROM");
            ProgramCode.Append(node.Variable);
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode.Append(";\n");
        }

        public override void Visit(SelectQueryNode node)
        {
            ProgramCode.Append("SELECT ");
            ProgramCode.Append(node.Type);
            ProgramCode.Append("FROM");
            ProgramCode.Append(node.Variable);
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode.Append(";\n");
        }

        #endregion

        public override void Visit(PredicateNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PredicateParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(CollectionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(AddQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            ProgramCode.Append("RETURN ");
            VisitChildren(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EdgeDclsNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VariableAttributeNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VariableNode node)
        {
            throw new NotImplementedException();
        }
        public override void Visit(TerminalNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ForLoopNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ForeachLoopNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }
    }
}
