using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using System.Collections.Generic;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
using Compiler.AST.Nodes.LoopNodes;

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
            //console.WriteLine("FunctionNode");
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
            //console.WriteLine("ProgramNode");
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            //console.WriteLine("StartNode");
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            //console.WriteLine("GraphNode");
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
            //console.WriteLine("VertexNode");
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
            //console.WriteLine("EdgeNode");
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
            //console.WriteLine("GraphSetQueryNode");
            ProgramCode.Append($"SET {node.Attributes.Item1.Name} = {node.Attributes.Item3.ExpressionString()};\n");
        }

        public override void Visit(SetQueryNode node)
        {
            //console.WriteLine("SetQueryNode");
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
            //console.WriteLine("WhereNode");
            ProgramCode.Append(" WHERE ");
            VisitChildren(node);
        }

        public override void Visit(PushQueryNode node)
        {
            //console.WriteLine("PushNode");
            ProgramCode.Append($"PUSH {node.VariableToAdd} TO {node.VariableAddTo}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(PopQueryNode node)
        {
            //console.WriteLine("PopNode");
            ProgramCode.Append($"POP FROM {node.Variable}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(IfElseIfElseNode node)
        {
            //console.WriteLine("IfElseIfElseNode");
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
            //console.WriteLine("CodeBlockNode");
            VisitChildren(node);
        }

        public override void Visit(BoolComparisonNode node)
        {
            //console.WriteLine("BoolComparisonNode");
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
            //console.WriteLine("ExtendNode");
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
            //console.WriteLine("DequeueQueryNode");
            ProgramCode.Append("DEQUEUE FROM ");
            ProgramCode.Append($"{node.Variable}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(EnqueueQueryNode node)
        {
            //console.WriteLine("EnqueueQueryNode");
            ProgramCode.Append("ENQUEUE ");
            ProgramCode.Append($"{node.VariableToAdd} TO {node.VariableTo}");
            ProgramCode.Append(");\n");
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            //console.WriteLine("ExtractMaxQueryNode");
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
            //console.WriteLine("ExtractMinQueryNode");
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
            throw new NotImplementedException();
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
    }
}
