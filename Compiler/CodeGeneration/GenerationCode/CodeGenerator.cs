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
        StringBuilder ProgramCode = new StringBuilder();

        public string ResolveTypeToCS(AllType type) {
            switch (type)
            {
                case(AllType.VERTEX):
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
            throw new NotImplementedException();
        }

        public override void Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StartNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GraphNode node)
        {
            ProgramCode.Append($"\nGraph {node.Name} = new Graph();\n\n");

            foreach (GraphDeclVertexNode vertex in node.Vertices)
            {
                ProgramCode.Append($"Vertex {vertex.Name} = new Vertex();\n");
                foreach (KeyValuePair<string, string> value in vertex.ValueList)
                {
                    ProgramCode.Append($"{vertex.Name}.{value.Key} = {value.Value};\n");
                }
                ProgramCode.Append($"{node.Name}.Vertices.Add({vertex.Name});\n\n");
            }

            foreach (GraphDeclEdgeNode edge in node.Edges)
            {
                ProgramCode.Append($"Edge {edge.Name} = new Edge({edge.VertexNameFrom}, {edge.VertexNameTo});\n");
                foreach (KeyValuePair<string, string> value in edge.ValueList)
                {
                    ProgramCode.Append($"{edge.Name}.{value.Key} = {value.Value};\n");
                }
                ProgramCode.Append($"{node.Name}.Edges.Add({edge.Name});\n\n");
            }

            ProgramCode.Append($"Graph.Directed = {node.Directed};\n\n");

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
            throw new NotImplementedException();
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GraphSetQuery node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SetQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(WhereNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtendNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IfElseIfElseNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BoolComparisonNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(CodeBlockNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(AddQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PredicateNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DequeueQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EnqueueQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PopQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PushQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SelectAllQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SelectQueryNode node)
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

        public override void Visit(WhileLoopNode node)
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

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(OperatorNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ConstantNode node)
        {
            throw new NotImplementedException();
        }
    }
}
