using System;
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
            ProgramCode += "\n}\n";
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

        public override void Visit(SetQueryNode node)
        {
            Console.WriteLine("SetQueryNode");
            ProgramCode += $"SET ";
            int i = 0;
            foreach (KeyValuePair<string, string> attribute in node.Attributes)
            {
                InsertComma(ref i);
                ProgramCode += $"'{attribute.Key}' = {attribute.Value}";
            }
            ProgramCode += $" IN {node.Name}";
            if (node.WhereCondition == null)
            {
                ProgramCode += $";";
            }
            else
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(WhereNode node)
        {
            Console.WriteLine("WhereNode");
            ProgramCode += $" WHERE A == A;";
        }

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
