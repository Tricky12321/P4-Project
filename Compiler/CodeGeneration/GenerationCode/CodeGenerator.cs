﻿using System;
using Compiler.AST;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
namespace Compiler.CodeGeneration.GenerationCode
{
    public class CodeGenerator : AstVisitorBase
    {
        public StringBuilder MainBody = new StringBuilder();
        private StringBuilder _currentStringBuilder;
        private StringBuilder Global = new StringBuilder();
        public StringBuilder Functions = new StringBuilder();

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
                StringBuilder FunctionHeader = new StringBuilder();
                FunctionHeader.Append($"public static");
                FunctionHeader.Append($" {ResolveTypeToCS(Utilities.FindTypeFromString(node.ReturnType))}");
                FunctionHeader.Append($" {node.Name} (");
                bool first = true;
                foreach (var item in node.Parameters)
                {
                    if (first) {
                        first = false;
						FunctionHeader.Append($"{ResolveTypeToCS(item.Type_enum)} {node.Name}");
                    } else {
                        FunctionHeader.Append($", {ResolveTypeToCS(item.Type_enum)} {node.Name}");
                    }
                }
                FunctionHeader.Append($") \n {{");
                VisitChildren(node);
                FunctionHeader.Append($"\n}}");
                _currentStringBuilder.AppendLine(FunctionHeader.ToString());
            }
			_currentStringBuilder = Functions;


        }

        public override void Visit(ReturnNode node)
        {
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
        }

        public override void Visit(VariableDclNode node)
        {
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
        }

        public override void Visit(BoolComparisonNode node)
        {
        }

        public override void Visit(ExpressionNode node)
        {
        }

        public override void Visit(CodeBlockNode node)
        {
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
             
        }

        public override void Visit(AbstractNode node)
        {
             
        }

        public override void Visit(OperatorNode node)
        {
             
        }

        public override void Visit(ConstantNode node)
        {
             
        }
    }
}
