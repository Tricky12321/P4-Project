using System;
using Compiler.AST;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.CodeGeneration.GenerationCode
{
    public class CodeGenerator : AstVisitorBase
    {
        public StringBuilder MainBody;
        private StringBuilder _currentStringBuilder;
        private StringBuilder Global = new StringBuilder();
        public StringBuilder Functions;
        private int _forLoopCounter = 0;
        private int _newVariableCounter_private = 0;
        private string _newVariabelCounter
        {
            get
            {
                return "_newVariable" + _newVariableCounter_private++;
            }
        }

        public CodeGenerator(CodeWriter codeWriter)
        {
            MainBody = codeWriter.MainBody;
            Functions = codeWriter.Functions;
            _currentStringBuilder = Functions;

            _graphExtensions = codeWriter.GraphExtends;
            _vertexExtensions = codeWriter.VertexExtends;
            _edgeExtensions = codeWriter.EdgeExtends;
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

        public string ResolveBoolean(string Boolean)
        {
            return Boolean == "True" ? "true" : "false";
        }

        public string ResolveBoolean(bool Boolean)
        {
            return Boolean ? "true" : "false";
        }

        public override void Visit(DeclarationNode node)
        {
            if (node.CollectionDcl)
            {
                _currentStringBuilder.Append($"Collection<{ResolveTypeToCS(node.Type_enum)}> ");
                _currentStringBuilder.Append(node.Name);
                _currentStringBuilder.Append($"= new Collection<{ResolveTypeToCS(node.Type_enum)}>()");
            }
            else
            {
                _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum) + " ");
                _currentStringBuilder.Append(node.Name);
                if (node.Children.Count > 0)
                {
                    _currentStringBuilder.Append(" = ");
                    foreach (var item in node.Children)
                    {
                        item.Accept(this);
                    }
                }
            }
            _currentStringBuilder.Append(";\n");
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
                    if (first)
                    {
                        first = false;
                        item.Accept(this);
                    }
                    else
                    {
                        _currentStringBuilder.Append($",");
                        item.Accept(this);
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
            _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum));
            _currentStringBuilder.Append(" " + node.Name);
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            _currentStringBuilder.Append($"\nGraph {node.Name} = new Graph();\n\n");

            _currentStringBuilder.Append($"Vertex _newVertex{node.Name};\n");
            foreach (GraphDeclVertexNode vertex in node.Vertices)
            {
                string vertexName;
                if (vertex.Name == null)
                {
                    vertexName = $"_newVertex{node.Name}";
                    _currentStringBuilder.Append($"{vertexName} = new Vertex();\n");
                }
                else
                {
                    vertexName = vertex.Name;
                    _currentStringBuilder.Append($"Vertex {vertexName} = new Vertex();\n");
                }

                foreach (KeyValuePair<string, AbstractNode> value in vertex.ValueList)
                {
                    _currentStringBuilder.Append($"{vertexName}.{value.Key} = ");
                    value.Value.Accept(this);
                    _currentStringBuilder.Append($";\n");

                }
                _currentStringBuilder.Append($"{node.Name}.Vertices.Push({vertexName});\n\n");
            }

            _currentStringBuilder.Append($"Edge _newEdge{node.Name};\n");
            foreach (GraphDeclEdgeNode edge in node.Edges)
            {
                string edgeName;
                if (edge.Name == null)
                {
                    edgeName = $"_newEdge{node.Name}";
                    _currentStringBuilder.Append($"{edgeName} = new Edge();\n");
                }
                else
                {
                    edgeName = edge.Name;
                    _currentStringBuilder.Append($"Edge {edgeName} = new Edge({edge.VertexNameFrom},{edge.VertexNameTo});\n");
                }

                foreach (KeyValuePair<string, AbstractNode> value in edge.ValueList)
                {
                    _currentStringBuilder.Append($"{edgeName}.{value.Key} = ");
                    value.Value.Accept(this);
                    _currentStringBuilder.Append($";\n");
                }
                _currentStringBuilder.Append($"{node.Name}.Edges.Push({edgeName});\n\n");
            }

            _currentStringBuilder.Append($"{node.Name}.Directed = {ResolveBoolean(node.Directed)};\n\n");
        }

        public override void Visit(VariableDclNode node)
        {
            _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum) + " ");
            _currentStringBuilder.Append(node.Name);
            if (node.Children.Count > 0)
            {
                _currentStringBuilder.Append(" = ");
                foreach (var item in node.Children)
                {
                    item.Accept(this);
                }
            }
            _currentStringBuilder.Append(";\n");
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            throw new Exception("This should be done in the GraphDeclNode");
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            throw new Exception("This should be done in the GraphDeclNode");
        }

        public override void Visit(GraphSetQuery node)
        {
            throw new Exception("This should be done in the GraphDeclNode");
        }

        public override void Visit(SetQueryNode node)
        {
            if (node.SetAttributes)
            {
                foreach (var item in node.Attributes)
                {
                    string InVaraible = node.InVariable.Name;
                    string VariableName = item.Item1.Name;
                    string AssignOperator = item.Item2;
                    ExpressionNode expression = item.Item3;
                    _currentStringBuilder.Append($"{InVaraible}.{VariableName} {AssignOperator} ");
                    expression.Accept(this);
                    _currentStringBuilder.Append($";\n");
                }
            }
            else if (node.SetVariables)
            {
                foreach (var item in node.Attributes)
                {
                    string VariableName = item.Item1.Name;
                    string AssignOperator = item.Item2;
                    ExpressionNode expression = item.Item3;
                    _currentStringBuilder.Append($"{VariableName} {AssignOperator} ");
                    expression.Accept(this);
                    _currentStringBuilder.Append($";\n");
                }
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            _currentStringBuilder.Append($"_fun{node.ID}();\nCollection<{node.Type}> _fun{node.ID}(){{\n");
            _currentStringBuilder.Append($"Collection<{node.Type}> _col{node.ID} = new Collection<{node.Type}>();\n");

            _currentStringBuilder.Append($"foreach (var place in {node.Variable}){{\n");
            _currentStringBuilder.Append("if (");
            node.WhereCondition.Children[0].Accept(this);
            _currentStringBuilder.Append("){\n");
            _currentStringBuilder.Append($"_col{node.ID}.Add(place);\n}}\n}}\n");

            _currentStringBuilder.Append($"return _col{node.ID};\n}}");
        }

        public override void Visit(SelectQueryNode node)
        {
            _currentStringBuilder.Append($"_fun{node.ID}();\n{node.Type} _fun{node.ID}(){{\n");
            _currentStringBuilder.Append($"{node.Type} _val{node.ID};\n");

            _currentStringBuilder.Append($"foreach (var place in {node.Variable}){{\n");
            _currentStringBuilder.Append("if (");
            node.WhereCondition.Children[0].Accept(this);
            _currentStringBuilder.Append("){\n");
            _currentStringBuilder.Append($"_val{node.ID} = place;\n break;\n}}\n}}\n");

            _currentStringBuilder.Append($"return _val{node.ID};\n}}");
        }

        public override void Visit(WhereNode node)
        {
            //.Where(x => x.{)
            throw new NotImplementedException();
        }

        /*private string getWhereString(WhereNode node)
        {
            string stringPlaceholder = ".Where(";
            //stringPlaceholder += node.

            return stringPlaceholder;
        }*/

        public override void Visit(ExtendNode node)
        {
            ExtendClass(node.ClassToExtend_enum, node.ExtendWithType_enum, node.ExtensionName, node.ExtensionShortName, node.IsCollection);
        }

        public override void Visit(IfElseIfElseNode node)
        {
            _currentStringBuilder.Append("\nif (");
            node.IfCondition.Accept(this);
            _currentStringBuilder.Append(") \n {");
            node.IfCodeBlock.Accept(this);
            _currentStringBuilder.Append("\n } \n");
            foreach (var item in node.ElseIfList)
            {
                _currentStringBuilder.Append("\n else if (");
                item.Item1.Accept(this); // BoolComparison
                _currentStringBuilder.Append(") \n {");
                item.Item2.Accept(this); // Codeblock
                _currentStringBuilder.Append("\n }\n ");
            }
            if (node.ElseCodeBlock != null)
            {
                _currentStringBuilder.Append("\n else \n {");
                node.ElseCodeBlock.Accept(this);
                _currentStringBuilder.Append("\n } \n");
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
            }
            else
            {
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
            if (node.IsGraph)
            {
                foreach (var item in node.Dcls)
                {
                    if (item is GraphDeclVertexNode || item is GraphDeclEdgeNode)
                    {
                        string _newVariable = _newVariabelCounter;
                        string type = item is GraphDeclEdgeNode ? "Edge" : "Vertex";
                        _currentStringBuilder.Append($"{type} {_newVariable} = new {type}();\n");

                        foreach (var val in ((GraphDeclVertexNode)item).ValueList)
                        {
                            _currentStringBuilder.Append($"{_newVariable}.{val.Key} = ");
                            val.Value.Accept(this);
                            _currentStringBuilder.Append($";\n");
                        }
                        if (item is GraphDeclEdgeNode)
                        {
                            _currentStringBuilder.Append($"{node.ToVariable}.Edges.Push({_newVariable});\n");
                        }
                        else
                        {
                            _currentStringBuilder.Append($"{node.ToVariable}.Vertices.Push({_newVariable});\n");
                        }
                    }
                }
            }
            else
            {
                foreach (var item in node.TypeOrVariable)
                {
                    _currentStringBuilder.Append($"{node.ToVariable}.Push(");
                    item.Accept(this);
                    _currentStringBuilder.Append($");\n");
                }
            }
        }

        public override void Visit(PredicateNode node)
        {
            _currentStringBuilder.Append($"\nbool {node.Name} (");

            bool first = true;
            
            foreach (var item in node.Parameters)
            {
                if (first) {
                    item.Accept(this);
                    first = false;
                } else {
					_currentStringBuilder.Append(",");
                    item.Accept(this);
                }
            }

            _currentStringBuilder.Append($") {{ \n return ");
            VisitChildren(node);
            _currentStringBuilder.Append($"; \n }}\n");


            bool test() {
                return first;
            }

            if (test()) {
                
            }

        }

        public override void Visit(DequeueQueryNode node)
        {
            _currentStringBuilder.Append($"{node.Variable}.Dequeue;");
        }

        public override void Visit(EnqueueQueryNode node)
        {
            _currentStringBuilder.Append($"{node.VariableCollection}.Enqueue(");
            node.VariableToAdd.Accept(this);
            _currentStringBuilder.Append($");\n");
        }

        public override void Visit(ExtractMaxQueryNode node)
        {

        }

        public override void Visit(ExtractMinQueryNode node)
        {

        }

        public override void Visit(PopQueryNode node)
        {
            _currentStringBuilder.Append($"{node.Variable}.Pop;\n");

        }

        public override void Visit(PushQueryNode node)
        {
            _currentStringBuilder.Append($"{node.VariableCollection}.Push(");
            node.VariableToAdd.Accept(this);
            _currentStringBuilder.Append($");\n");
        }

        public override void Visit(ForLoopNode node)
        {
            _currentStringBuilder.Append("\nfor (");
            if (node.VariableDeclaration != null)
            {
                node.VariableDeclaration.Accept(this);
                node.ToValueOperation.Accept(this);
                _currentStringBuilder.Append($";{node.VariableDeclaration.Name} += ");
                node.Increment.Accept(this);
            }
            else
            {
                _currentStringBuilder.Append($"int _i{_forLoopCounter} = 0; _i{_forLoopCounter} < ");
                node.ToValueOperation.Accept(this);
                _currentStringBuilder.Append($";_i{_forLoopCounter} = ");
                node.ToValueOperation.Accept(this);
            }
            _currentStringBuilder.Append($") \n {{");
            VisitChildren(node);
            _currentStringBuilder.Append($"\n}}");
        }

        public override void Visit(ForeachLoopNode node)
        {
            _currentStringBuilder.Append($"foreach (");
            _currentStringBuilder.Append($"{ResolveTypeToCS(node.VariableType_enum)} {node.VariableName} in {node.InVariableName}");
            _currentStringBuilder.Append($") \n {{");
            VisitChildren(node);
            _currentStringBuilder.Append($"\n }}\n");

        }

        public override void Visit(WhileLoopNode node)
        {
            _currentStringBuilder.Append("while (");
            node.BoolCompare.Accept(this);
            _currentStringBuilder.Append(") \n {");
            VisitChildren(node);
            _currentStringBuilder.Append("\n}");

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
            if (node.Type_enum == AllType.STRING)
            {
                _currentStringBuilder.Append(node.Value);
            }
            else if (node.Type_enum == AllType.DECIMAL)
            {
                _currentStringBuilder.Append(node.Value);
                _currentStringBuilder.Append("m");
            }
            else if (node.Type_enum == AllType.BOOL || node.Type_enum == AllType.INT)
            {
                _currentStringBuilder.Append(node.Value);
            }
        }

        public override void Visit(PrintQueryNode node)
        {
            bool first = true;
            _currentStringBuilder.Append("\nConsole.WriteLine(");
            foreach (var item in node.Children)
            {
                if (first)
                {
                    if (item is ExpressionNode)
                    {
                        _currentStringBuilder.Append("(");
                        item.Accept(this);
                        _currentStringBuilder.Append(")");
                    }
                    else
                    {
                        item.Accept(this);
                    }
                    first = false;
                }
                else
                {
                    _currentStringBuilder.Append("+");
                    if (item is ExpressionNode)
                    {
                        _currentStringBuilder.Append("(");
                        item.Accept(this);
                        _currentStringBuilder.Append(")");
                    }
                    else
                    {
                        item.Accept(this);
                    }
                }
            }
            _currentStringBuilder.Append(");\n");
        }

        public override void Visit(RunQueryNode node)
        {
            _currentStringBuilder.Append("\n" + node.FunctionName + "(");
            bool first = true;
            foreach (var item in node.Children)
            {
                if (first)
                {
                    first = false;
                    item.Accept(this);
                }
                else
                {
                    _currentStringBuilder.Append(",");
                    item.Accept(this);
                }
            }
            _currentStringBuilder.Append(");\n");
        }

        StringBuilder _graphExtensions;
        StringBuilder _edgeExtensions;
        StringBuilder _vertexExtensions;

        public void ExtendClass(AllType Class, AllType ExtendType, string ExtendName, string ExtendNameShort, bool IsCollection = false)
        {
            // TODO: Default values for extended variables needs to be set
            StringBuilder _currentExtension;
            // Find out what class to extend, as they have their own extension classes.
            switch (Class)
            {
                case AllType.GRAPH:
                    _currentExtension = _graphExtensions;
                    break;
                case AllType.EDGE:
                    _currentExtension = _edgeExtensions;
                    break;
                case AllType.VERTEX:
                    _currentExtension = _vertexExtensions;
                    break;
                default:
                    throw new Exception("You are trying to extend a non-class type, which is illegal!");
            }
            // If its a collection
            if (IsCollection)
            {
                _currentExtension.AppendLine($"\npublic Collection<{ResolveTypeToCS(ExtendType)}> {ExtendName} = new Collection<{ResolveTypeToCS(ExtendType)}>();");
                if (ExtendNameShort != null && ExtendNameShort != "")
                {
                    _currentExtension.AppendLine($"\npublic Collection<{ResolveTypeToCS(ExtendType)}> {ExtendNameShort} {{ ");
                    _currentExtension.AppendLine("get");
                    _currentExtension.AppendLine($"{{return {ExtendName};}}");
                    _currentExtension.AppendLine("set");
                    _currentExtension.AppendLine($"{{{ExtendName} = value;}}");
                    _currentExtension.AppendLine("}");
                }
            }
            else // if its everything else
            {
                // Check if the extension is a class?
                // TODO: consider if this is correct
                if (ExtendType == AllType.GRAPH || ExtendType == AllType.VERTEX || ExtendType == AllType.EDGE)
                {
                    _currentExtension.AppendLine($"\npublic {ResolveTypeToCS(ExtendType)} {ExtendName} = new {ResolveTypeToCS(ExtendType)};");
                }
                else
                {
                    _currentExtension.AppendLine($"\npublic {ResolveTypeToCS(ExtendType)} {ExtendName};");
                }
                if (ExtendNameShort != null && ExtendNameShort != "")
                {
                    _currentExtension.AppendLine($"\npublic {ResolveTypeToCS(ExtendType)} {ExtendNameShort} {{ ");
                    _currentExtension.AppendLine("get");
                    _currentExtension.AppendLine($"{{return {ExtendName};}}");
                    _currentExtension.AppendLine("set");
                    _currentExtension.AppendLine($"{{{ExtendName} = value;}}");
                    _currentExtension.AppendLine("}\n");
                }
            }

        }

        public override void Visit(PredicateCall node)
        {
            _currentStringBuilder.Append($"{node.Name}(");
            VisitChildren(node);
            _currentStringBuilder.Append($")");
        }
    }
}
