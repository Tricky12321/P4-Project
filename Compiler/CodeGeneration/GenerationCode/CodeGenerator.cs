using System;
using Compiler.AST;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
using System.Collections.Generic;

namespace Compiler.CodeGeneration.GenerationCode
{
    public class CodeGenerator : AstVisitorBase
    {
        public StringBuilder MainBody;
        private StringBuilder _currentStringBuilder;
        private StringBuilder Global = new StringBuilder();
        public StringBuilder Functions;

        public CodeGenerator(FunctionGeneration functionGeneration) {
            MainBody = functionGeneration.MainBody;
            Functions = functionGeneration.Functions;
            _currentStringBuilder = Functions;
        }

        public string ResolveTypeToCS(AllType type)
        {
            switch (type)
            {
                case (AllType.VERTEX):
                    return "Vertex";
                case (AllType.EDGE):
                    return "Edge";
                case (AllType.GRAPH):
                    return "Graph";
                case (AllType.BOOL):
                    return "bool";
                case (AllType.STRING):
                    return "string";
                case (AllType.INT):
                    return "int";
                case (AllType.DECIMAL):
                    return "decimal";
                case (AllType.VOID):
                    return "void";
                default:
                    return AllType.UNKNOWNTYPE.ToString();
            }
        }

        public override void Visit(DeclarationNode node)
        {
             
        }

        public override void Visit(FunctionNode node)
        {

            if (node.Name == "Main")
            {
                _currentStringBuilder = MainBody;
                VisitChildren(node);
            }
            else
            {
                _currentStringBuilder = Functions;
                _currentStringBuilder.Append($"public static");
                _currentStringBuilder.Append($" {ResolveTypeToCS(Utilities.FindTypeFromString(node.ReturnType))}");
                _currentStringBuilder.Append($" {node.Name} (");
                bool first = true;
                foreach (var item in node.Parameters)
                {
                    if (first) {
                        first = false;
                        _currentStringBuilder.Append($"{ResolveTypeToCS(item.Type_enum)} {item.Name}");
                    } else {
                        _currentStringBuilder.Append($", {ResolveTypeToCS(item.Type_enum)} {item.Name}");
                    }
                }
                _currentStringBuilder.Append(") \n {");
                VisitChildren(node);
                _currentStringBuilder.Append("\n}");
            }
			_currentStringBuilder = Functions;
        }

        public override void Visit(ReturnNode node)
        {
            _currentStringBuilder.Append("return ");
            node.Children[0].Accept(this);
            _currentStringBuilder.Append(";\n");
        }

        public override void Visit(ParameterNode node)
        {
            
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            _currentStringBuilder.Append($"\nGraph {node.Name} = new Graph();\n\n");

            foreach (GraphDeclVertexNode vertex in node.Vertices)
            {
                string vertexName = vertex.Name == null ? "_new" : vertex.Name;
                _currentStringBuilder.Append($"Vertex {vertexName} = new Vertex();\n");
                foreach (KeyValuePair<string, string> value in vertex.ValueList)
                {
                    _currentStringBuilder.Append($"{vertexName}.{value.Key} = {value.Value};\n");
                }
                _currentStringBuilder.Append($"{node.Name}.Vertices.Add({vertexName});\n\n");
            }

            foreach (GraphDeclEdgeNode edge in node.Edges)
            {
                string edgeName = edge.Name == null ? "_new" : edge.Name;
                _currentStringBuilder.Append($"Edge {edgeName} = new Edge({edge.VertexNameFrom}, {edge.VertexNameTo});\n");
                foreach (KeyValuePair<string, string> value in edge.ValueList)
                {
                    _currentStringBuilder.Append($"{edgeName}.{value.Key} = {value.Value};\n");
                }
                _currentStringBuilder.Append($"{edgeName}.Edges.Add({edgeName});\n\n");
            }

            _currentStringBuilder.Append($"Graph.Directed = {node.Directed};\n\n");
            Console.WriteLine(_currentStringBuilder);
            /*  
            graph Graph1 
                {
		            vertex Ve(), Va();
		            edge Eb(Ve, Va);
		            set 'Directed' = true;
                }

            --------------------------------
            ^ Should be like v in c#
            --------------------------------

            Graph Graph1 = new Graph();
            
            Vertex Ve = new Vertex();
            Graph1.Vertices.Add(Ve);
            
            Vertex Va = new Vertex();
            Graph1.Vertices.Add(Va);
            
            Edge Eb = new Edge(Ve, Va);
            Graph1.Edges.Add(Eb);
            
            Graph1.Directed = true;
            */
        }

