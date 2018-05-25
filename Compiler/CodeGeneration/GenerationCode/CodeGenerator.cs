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
		private StringBuilder Global;
		public StringBuilder Functions;
		public StringBuilder GlobalSet;
		private int _forLoopCounter = 0;
		private int _newVariableCounter_private = 0;
		private string _boolComparisonPrefix = "";
		private string _newVariabelCounter
		{
			get
			{
				return "_newVariable" + _newVariableCounter_private++;
			}
		}

		private string HandleCSharpKeywords(string VariableName)
		{
			return "_name" + VariableName.Replace(".", "._name");
		}

		public CodeGenerator(CodeWriter codeWriter)
		{
			MainBody = codeWriter.MainBody;
			Functions = codeWriter.Functions;
			GlobalSet = codeWriter.GlobalSet;
			Global = codeWriter.Global;
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
				_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
				if (node.Assignment != null)
				{
					_currentStringBuilder.Append(" = ");
					node.Assignment.Accept(this);
					_currentStringBuilder.Append(";\n");
				}
				else
				{
					_currentStringBuilder.Append($" = new Collection<{ResolveTypeToCS(node.Type_enum)}>()");
				}
			}
			else
			{

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

				VisitChildren(node);

			}
			else
			{
				_currentStringBuilder = Functions;
				_currentStringBuilder.Append("\n");

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

				_currentStringBuilder.Append("{\n");

				VisitChildren(node);

				_currentStringBuilder.Append("\n");

				_currentStringBuilder.Append("}");
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
			if (node.IsCollection)
			{
				_currentStringBuilder.Append("Collection<");
				_currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum));
				_currentStringBuilder.Append("> ");
			}
			else
			{
				_currentStringBuilder.Append(ResolveTypeToCS(node.Type_enum));
			}
			_currentStringBuilder.Append(" " + HandleCSharpKeywords(node.Name));
		}

		public override void Visit(StartNode node)
		{
			VisitChildren(node);
		}

		public override void Visit(GraphNode node)
		{

			StringBuilder OldStringbuilder = _currentStringBuilder;
			if (node.Global)
			{
				_currentStringBuilder = Global;
				_currentStringBuilder.Append("\n");

				_currentStringBuilder.Append("public static ");
				_currentStringBuilder.Append($"Graph {HandleCSharpKeywords(node.Name)} = new Graph();\n\n");
			}
			else
			{
				_currentStringBuilder.Append("\n");

				_currentStringBuilder.Append($"Graph {HandleCSharpKeywords(node.Name)} = new Graph();\n\n");

			}
			if (node.Global)
			{
				_currentStringBuilder = Global;

				_currentStringBuilder.Append($"public static Vertex _newVertex{HandleCSharpKeywords(node.Name)};\n");
			}
			else
			{

				_currentStringBuilder.Append($"Vertex _newVertex{HandleCSharpKeywords(node.Name)};\n");

			}
			_currentStringBuilder = OldStringbuilder;
			foreach (GraphDeclVertexNode vertex in node.Vertices)
			{
				string vertexName;
				if (vertex.Name == null)
				{
					vertexName = $"_newVertex{HandleCSharpKeywords(node.Name)}";

					if (node.Global)
					{
						_currentStringBuilder = Global;
						_currentStringBuilder.Append($"public static {vertexName} = new Vertex();\n");
					}
					else
					{
						_currentStringBuilder.Append($"{vertexName} = new Vertex();\n");
					}
					_currentStringBuilder = OldStringbuilder;
				}
				else
				{
					vertexName = HandleCSharpKeywords(vertex.Name);
					if (node.Global)
					{
						_currentStringBuilder = Global;

						_currentStringBuilder.Append($"public static Vertex {vertexName} = new Vertex();\n");
					}
					else
					{

						_currentStringBuilder.Append($"Vertex {vertexName} = new Vertex();\n");
					}
					_currentStringBuilder = OldStringbuilder;
				}

				foreach (KeyValuePair<string, AbstractNode> value in vertex.ValueList)
				{
					if (node.Global)
					{
						_currentStringBuilder = GlobalSet;
					}

					_currentStringBuilder.Append($"{vertexName}.{HandleCSharpKeywords(value.Key)} = ");
					value.Value.Accept(this);
					_currentStringBuilder.Append($";\n");
					_currentStringBuilder = OldStringbuilder;

				}
				if (node.Global)
				{
					_currentStringBuilder = GlobalSet;
				}

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}._nameVertices.Add({vertexName});\n\n");
				_currentStringBuilder = OldStringbuilder;
			}
			if (node.Global)
			{
				_currentStringBuilder = Global;

				_currentStringBuilder.Append($"public static Edge _newEdge{HandleCSharpKeywords(node.Name)};\n");
			}
			else
			{

				_currentStringBuilder.Append($"Edge _newEdge{HandleCSharpKeywords(node.Name)};\n");
			}
			_currentStringBuilder = OldStringbuilder;
			foreach (GraphDeclEdgeNode edge in node.Edges)
			{
				string edgeName;
				if (edge.Name == null)
				{
					edgeName = $"_newEdge{HandleCSharpKeywords(node.Name)}";
					if (node.Global)
					{
						_currentStringBuilder = Global;

						_currentStringBuilder.Append($"public static {edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");

					}
					else
					{

						_currentStringBuilder.Append($"{edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");
					}
					_currentStringBuilder = OldStringbuilder;
				}
				else
				{
					edgeName = HandleCSharpKeywords(edge.Name);
					if (node.Global)
					{
						_currentStringBuilder = Global;

						_currentStringBuilder.Append($"public static Edge {edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");

					}
					else
					{

						_currentStringBuilder.Append($"Edge {edgeName} = new Edge({HandleCSharpKeywords(edge.VertexNameFrom)},{HandleCSharpKeywords(edge.VertexNameTo)});\n");

					}
					_currentStringBuilder = OldStringbuilder;
				}

				foreach (KeyValuePair<string, AbstractNode> value in edge.ValueList)
				{
					if (node.Global)
					{
						_currentStringBuilder = GlobalSet;
					}

					_currentStringBuilder.Append($"{edgeName}.{HandleCSharpKeywords(value.Key)} = ");
					value.Value.Accept(this);
					_currentStringBuilder.Append($";\n");
					_currentStringBuilder = OldStringbuilder;
				}
				if (node.Global)
				{
					_currentStringBuilder = GlobalSet;
				}

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Name)}._nameEdges.Add({edgeName});\n\n");
				_currentStringBuilder = OldStringbuilder;
			}
		}

		public override void Visit(VariableDclNode node)
		{
			var OldStringbuilder = _currentStringBuilder;
			if (node.Global)
			{
				_currentStringBuilder = Global;
			}
			_currentStringBuilder.Append("\n");

			if (node.Global)
			{
				_currentStringBuilder.Append("public static ");
			}

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
			_currentStringBuilder = OldStringbuilder;
		}

		public override void Visit(GraphDeclVertexNode node)
		{
			throw new Exception("This should be done in the GraphDeclNode");
		}

		public override void Visit(GraphDeclEdgeNode node)
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

						_currentStringBuilder.Append($"foreach (var {HandleCSharpKeywords("val")} in {HandleCSharpKeywords(InVaraible)}) \n");

						_currentStringBuilder.Append($"{{\n");

						//Foreach body
						if (node.WhereCondition != null)
						{
							_currentStringBuilder.Append($"if (");
							node.WhereCondition.Accept(this);
							_currentStringBuilder.Append($")\n {{\n");

							// IfBody


							_currentStringBuilder.Append($"{HandleCSharpKeywords("val")}.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
							expression.Accept(this);
							_currentStringBuilder.Append($";\n }}");


						}
						else
						{

							_currentStringBuilder.Append($"{HandleCSharpKeywords("val")}.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
							expression.Accept(this);
							_currentStringBuilder.Append($";\n");

						}

						_currentStringBuilder.Append($"\n }}\n");


					}
					else
					{

						_currentStringBuilder.Append($"{HandleCSharpKeywords(InVaraible)}.{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
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


					_currentStringBuilder.Append($"{HandleCSharpKeywords(VariableName)} {AssignOperator} ");
					expression.Accept(this);
					_currentStringBuilder.Append($";\n");
				}
			}
		}

		public override void Visit(SelectAllQueryNode node)
		{
			_currentStringBuilder.Append($"_funSelectAllQuery{node.ID}_{functionID}();\nCollection<{ResolveTypeToCS(node.Type_enum)}> _funSelectAllQuery{node.ID}_{functionID++}(){{\n");
			_currentStringBuilder.Append($"Collection<{ResolveTypeToCS(node.Type_enum)}> _col{node.ID} = new Collection<{ResolveTypeToCS(node.Type_enum)}>();\n");

			if (node.Type == "void")
			{
				throw new NotImplementedException("Typen skal gerne sættes i typechecker.");
			}
			//_currentStringBuilder.Append($"{ResolveTypeToCS(node.Type_enum)} _val{node.ID};\n");

			_currentStringBuilder.Append($"foreach (var {HandleCSharpKeywords("val")} in {HandleCSharpKeywords(node.Variable)}){{\n");
			if (node.WhereCondition != null)
			{
				_currentStringBuilder.Append("if (");
				_boolComparisonPrefix = $"{HandleCSharpKeywords("val")}.";
				node.WhereCondition.Children[0].Accept(this);
				_boolComparisonPrefix = "";
				_currentStringBuilder.Append("){\n");
			}
			_currentStringBuilder.Append($"_col{node.ID}.Add({HandleCSharpKeywords("val")});\n}}\n");
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
			_currentStringBuilder.Append($"_funSelectQuery{node.ID}_{functionID.ToString()}();\n{ResolveTypeToCS(node.Type_enum)} _funSelectQuery{node.ID}_{functionID++}(){{\n");
			if (node.Type == "void")
			{
				throw new NotImplementedException("Typen skal gerne sættes i typechecker.");
			}
			_currentStringBuilder.Append($"{ResolveTypeToCS(node.Type_enum)} _val{node.ID} = default({ResolveTypeToCS(node.Type_enum)});\n");

			_currentStringBuilder.Append($"foreach (var {HandleCSharpKeywords("val")} in {HandleCSharpKeywords(node.Variable)}){{\n");
			if (node.WhereCondition != null)
			{
				_currentStringBuilder.Append("if (");
				_boolComparisonPrefix = "val.";
				node.WhereCondition.Children[0].Accept(this);
				_boolComparisonPrefix = "";
				_currentStringBuilder.Append("){\n");
			}
			_currentStringBuilder.Append($"_val{node.ID} = {HandleCSharpKeywords("val")};\n break;\n}}\n");
			if (node.WhereCondition != null)
			{
				_currentStringBuilder.Append("}\n");
			}
			_currentStringBuilder.Append($"return _val{node.ID};\n}}\n\n");
		}

		public override void Visit(ExtractMaxQueryNode node)
		{
            GetExtractString(node, true);
        }

        public override void Visit(ExtractMinQueryNode node)
		{
			GetExtractString(node, false);
		}

		private void GetExtractString(AbstractExtractNode node, bool maxIfTrue)
		{
			string placeFuncString = maxIfTrue ? "_funExtMax" : "_funExtMin";
			string placeValString = $"_val{node.ID}";
			string placeAttriString = node.Attribute == null ? "" : $".{node.Attribute.Replace("'", "")}";
			string boolOpString = maxIfTrue ? ">" : "<";
            _currentStringBuilder.Append($"{placeFuncString}{node.ID}_{functionID}();" +
			                     $"\n{ResolveTypeToCS(node.Type_enum)} {placeFuncString}{node.ID}_{functionID++}(){{\n");
            string maxOrMinDec = maxIfTrue ? "decimal.MinValue;" : "decimal.MaxValue;";
            _currentStringBuilder.Append($"{ResolveTypeToCS(node.Type_enum)} {placeValString} = {maxOrMinDec}\nint _variablei = 0;\n");
            _currentStringBuilder.Append($"foreach (var {HandleCSharpKeywords("val")} in {HandleCSharpKeywords(node.Variable)}){{\n");

            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append($"if(");
                node.WhereCondition.Accept(this);
                _currentStringBuilder.Append($")\n{{");
            }
            _currentStringBuilder.Append($"if({HandleCSharpKeywords("val")}{placeAttriString} {boolOpString} {placeValString}){{\n");

            _currentStringBuilder.Append($"{placeValString} = {HandleCSharpKeywords("val")};\n_variablei++;}}\n}}\n");
            if (node.WhereCondition != null)
            {
                _currentStringBuilder.Append($"}}");
            }
            _currentStringBuilder.Append($"if(_variablei == 0){{throw new NullReferenceException(\"No element to extract\");}}");
            _currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Remove({placeValString});\n");
            _currentStringBuilder.Append($"return {placeValString};\n}}\n\n");
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
			_currentStringBuilder.Append("\n if(");
			node.IfCondition.Accept(this);
			_currentStringBuilder.Append(") \n {\n");
			node.IfCodeBlock.Accept(this);
			_currentStringBuilder.Append("\n } \n");
			foreach (var item in node.ElseIfList)
			{
				_currentStringBuilder.Append("\n else if (");
				item.Item1.Accept(this); // BoolComparison
				_currentStringBuilder.Append(") \n {");
				item.Item2.Accept(this); // Codeblock
				_currentStringBuilder.Append("\n }\n");
			}
			if (node.ElseCodeBlock != null)
			{
				_currentStringBuilder.Append("\n else {");
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

				bool Parentheses = item is ExpressionNode expNode && expNode.hasparentheses;
				if (Parentheses)
				{
					_currentStringBuilder.Append("(");
				}
				item.Accept(this);

				if (Parentheses)
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
						string _newVariable;
						if (item.Name != null && item.Name != "")
						{
							_newVariable = item.Name;
						}
						else
						{
							_newVariable = _newVariabelCounter;
						}
						string type = item is GraphDeclEdgeNode ? "Edge" : "Vertex";

						_currentStringBuilder.Append($"{type} {HandleCSharpKeywords(_newVariable)} = new {type}();\n");

						foreach (var val in ((GraphDeclVertexNode)item).ValueList)
						{
							_currentStringBuilder.Append($"{HandleCSharpKeywords(_newVariable)}.{HandleCSharpKeywords(val.Key)} = ");
							val.Value.Accept(this);
							_currentStringBuilder.Append($";\n");
						}
						if (item is GraphDeclEdgeNode)
						{

							_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}._nameEdges.Push({HandleCSharpKeywords(_newVariable)});\n");
						}
						else
						{

							_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}._nameVertices.Push({HandleCSharpKeywords(_newVariable)});\n");
						}
					}
				}
			}
			else
			{
				foreach (var item in node.TypeOrVariable)
				{

					_currentStringBuilder.Append($"{HandleCSharpKeywords(node.ToVariable)}.Push(");
					item.Accept(this);
					_currentStringBuilder.Append($");\n");
				}
			}
		}

		public override void Visit(PredicateNode node)
		{
			_currentStringBuilder.Append($"\n");

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


			_currentStringBuilder.Append($"return ");
			VisitChildren(node);
			_currentStringBuilder.Append($"; \n");


			_currentStringBuilder.Append($"}}\n");

		}

		public override void Visit(DequeueQueryNode node)
		{
			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Dequeue;");
		}

		public override void Visit(EnqueueQueryNode node)
		{

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

			_currentStringBuilder.Append($"{HandleCSharpKeywords(node.VariableCollection)}.Push(");
			node.VariableToAdd.Accept(this);
			_currentStringBuilder.Append($");\n");
		}

		public override void Visit(ForLoopNode node)
		{
			_currentStringBuilder.Append("\n");

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

			_currentStringBuilder.Append($"{{");

			VisitChildren(node);

			_currentStringBuilder.Append($"\n");

			_currentStringBuilder.Append($"}}");
		}

		public override void Visit(ForeachLoopNode node)
		{

			_currentStringBuilder.Append($"foreach (");
			_currentStringBuilder.Append($"{ResolveTypeToCS(node.VariableType_enum)} {HandleCSharpKeywords(node.VariableName)} in {HandleCSharpKeywords(node.InVariableName)}");
			_currentStringBuilder.Append($") \n");

			_currentStringBuilder.Append($"{{");

			VisitChildren(node);

			_currentStringBuilder.Append($"\n");


			_currentStringBuilder.Append($"}}\n");

		}

		public override void Visit(WhileLoopNode node)
		{

			_currentStringBuilder.Append("while (");
			node.BoolCompare.Accept(this);
			_currentStringBuilder.Append(")\n");

			_currentStringBuilder.Append("{");
			VisitChildren(node);
			_currentStringBuilder.Append("\n");

			_currentStringBuilder.Append("}\n");

		}

		public override void Visit(VariableAttributeNode node)
		{
			_currentStringBuilder.Append($"{_boolComparisonPrefix}");
			_currentStringBuilder.Append($"{node.Name.Trim('\'')}");
		}

		public override void Visit(VariableNode node)
		{
			if ((node.Type_enum == AllType.EDGE || node.Type_enum == AllType.VERTEX) && node.IsCollection == false)
			{
				_currentStringBuilder.Append(HandleCSharpKeywords(node.Name) + ".Get()");
			}
			else
			{
				_currentStringBuilder.Append(HandleCSharpKeywords(node.Name));
			}
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
			if (node.Type_enum == AllType.STRING || node.Type_enum == AllType.BOOL || node.Type_enum == AllType.INT)
			{
				_currentStringBuilder.Append(node.Value);
			}
			else if (node.Type_enum == AllType.DECIMAL)
			{
				_currentStringBuilder.Append(node.Value);
			}
		}

		public override void Visit(PrintQueryNode node)
		{
			bool first = true;
			_currentStringBuilder.Append("\n");

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
				_currentExtension.Append($"public Collection<{ResolveTypeToCS(ExtendType)}> {HandleCSharpKeywords(ExtendName)} = new Collection<{ResolveTypeToCS(ExtendType)}>();\n");
				if (ExtendNameShort != null && ExtendNameShort != "")
				{
					_currentExtension.Append("\n");
					_currentExtension.Append($"public Collection<{ResolveTypeToCS(ExtendType)}> {HandleCSharpKeywords(ExtendNameShort)} {{ \n");

					_currentExtension.Append("get\n");
					_currentExtension.Append($"{{\n");

					_currentExtension.Append($"return { HandleCSharpKeywords(ExtendName)};\n");

					_currentExtension.Append($"}}\n");
					_currentExtension.Append("set \n");
					_currentExtension.Append($"{{\n");

					_currentExtension.Append($"{HandleCSharpKeywords(ExtendName)} = value;\n");

					_currentExtension.Append($"}}\n");

					_currentExtension.Append("}\n");
				}
			}
			else // if its everything else
			{
				// Check if its a disposeable class
				string OriginalName = $"_ORIGINAL_{HandleCSharpKeywords(ExtendName)}";

				if (ExtendType == AllType.GRAPH)
				{
					_currentExtension.Append("\n");
					_currentExtension.Append($"private {ResolveTypeToCS(ExtendType)} {OriginalName} = new {ResolveTypeToCS(ExtendType)}();\n");
				}
				else
				{

					_currentExtension.Append($"public {ResolveTypeToCS(ExtendType)} {OriginalName};\n");
				}
				ExtendWithGetter(ExtendType, _currentExtension, ExtendName, OriginalName);
				if (ExtendNameShort != null && ExtendNameShort != "")
				{
					ExtendWithGetter(ExtendType, _currentExtension, ExtendNameShort, OriginalName);
				}
			}

		}

		private void ExtendWithGetter(AllType ExtendType, StringBuilder _currentExtension, string ExtendName, string OriginalName)
		{
			_currentExtension.Append("\n");
			_currentExtension.Append($"public {ResolveTypeToCS(ExtendType)} {HandleCSharpKeywords(ExtendName)} {{ \n");
			_currentExtension.Append("get\n");
			_currentExtension.Append($"{{\n Update(); \n");
			_currentExtension.Append($"if (disposed) {{  Console.WriteLine(\"You are trying to reference an object which no longer exists\"); Environment.Exit(0); }}\n");
			_currentExtension.Append($"return {(OriginalName)};\n");
			_currentExtension.Append($"}}\n");
			_currentExtension.Append("set\n");
			_currentExtension.Append($"{{\nUpdate(); \n");
			_currentExtension.Append($"if (disposed) {{  Console.WriteLine(\"You are trying to reference an object which no longer exists\"); Environment.Exit(0); }}\n");
			_currentExtension.Append($"{(OriginalName)} = value;\n");
			_currentExtension.Append($"}}\n");
			_currentExtension.Append("}\n");
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

		public override void Visit(RemoveAllQueryNode node)
		{
			_currentStringBuilder.Append("\n");
			if (node.WhereCondition != null)
			{

				_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(node.Variable)})\n");

				_currentStringBuilder.Append($"{{\n");
				// ForeachBody

				// Open If
				_currentStringBuilder.Append($"if (");
				node.WhereCondition.Accept(this);
				_currentStringBuilder.Append(") \n");

				_currentStringBuilder.Append($"{{\n");

				// IfBody

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Remove(val);\n");


				// Close IfBody
				_currentStringBuilder.Append($"}}");
				_currentStringBuilder.Append($"\n");


				// Close Foreach
				_currentStringBuilder.Append($"}}");
			}
			else
			{

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.RemoveAll();\n");
			}
		}

		public override void Visit(RemoveQueryNode node)
		{
			_currentStringBuilder.Append("\n");
			if (node.WhereCondition != null)
			{

				// ForeachBody
				// Open If
				_currentStringBuilder.Append($"foreach (var val in {HandleCSharpKeywords(node.Variable)})\n {{\n if (");
				node.WhereCondition.Accept(this);
				_currentStringBuilder.Append(") \n {{\n");

				// IfBody

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.Remove(val);\n break;\n }} \n }}");

			}
			else
			{

				_currentStringBuilder.Append($"{HandleCSharpKeywords(node.Variable)}.RemoveAt(0);\n");
			}
		}

	}
}
