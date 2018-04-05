﻿using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using System.Collections.Generic;
using Compiler.AST.Nodes.QueryNodes;

namespace Compiler.AST
{
    public class AstPrettyPrintVisitor : AstVisitorBase
    {
        public string ProgramCode;
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
                ProgramCode += ", ";
            }
            i++;
        }

        public override void Visit(FunctionNode node)
        {
            Console.WriteLine("FunctionNode");
            ProgramCode += $"{node.Name} -> {node.ReturnType}(";
            int i = 0;
            foreach (FunctionParameterNode Param in node.Parameters)
            {
                InsertComma(ref i);
                Param.Accept(this);
            }
            ProgramCode += $")\n{{\n";
            VisitChildren(node);
            ProgramCode += "}\n";
        }

        public override void Visit(FunctionParameterNode node)
        {
            ProgramCode += $"{node.Type} {node.Name}";
        }

        public override void Visit(ProgramNode node)
        {
            Console.WriteLine("ProgramNode");
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            Console.WriteLine("StartNode");
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            Console.WriteLine("GraphNode");
            ProgramCode += $"GRAPH {node.Name}\n{{\n";

            if (node.Vertices.Count != 0)
            {
                ProgramCode += $"VERTEX ";
                int i = 0;
                foreach (VertexNode vertex in node.Vertices)
                {
                    InsertComma(ref i);
                    vertex.Accept(this);
                }
                ProgramCode += ";\n";
            }
            if (node.Edges.Count != 0)
            {
                ProgramCode += $"EDGE ";
                int i = 0;
                foreach (EdgeNode edge in node.Edges)
                {
                    InsertComma(ref i);
                    edge.Accept(this);
                }
                ProgramCode += ";\n";
            }
            if (node.LeftmostChild != null)
            {
                VisitChildren(node);
            }
            ProgramCode += $"}}\n";
        }

        public override void Visit(VertexNode node)
        {
            Console.WriteLine("VertexNode");
            ProgramCode += $"{node.Name}(";
            int i = 0;
            foreach (KeyValuePair<string, string> item in node.ValueList)
            {
                InsertComma(ref i);
                ProgramCode += $"{item.Key} = {item.Value}";
            }
            ProgramCode += ")";
        }

        public override void Visit(EdgeNode node)
        {
            Console.WriteLine("EdgeNode");
            ProgramCode += $"{node.Name}(";
            int i = 0;
            ProgramCode += $"{node.VertexNameFrom}, {node.VertexNameTo}";
            if (node.ValueList.Count > 0)
            {
                ProgramCode += ", ";
            }

            foreach (KeyValuePair<string, string> item in node.ValueList)
            {
                InsertComma(ref i);
                ProgramCode += $"{item.Key} = {item.Value}";
            }
            ProgramCode += ")";
        }

        public override void Visit(GraphSetQuery node)
        {
            Console.WriteLine("GraphSetQueryNode");
            ProgramCode += $"SET {node.Attributes.Item1.Name} = {node.Attributes.Item3.ExpressionString()};\n";
        }

        public override void Visit(SetQueryNode node)
        {
            Console.WriteLine("SetQueryNode");
            ProgramCode += "SET ";
            int i = 0;

            foreach (var attribute in node.Attributes)
            {
                InsertComma(ref i);
                ProgramCode += $"'{attribute.Item1.Name}' = {attribute.Item3.ExpressionString()}";
            }
            ProgramCode += $" IN {node.InVariable}";
            if (node.WhereCondition == null)
            {
                ProgramCode += ";\n";
            }
            else
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(WhereNode node)
        {
            Console.WriteLine("WhereNode");
            ProgramCode += " WHERE ";
            VisitChildren(node);
        }

        public override void Visit(PushQueryNode node)
        {
            Console.WriteLine("PushNode");
            ProgramCode += $"PUSH {node.VariableToAdd} TO {node.VariableAddTo}";
            ProgramCode += ";\n";
        }

        public override void Visit(PopQueryNode node)
        {
            Console.WriteLine("PopNode");
            ProgramCode += $"POP FROM {node.Variable}";
            ProgramCode += ";\n";
        }

        public override void Visit(IfElseIfElseNode node)
        {
            Console.WriteLine("IfElseIfElseNode");
            ProgramCode += "IF (";
            node.IfCondition.Accept(this);
            ProgramCode += ")\n{\n";
            VisitChildren(node.IfCodeBlock);
            ProgramCode += "}\n";
            for (int i = 0; i < node.ElseIfCodeBlocks.Count; i++)
            {
                ProgramCode += "ELSEIF (";
                node.ElseIfConditions[i].Accept(this);
                ProgramCode += ")\n{\n";
                VisitChildren(node.ElseIfCodeBlocks[i]);
                ProgramCode += "}\n";
            }
            if (node.ElseCodeBlock != null)
            {
                ProgramCode += "ELSE\n{\n";
                VisitChildren(node.ElseCodeBlock);
                ProgramCode += "}\n";
            }
        }

        public override void Visit(BoolComparisonNode node)
        {
            Console.WriteLine("BoolComparisonNode");
            if (node.LeftmostChild != null)
            {
                VisitChildren(node);
            }
            else
            {
                if (node.Left != null)
                {
                    node.Left.Accept(this);
                    ProgramCode += $" {node.ComparisonOperator} ";
                    node.Right.Accept(this);
                }
            }
        }

        public override void Visit(ExpressionNode node)
        {
            ProgramCode += node.ExpressionString();
        }

        #region CollOPSvisits
        public override void Visit(ExtendNode node)
        {
            Console.WriteLine("ExtendNode");
            ProgramCode += "EXTEND ";
            ProgramCode += $"{node.ClassToExtend} {node.ExtendWithType} ";
            ProgramCode += $"'{node.ExtensionName}'";
            if (node.ExtensionShortName != null)
            {
                ProgramCode += $":'{node.ExtensionShortName}'";
            }
            if (node.ExtensionDefaultValue != null)
            {
                ProgramCode += $"= {node.ExtensionDefaultValue}";
            }
            ProgramCode += ";\n";
        }

        public override void Visit(DequeueQueryNode node)
        {
            Console.WriteLine("DequeueQueryNode");
            ProgramCode += "DEQUEUE FROM ";
            ProgramCode += $"{node.Variable}";
            ProgramCode += ";\n";
        }

        public override void Visit(EnqueueQueryNode node)
        {
            Console.WriteLine("EnqueueQueryNode");
            ProgramCode += "ENQUEUE ";
            ProgramCode += $"{node.VariableToAdd} TO {node.VariableTo}";
            ProgramCode += ";\n";
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            Console.WriteLine("ExtractMaxQueryNode");
            ProgramCode += "EXTRACTMAX ";
            if (node.Attribute != null)
            {
                ProgramCode += $"{node.Attribute} ";
            }
            ProgramCode += $"FROM {node.Variable}";
            if (node.WhereCondition != null)
            {
            node.WhereCondition.Accept(this);
            }
            ProgramCode += ";\n";
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            Console.WriteLine("ExtractMinQueryNode");
            ProgramCode += "EXTRACTMIN ";
            if (node.Attribute != null)
            {
                ProgramCode += $"{node.Attribute} ";
            }
            ProgramCode += $"FROM {node.Variable}";
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
            ProgramCode += ";\n";

        }

        public override void Visit(SelectAllQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SelectQueryNode node)
        {
            throw new NotImplementedException();
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

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