        public override void Visit(VariableDclNode node)
        {
            _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum)+" ");
            _currentStringBuilder.Append(node.Name);
            if (node.Children.Count > 0) {
                _currentStringBuilder.Append(" = ");
                foreach (var item in node.Children)
                {
                    item.Accept(this);
                }
                _currentStringBuilder.Append(";\n");
            }
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            
        }

        public override void Visit(GraphSetQuery node)
        {
            
        }

        public override void Visit(SetQueryNode node)
        {

        }

        public override void Visit(WhereNode node)
        {
            
        }

        public override void Visit(ExtendNode node)
        {
            
        }

        public override void Visit(IfElseIfElseNode node)
        {
            _currentStringBuilder.Append("if (");
            node.IfCondition.Accept(this);
            _currentStringBuilder.Append(") \n {");
            node.IfCodeBlock.Accept(this);
            _currentStringBuilder.Append("\n } ");
            foreach (var item in node.ElseIfList)
            {
                _currentStringBuilder.Append("\n else if (");
                item.Item1.Accept(this); // BoolComparison
                _currentStringBuilder.Append(") \n {");
                item.Item2.Accept(this); // Codeblock
                _currentStringBuilder.Append("\n } "); 
            }
            if (node.ElseCodeBlock != null) {
                _currentStringBuilder.Append("\n else \n {{");
                node.ElseCodeBlock.Accept(this);
                _currentStringBuilder.Append("\n } "); 
            }
        }

        public override void Visit(BoolComparisonNode node)
        {
            if (node.InsideParentheses)
            {
                _currentStringBuilder.Append("(");
                node.Children[0].Accept(this);
                _currentStringBuilder.Append(")");
            }
            else if (node.Prefix != null && node.Prefix != "")
            {
                _currentStringBuilder.Append(node.Prefix);
                node.Children[0].Accept(this);
            }
            else if (node.Left != null && node.Right != null)
            {
                node.Left.Accept(this);
                _currentStringBuilder.Append(node.ComparisonOperator);
                node.Right.Accept(this);
            } else {
                node.Children[0].Accept(this);
            }
        }

        public override void Visit(ExpressionNode node)
        {
            foreach (var item in node.ExpressionParts)
            {
                item.Accept(this);
            }
        }

        public override void Visit(CodeBlockNode node)
        {
            foreach (var item in node.Children)
            {
                item.Accept(this);
            }
        }

        public override void Visit(AddQueryNode node)
        {
        }

        public override void Visit(PredicateNode node)
        {
        }

        public override void Visit(DequeueQueryNode node)
        {
        }

        public override void Visit(EnqueueQueryNode node)
        {
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
        }

        public override void Visit(ExtractMinQueryNode node)
        {
        }

        public override void Visit(PopQueryNode node)
        {
        }

        public override void Visit(PushQueryNode node)
        {
        }

        public override void Visit(SelectAllQueryNode node)
        {
             
        }

        public override void Visit(SelectQueryNode node)
        {
             
        }

        public override void Visit(ForLoopNode node)
        {
             
        }

        public override void Visit(ForeachLoopNode node)
        {
             
        }

        public override void Visit(WhileLoopNode node)
        {
             
        }

        public override void Visit(VariableAttributeNode node)
        {
             
        }

        public override void Visit(VariableNode node)
        {
            _currentStringBuilder.Append(node.Name);
        }

        public override void Visit(AbstractNode node)
        {
             
        }

        public override void Visit(OperatorNode node)
        {
            _currentStringBuilder.Append(node.Operator);
        }

        public override void Visit(ConstantNode node)
        {
            if (node.Type_enum == AllType.STRING) {
				_currentStringBuilder.Append(node.Value);
            } else if (node.Type_enum == AllType.DECIMAL) {
                _currentStringBuilder.Append(node.Value);
                _currentStringBuilder.Append("m");
            } else if (node.Type_enum == AllType.BOOL ||  node.Type_enum == AllType.INT) {
                _currentStringBuilder.Append(node.Value);
            }
        }

        public override void Visit(PrintQueryNode node)
        {
            bool first = true;
            _currentStringBuilder.Append("Console.WriteLine(");
            foreach (var item in node.Children)
            {
                if (first) {
                    item.Accept(this);
                    first = false;
                } else {
					_currentStringBuilder.Append("+");
                    item.Accept(this);
                }
            }
            _currentStringBuilder.Append(");\n");
        }
    }
}
