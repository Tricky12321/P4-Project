using System;
using Compiler.AST;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Compiler.AST.SymbolTable;

namespace Compiler.CodeGeneration.GenerationCode
{
    public class CodeGenerator : AstVisitorBase
    {
        private static int functionID = 0;
        public StringBuilder MainBody;
        private StringBuilder _currentStringBuilder;
        private StringBuilder Global = new StringBuilder();
        public StringBuilder Functions;
        private int _forLoopCounter = 0;
        private int _newVariableCounter_private = 0;
        private int _tabCount = 2;
        private string _boolComparisonPrefix = "";
        private string _newVariabelCounter
        {
            get
            {
                return "_newVariable" + _newVariableCounter_private++;
            }
        }

		private string HandleCSharpKeywords(string VariableName) {
			return "_name" + VariableName;
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
                Indent();
                _currentStringBuilder.Append($"Collection<{ResolveTypeToCS(node.Type_enum)}> ");
				_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
                if (node.Assignment != null)
                {
                    _currentStringBuilder.Append("= ");
                    node.Assignment.Accept(this);
                }
                else
                {
                    _currentStringBuilder.Append($" = new Collection<{ResolveTypeToCS(node.Type_enum)}>()");
                }
            }
            else
            {
                Indent();
                _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum) + " ");
				_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
                if (node.Assignment != null)
                {
                    _currentStringBuilder.Append(" = ");
                    node.Assignment.Accept(this);
                    _currentStringBuilder.Append(";");

                }
                else if (node.Children.Count > 0)
                {
                    _currentStringBuilder.Append(" = ");
                    foreach (var item in node.Children)
                    {
                        item.Accept(this);
                    }
                }
            }
            if (node.Assignment == null)
            {
                _currentStringBuilder.Append(";\n");
            }
        }

        public override void Visit(FunctionNode node)
        {
            if (node.Name == "Main")
            {
                _currentStringBuilder = MainBody;
                _tabCount++;
                VisitChildren(node);
                _tabCount--;
            }
            else
            {
                _currentStringBuilder = Functions;
                _currentStringBuilder.Append("\n");
                Indent();
                _currentStringBuilder.Append($"public static");
                if (node.IsCollection)
                {
                    _currentStringBuilder.Append($" Collection<");
                    _currentStringBuilder.Append($"{ResolveTypeToCS(Utilities.FindTypeFromString(node.ReturnType))}");
                    _currentStringBuilder.Append($">");
                }
                else
                {
                    _currentStringBuilder.Append($" {ResolveTypeToCS(Utilities.FindTypeFromString(node.ReturnType))}");

                }

				_currentStringBuilder.Append($" {HandleCSharpKeywords(node.Name)} (");
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
                _currentStringBuilder.Append(") \n");
                Indent();
                _currentStringBuilder.Append("{\n");
                _tabCount++;
                VisitChildren(node);
                _tabCount--;
                _currentStringBuilder.Append("\n");
                Indent();
                _currentStringBuilder.Append("}");
            }
            _currentStringBuilder = Functions;
        }

        public override void Visit(ReturnNode node)
        {
            Indent();
            _currentStringBuilder.Append("return ");
            node.Children[0].Accept(this);
            _currentStringBuilder.Append(";\n");
        }

        public override void Visit(ParameterNode node)
        {
            _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum));
			_currentStringBuilder.Append(" " + HandleCSharpKeywords(node.Name));
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            _currentStringBuilder.Append("\n");
            Indent();
			_currentStringBuilder.Append($"Graph {HandleCSharpKeywords(node.Name)} = new Graph();\n\n");
            Indent();
			_currentStringBuilder.Append($"Vertex _newVertex{HandleCSharpKeywords(node.Name)};\n");
            foreach (GraphDeclVertexNode vertex in node.Vertices)
            {
                string vertexName;
                if (vertex.Name == null)
                {
					vertexName = $"_newVertex{HandleCSharpKeywords(node.Name)}";
                    Indent();

                    _currentStringBuilder.Append($"{vertexName} = new Vertex();\n");
                }
                else
                {
					vertexName = HandleCSharpKeywords(vertex.Name);
                    Indent();

					_currentStringBuilder.Append($"Vertex {vertexName} = new Vertex();\n");
                }

                foreach (KeyValuePair<string, AbstractNode> value in vertex.ValueList)
                {
                    Indent();
					_currentStringBuilder.Append($"{vertexName}.{HandleCSharpKeywords(value.Key)} = ");
                    value.Value.Accept(this);
                    _currentStringBuilder.Append($";\n");

                }
                Indent();
				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}.Vertices.Add({vertexName});\n\n");
            }
            Indent();
			_currentStringBuilder.Append($"Edge _newEdge{HandleCSharpKeywords(node.Name)};\n");
            foreach (GraphDeclEdgeNode edge in node.Edges)
            {
                string edgeName;
                if (edge.Name == null)
                {
                    Indent();
					edgeName = $"_newEdge{HandleCSharpKeywords(node.Name)}";
                    Indent();

					_currentStringBuilder.Append($"{edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");
                }
                else
                {
					edgeName = HandleCSharpKeywords(edge.Name);
                    Indent();
					_currentStringBuilder.Append($"Edge {edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");
                }

                foreach (KeyValuePair<string, AbstractNode> value in edge.ValueList)
                {
                    Indent();

					_currentStringBuilder.Append($"{edgeName}.{HandleCSharpKeywords(value.Key)} = ");
                    value.Value.Accept(this);
                    _currentStringBuilder.Append($";\n");
                }
                Indent();

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}.Edges.Add({edgeName});\n\n");
            }
            Indent();
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}.Directed = {ResolveBoolean(node.Directed)};\n\n");
        }

        public override void Visit(VariableDclNode node)
        {
            _currentStringBuilder.Append("\n");

            Indent();
            _currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum) + " ");
			_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
            if (node.Children.Count > 0)
            {
                _currentStringBuilder.Append(" = ");
                foreach (var item in node.Children)
                {
                    bool tester = item is ExpressionNode expNode && expNode.hasparentheses;
                    if (tester)
                    {
                        _currentStringBuilder.Append("(");
                    }

                    item.Accept(this);
                    if (tester)
                    {
                        _currentStringBuilder.Append(")");
                    }
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
					string VariableName = item.Item1.Name.Trim('\'');
                    string AssignOperator = item.Item2;
                    bool IsCollection = (item.Item1 as VariableAttributeNode).IsCollection;
                    AbstractNode expression = item.Item3;
                    if (IsCollection)
                    {
                        Indent();
						_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(InVaraible)}) \n");
                        Indent();
                        _currentStringBuilder.Append($"{{\n");
                        _tabCount++;
                        //Foreach body
                        if (node.WhereCondition != null)
                        {
                            Indent();
                            _currentStringBuilder.Append($"if (");
                            node.WhereCondition.Accept(this);
                            _currentStringBuilder.Append($")\n");
                            Indent();
                            _currentStringBuilder.Append($"{{\n");
                            // IfBody
                            _tabCount++;
                            Indent();
							_currentStringBuilder.Append($"val.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
                            expression.Accept(this);
                            _currentStringBuilder.Append($";\n");

                            Indent();
                            _currentStringBuilder.Append($"}}");
                        }
                        else
                        {
                            Indent();
							_currentStringBuilder.Append($"val.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
                            expression.Accept(this);
                            _currentStringBuilder.Append($";\n");

                        }

                        _currentStringBuilder.Append($"\n");
                        _tabCount--;
                        Indent();
                        _currentStringBuilder.Append($"}}\n");
                    }
                    else
                    {
                        Indent();
						_currentStringBuilder.Append($"{InVaraible}.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
                        expression.Accept(this);
                        _currentStringBuilder.Append($";\n");

                    }
                }
            }
            else if (node.SetVariables)
            {
                foreach (var item in node.Attributes)
                {
                    string VariableName = item.Item1.Name;
                    string AssignOperator = item.Item2;

                    AbstractNode expression = null;
                    if (item.Item3 is BoolComparisonNode boolNode && boolNode.ChildCount != 0 && boolNode.Children[0] is ExpressionNode)
                    {
                        expression = item.Item3.Children[0];
                    }
                    else
                    {
                        expression = item.Item3;
                    }

                    Indent();
					_currentStringBuilder.Append($"{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
                    expression.Accept(this); 
                    _currentStringBuilder.Append($";\n");
                }
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            _currentStringBuilder.Append($"_funSelectAllQuery{node.ID}{functionID}();\nCollection<{ResolveTypeToCS(node.Type_enum)}> _funSelectAllQuery{node.ID}{functionID++}(){{\n");
            _currentStringBuilder.Append($"Collection<{ResolveTypeToCS(node.Type_enum)}> _col{node.ID} = new Collection<{ResolveTypeToCS(node.Type_enum)}>();\n");

            if (node.Type == "void")
            {
                throw new NotImplementedException("Typen skal gerne sættes i typechecker.");
            }
            //_currentStringBuilder.Append($"{ResolveTypeToCS(node.Type_enum)} _val{node.ID};\n");

            _currentStringBuilder.Append($"foreach (var val in {node.Variable}){{\n");
            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append("if (");
                _boolComparisonPrefix = "val.";
                node.WhereCondition.Children[0].Accept(this);
                _boolComparisonPrefix = "";
                _currentStringBuilder.Append("){\n");
            }
            _currentStringBuilder.Append($"_col{node.ID}.Add(val);\n}}\n");
            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append("}\n");
            }

            _currentStringBuilder.Append($"if (_col{ node.ID}.Count == 0){{\n");
            _currentStringBuilder.Append($"return null;\n}} else {{\n");
            _currentStringBuilder.Append($"return _col{node.ID};}}\n}}\n");
        }

        public override void Visit(SelectQueryNode node)
        {
            _currentStringBuilder.Append($"_funSelectQuery{node.ID}_{functionID.ToString()}();\n{ResolveTypeToCS(node.Type_enum)} _funSelectQuery{node.ID}{functionID++}(){{\n");
            if (node.Type == "void")
            {
                throw new NotImplementedException("Typen skal gerne sættes i typechecker.");
            }
            _currentStringBuilder.Append($"{ResolveTypeToCS(node.Type_enum)} _val{node.ID} = default({ResolveTypeToCS(node.Type_enum)});\n");

			_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(node.Variable)}){{\n");
            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append("if (");
                _boolComparisonPrefix = "val.";
                node.WhereCondition.Children[0].Accept(this);
                _boolComparisonPrefix = "";
                _currentStringBuilder.Append("){\n");
            }
            _currentStringBuilder.Append($"_val{node.ID} = val;\n break;\n}}\n");
            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append("}\n");
            }
            _currentStringBuilder.Append($"return _val{node.ID};\n}}\n\n");
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            _currentStringBuilder.Append(GetExtractString(node, true));
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            _currentStringBuilder.Append(GetExtractString(node, false));
        }

        private StringBuilder GetExtractString(AbstractExtractNode node, bool maxIfTrue)
        {
            StringBuilder extractString = new StringBuilder();

            string placeFuncString = maxIfTrue ? "_funExtMax" : "_funExtMin";
            string placeValString = $"_val{node.ID}";
            string placeAttriString = node.Attribute == null ? "" : $".{node.Attribute.Replace("'", "")}";
            string boolOpString = maxIfTrue ? ">" : "<";

			extractString.Append($"{placeFuncString}{node.ID}_{functionID}();\n{ResolveTypeToCS(node.Type_enum)} {placeFuncString}{node.ID}_{functionID++}(){{\n");

			extractString.Append($"{ResolveTypeToCS(node.Type_enum)} {placeValString} = {HandleCSharpKeywords(node.Variable)}.First();\ndouble placeDouble = {HandleCSharpKeywords(node.Variable)}.First(){placeAttriString};\n");

			extractString.Append($"foreach (var item in {HandleCSharpKeywords(node.Variable)}){{\n");

            extractString.Append($"if(item{placeAttriString} {boolOpString} placeDouble){{\n");
            extractString.Append($"{placeValString} = item;\nplaceDouble = item{placeAttriString};\n}}\n}}\n");



			extractString.Append($"{HandleCSharpKeywords(node.Variable)}.Remove({placeValString});\n");

            extractString.Append($"return {placeValString};\n}}\n\n");

            return extractString;
        }

        public override void Visit(WhereNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            ExtendClass(node.ClassToExtend_enum, node.ExtendWithType_enum, node.ExtensionName, node.ExtensionShortName, node.IsCollection);
        }

        public override void Visit(IfElseIfElseNode node)
        {
            // This is magic, dont try to learn it...
            // It works by:
            // if (boolComparison) {statement}
            // elseif (boolComparison) {statement} (unlimited times...)
            // else {statement}
            _currentStringBuilder.Append("\n");
            Indent();
            _currentStringBuilder.Append("if (");
            node.IfCondition.Accept(this);
            _currentStringBuilder.Append(") \n");
            Indent();
            _currentStringBuilder.Append("{\n");
            _tabCount++;
            node.IfCodeBlock.Accept(this);
            _tabCount--;
            _currentStringBuilder.Append("\n");
            Indent();
            _currentStringBuilder.Append("} \n");
            foreach (var item in node.ElseIfList)
            {
                _currentStringBuilder.Append("\n");
                Indent();
                _currentStringBuilder.Append("else if (");
                item.Item1.Accept(this); // BoolComparison
                _currentStringBuilder.Append(") \n");
                Indent();
                _currentStringBuilder.Append("{");
                _tabCount++;
                item.Item2.Accept(this); // Codeblock
                _tabCount--;
                _currentStringBuilder.Append("\n ");
                Indent();
                _currentStringBuilder.Append("}\n ");
            }
            if (node.ElseCodeBlock != null)
            {
                _currentStringBuilder.Append("\n");
                Indent();
                _currentStringBuilder.Append("else");
                Indent();
                _currentStringBuilder.Append("{");
                _tabCount++;
                node.ElseCodeBlock.Accept(this);
                _tabCount--;
                _currentStringBuilder.Append("\n");
                Indent();
                _currentStringBuilder.Append("} \n");
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
                node.ComparisonOperator = node.ComparisonOperator.Replace("&", "&&");
                node.ComparisonOperator = node.ComparisonOperator.Replace("|", "||");
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
                bool tester = item is ExpressionNode expNode && expNode.hasparentheses;
                if (tester)
                {
                    _currentStringBuilder.Append("(");
                }
                item.Accept(this);

                if (tester)
                {
                    _currentStringBuilder.Append(")");
                }
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
                        Indent();
                        _currentStringBuilder.Append($"{type} {_newVariable} = new {type}();\n");

                        foreach (var val in ((GraphDeclVertexNode)item).ValueList)
                        {
                            _currentStringBuilder.Append($"{_newVariable}.{val.Key} = ");
                            val.Value.Accept(this);
                            _currentStringBuilder.Append($";\n");
                        }
                        if (item is GraphDeclEdgeNode)
                        {
                            Indent();
							_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}.Edges.Push({_newVariable});\n");
                        }
                        else
                        {
                            Indent();
							_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}.Vertices.Push({_newVariable});\n");
                        }
                    }
                }
            }
            else
            {
                foreach (var item in node.TypeOrVariable)
                {
                    Indent();
					_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}.Push(");
                    item.Accept(this);
                    _currentStringBuilder.Append($");\n");
                }
            }
        }

        public override void Visit(PredicateNode node)
        {
            _currentStringBuilder.Append($"\n");
            Indent();
			_currentStringBuilder.Append($"bool {HandleCSharpKeywords(node.Name)} (");

            bool first = true;
            foreach (var item in node.Parameters)
            {
                if (first)
                {
                    item.Accept(this);
                    first = false;
                }
                else
                {
                    _currentStringBuilder.Append(",");
                    item.Accept(this);
                }
            }

            _currentStringBuilder.Append($") {{ \n ");
            _tabCount++;
            Indent();
            _currentStringBuilder.Append($"return ");
            VisitChildren(node);
            _currentStringBuilder.Append($"; \n");
            _tabCount--;
            Indent();
            _currentStringBuilder.Append($"}}\n");

        }

        public override void Visit(DequeueQueryNode node)
        {
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Dequeue;");
        }

        public override void Visit(EnqueueQueryNode node)
        {
            Indent();
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.VariableCollection)}.Enqueue(");
            node.VariableToAdd.Accept(this);
            _currentStringBuilder.Append($");\n");
        }

        public override void Visit(PopQueryNode node)
        {
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Pop;\n");

        }

        public override void Visit(PushQueryNode node)
        {
            Indent();
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.VariableCollection)}.Push(");
            node.VariableToAdd.Accept(this);
            _currentStringBuilder.Append($");\n");
        }

        public override void Visit(ForLoopNode node)
        {
            _currentStringBuilder.Append("\n");
            Indent();
            _currentStringBuilder.Append("for (");
            if (node.VariableDeclaration != null)
            {
                node.VariableDeclaration.Accept(this);
                node.ToValueOperation.Accept(this);
				_currentStringBuilder.Append($";{HandleCSharpKeywords(node.VariableDeclaration.Name)} += ");
                node.Increment.Accept(this);
            }
            else if (node.FromValueNode != null)
            {
                _currentStringBuilder.Append($"int _i{_forLoopCounter} = ");
                node.FromValueNode.Accept(this);
                _currentStringBuilder.Append($"; _i{_forLoopCounter} < ");
                node.ToValueOperation.Accept(this);
                _currentStringBuilder.Append($";_i{_forLoopCounter} += ");
                _forLoopCounter++;
                node.Increment.Accept(this);
            }
            _currentStringBuilder.Append($") \n");
            Indent();
            _currentStringBuilder.Append($"{{");
            _tabCount++;
            VisitChildren(node);
            _tabCount--;
            _currentStringBuilder.Append($"\n");
            Indent();
            _currentStringBuilder.Append($"}}");
        }

        public override void Visit(ForeachLoopNode node)
        {
            Indent();
            _currentStringBuilder.Append($"foreach (");
			_currentStringBuilder.Append($"{ResolveTypeToCS(node.VariableType_enum)} {HandleCSharpKeywords(node.VariableName)} in {HandleCSharpKeywords(node.InVariableName)}");
            _currentStringBuilder.Append($") \n");
            Indent();
            _currentStringBuilder.Append($"{{");
            _tabCount++;
            VisitChildren(node);
            Indent();
            _currentStringBuilder.Append($"\n");
            _tabCount--;
            Indent();
            _currentStringBuilder.Append($"}}\n");

        }

        public override void Visit(WhileLoopNode node)
        {
            Indent();
            _currentStringBuilder.Append("while (");
            node.BoolCompare.Accept(this);
            _currentStringBuilder.Append(")\n");
            Indent();
            _currentStringBuilder.Append("{");
            VisitChildren(node);
            _currentStringBuilder.Append("\n");
            Indent();
            _currentStringBuilder.Append("}\n");

        }

        public override void Visit(VariableAttributeNode node)
        {
            _currentStringBuilder.Append($"{_boolComparisonPrefix}");
            _currentStringBuilder.Append($"{node.Name.Trim('\'')}");
        }

        public override void Visit(VariableNode node)
        {

			_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
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
            _currentStringBuilder.Append("\n");
            Indent();
            _currentStringBuilder.Append("Console.WriteLine(");
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
            _currentStringBuilder.Append("\n");
            Indent();
			_currentStringBuilder.Append(HandleCSharpKeywords(node.FunctionName) + "(");
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
            _tabCount = 2;
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
                _currentExtension.Append("\n");
                Indent(ref _currentExtension);
				_currentExtension.Append($"public Collection<{ResolveTypeToCS(ExtendType)}> {HandleCSharpKeywords(ExtendName)} = new Collection<{ResolveTypeToCS(ExtendType)}>();\n");
                if (ExtendNameShort != null && ExtendNameShort != "")
                {
                    _currentExtension.Append("\n");
                    Indent(ref _currentExtension);
					_currentExtension.Append($"public Collection<{ResolveTypeToCS(ExtendType)}> {HandleCSharpKeywords(ExtendNameShort)} {{ \n");
                    _tabCount++;
                    Indent(ref _currentExtension);
                    _currentExtension.Append("get\n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"{{\n");
                    _tabCount++;
                    Indent(ref _currentExtension);
					_currentExtension.Append($"return { HandleCSharpKeywords(ExtendName)};\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"}}\n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append("set \n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"{{\n");
                    _tabCount++;
                    Indent(ref _currentExtension);
					_currentExtension.Append($"{HandleCSharpKeywords(ExtendName)} = value;\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"}}\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append("}\n");
                }
            }
            else // if its everything else
            {

                if (ExtendType == AllType.GRAPH)
                {
                    _currentExtension.Append("\n");
                    Indent(ref _currentExtension);
					_currentExtension.Append($"public {ResolveTypeToCS(ExtendType)} {HandleCSharpKeywords(ExtendName)} = new {ResolveTypeToCS(ExtendType)}();\n");
                }
                else
                {
                    _currentExtension.Append("\n");
                    Indent(ref _currentExtension);
					_currentExtension.Append($"public {ResolveTypeToCS(ExtendType)} {HandleCSharpKeywords(ExtendName)};\n");
                }
                if (ExtendNameShort != null && ExtendNameShort != "")
                {
                    _currentExtension.Append("\n");
                    Indent(ref _currentExtension);
					_currentExtension.Append($"public {ResolveTypeToCS(ExtendType)} {HandleCSharpKeywords(ExtendNameShort)} {{ \n");
                    _tabCount++;
                    Indent(ref _currentExtension);
                    _currentExtension.Append("get\n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"{{\n");
                    _tabCount++;
                    Indent(ref _currentExtension);
					_currentExtension.Append($"return {HandleCSharpKeywords(ExtendName)};\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"}}\n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append("set\n");
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"{{\n");
                    _tabCount++;
                    Indent(ref _currentExtension);
					_currentExtension.Append($"{HandleCSharpKeywords(ExtendName)} = value;\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append($"}}\n");
                    _tabCount--;
                    Indent(ref _currentExtension);
                    _currentExtension.Append("}\n");
                }
            }

        }

        public override void Visit(PredicateCall node)
        {
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}(");
            bool First = true;
            foreach (var item in node.Children)
            {
                if (First)
                {
                    First = false;
                    item.Accept(this);
                }
                else
                {
                    _currentStringBuilder.Append(",");
                    item.Accept(this);
                }
            }
            _currentStringBuilder.Append($")");
        }

        public void Indent(ref StringBuilder stringBuilder)
        {
            for (int i = 0; i < _tabCount; i++)
            {
                stringBuilder.Append("\t");
            }
        }

        public void Indent()
        {
            for (int i = 0; i < _tabCount; i++)
            {
                _currentStringBuilder.Append("\t");
            }
        }

        public override void Visit(RemoveAllQueryNode node)
        {
            _currentStringBuilder.Append("\n");
            if (node.WhereCondition != null)
            {
                Indent();
				_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(node.Variable)})\n");
                Indent();
                _currentStringBuilder.Append($"{{\n");
                // ForeachBody
                _tabCount++;
                // Open If
                _currentStringBuilder.Append($"if (");
                node.WhereCondition.Accept(this);
                _currentStringBuilder.Append(") \n");
                Indent();
                _currentStringBuilder.Append($"{{\n");
                _tabCount++;
                // IfBody
                Indent();
				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Remove(val);\n");
                _tabCount--;
                Indent();
                // Close IfBody
                _currentStringBuilder.Append($"}}");
                _currentStringBuilder.Append($"\n");
                _tabCount--;
                Indent();
                // Close Foreach
                _currentStringBuilder.Append($"}}");
            }
            else
            {
                Indent();
				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.RemoveAll();\n");
            }
        }

        public override void Visit(RemoveQueryNode node)
        {
            _currentStringBuilder.Append("\n");
            if (node.WhereCondition != null)
            {
                Indent();
				_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(node.Variable)})\n");
                Indent();
                _currentStringBuilder.Append($"{{\n");
                // ForeachBody
                _tabCount++;
                // Open If
                _currentStringBuilder.Append($"if (");
                node.WhereCondition.Accept(this);
                _currentStringBuilder.Append(") \n");
                Indent();
                _currentStringBuilder.Append($"{{\n");
                _tabCount++;
                // IfBody
                Indent();
				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Remove(val);\n");
                _currentStringBuilder.Append($"break;\n");
                _tabCount--;
                Indent();
                // Close IfBody
                _currentStringBuilder.Append($"}}");
                _currentStringBuilder.Append($"\n");
                _tabCount--;
                Indent();
                // Close Foreach
                _currentStringBuilder.Append($"}}");
            }
            else
            {
                Indent();
				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.RemoveAt(0);\n");
            }
        }

    }
}
