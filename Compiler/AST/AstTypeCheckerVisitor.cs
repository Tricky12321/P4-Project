using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;

namespace Compiler.AST
{
	public class AstTypeCheckerVisitor : AstVisitorBase
	{
		SymTable _symbolTable;

		public AstTypeCheckerVisitor(SymTable symbolTable)
		{
			_symbolTable = symbolTable;
		}

		public void VisitChildrenNewScope(AbstractNode node, string Name)
		{
			if (node != null)
			{
				_symbolTable.OpenScope(Name);
				foreach (AbstractNode child in node.GetChildren())
				{
					child.Accept(this);
				}
				_symbolTable.CloseScope();
			}
		}

		public void VisitChildrenNewScope(AbstractNode node, BlockType Type)
		{
			if (node != null)
			{
				_symbolTable.OpenScope(Type);
				foreach (AbstractNode child in node.GetChildren())
				{
					child.Accept(this);
				}
				_symbolTable.CloseScope();
			}
		}

		private string DeclarationSetPrint(AddQueryNode node)
		{
			StringBuilder dclList = new StringBuilder();
			dclList.Append($"declaration set:(");
			foreach (AbstractNode v in node.Dcls)
			{
				dclList.Append($"{v.Name}, ");
			}
			dclList.Remove(dclList.Length - 2, 2);
			dclList.Append(")");
			return dclList.ToString();
		}

		public override void VisitChildren(AbstractNode node)
		{
			/*if (node is BoolComparisonNode boolNode && boolNode.ChildCount != 0 && boolNode.Children[0] is ExpressionNode)
            {
                foreach (var child in (node.Children[0] as ExpressionNode).ExpressionParts)
                {
                    child.Parent = node.Children[0];
                    if (child != null)
                    {
                        child.Accept(this);
                    }
                }
            }
            else
            {
                */
			foreach (AbstractNode child in node.GetChildren())
			{
				child.Parent = node.Children[0];
				if (child != null)
				{
					child.Accept(this);
				}
			}
			//}
		}

		private void checkCollectionFollowsCollection(string varName)
		{
			if (varName.Contains('.'))
			{
				string[] names = varName.Split('.');
				string lastString = "";
				for (int i = 0; i < names.Length - 1; i++)
				{
					string currentCheck = lastString + names[i];
					lastString = currentCheck + ".";
					_symbolTable.RetrieveSymbol(currentCheck, out bool isCollection, false);
					{
						if (isCollection)
						{
							_symbolTable.IllegalCollectionPath(varName);
						}
					}
				}
			}
		}


		//-----------------------------Visitor----------------------------------------------
		public override void Visit(ParameterNode node)
		{
			_symbolTable.SetCurrentNode(node);

			if (node.Type_enum == AllType.VOID)
			{
				_symbolTable.ParamerIsVoid();
			}
			VisitChildren(node);
		}

		public override void Visit(StartNode node)
		{
			VisitChildren(node);
		}

		public override void Visit(SetQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			// Hvis man sætter en attribute i en klasse, og kun det?
			if (node.InVariable != null)
			{
				AllType ClassType = _symbolTable.RetrieveSymbol(node.InVariable.Name) ?? AllType.UNKNOWNTYPE;
				if (!_symbolTable.IsClass(ClassType))
				{
					_symbolTable.InvalidTypeClass();
				}
				else
				{
					foreach (var Attribute in node.Attributes)
					{
						AllType AttributeType = _symbolTable.GetAttributeType(Attribute.Item1.Name, ClassType) ?? AllType.UNKNOWNTYPE;
						Attribute.Item3.Accept(this);
						CheckAllowedCast(AttributeType, Attribute.Item3.Type_enum);
					}
				}
			}
			else
			{
				foreach (var Attribute in node.Attributes)
				{
					AllType AttributeType = _symbolTable.RetrieveSymbol(Attribute.Item1.Name) ?? AllType.UNKNOWNTYPE;
					Attribute.Item3.Accept(this);
					CheckAllowedCast(AttributeType, Attribute.Item3.Type_enum);
				}
			}
			VisitChildren(node);
		}

		public override void Visit(ExtendNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
		}

		public override void Visit(PredicateNode node)
		{
			_symbolTable.SetCurrentNode(node);
			_symbolTable.OpenScope(node.Name);

			VisitChildren(node);

			_symbolTable.CloseScope();
		}

		public override void Visit(ExtractMaxQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.Variable);
			AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
			AllType? typeAttribute = AllType.UNKNOWNTYPE;
			if (isCollectionInQuery)
			{
				if (node.Attribute != null)
				{
					if (collectionNameType != null && collectionNameType != AllType.INT && collectionNameType != AllType.DECIMAL)
					{
						if (_symbolTable.IsExtended(node.Attribute, collectionNameType ?? default(AllType)))
						{
							typeAttribute = _symbolTable.GetAttributeType(node.Attribute, collectionNameType ?? default(AllType));
							if (typeAttribute == AllType.DECIMAL || typeAttribute == AllType.INT)
							{
								//variable is a collection, which are different from decimal and int collections, 
								//an attribute is specified, and are of type int or decimal, which ir MUST be!
								node.Type = collectionNameType.ToString();
								node.Name = node.Variable;
							}
							else
							{
								//attribute is other than int or decimal, which it may not be.
								_symbolTable.AttributeIllegal();
							}
						}
						else
						{
							//the class is not extended with given attribute
							_symbolTable.AttributeNotExtendedOnClass(node.Attribute, collectionNameType);
						}
					}
					else
					{
						//the collection type is either int, decimal or null. which are not legal.
						_symbolTable.ExtractCollNotIntOrDeciError();
						//SKAL FINDE UD AF OM DEN ERROR I SYMTABLE ER KORREKT AT BRUGE ET ELLER ANDET STED HER
					}
				}
				else
				{
					if (collectionNameType == AllType.DECIMAL || collectionNameType == AllType.INT)
					{
						//the attribute is proveded, there the collection must be of type decimal or interger.
						//Because it will sort on the values of the items in the decimal or integer collections,
						//and not on some attribute extended on the classes.
						node.Type = collectionNameType.ToString();
					}
					else
					{
						_symbolTable.NoAttriProvidedCollNeedsToBeIntOrDecimalError();
					}
					//attribute not specified - coll needs to be int or deci
				}
			}
			else
			{
				//the from variable needs to be a collection. You cannot retrieve something from a variable - only retrieve from a collections.
				_symbolTable.FromVarIsNotCollError(node.Variable);
			}

			if (isCollectionInQuery)
			{
				if (node.Parent is ExpressionNode expNode)
				{
					expNode.OverAllType = collectionNameType;
				}
				node.Type = collectionNameType.ToString();
			}

			if (node.WhereCondition != null)
			{
				node.WhereCondition.Accept(this);
			}
		}

		public override void Visit(ExtractMinQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.Variable);
			AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
			if (isCollectionInQuery)
			{
				node.Type = collectionNameType.ToString().ToLower();
			}
			else
			{
				_symbolTable.NotCollection(node.Variable);
			}
			if (node.WhereCondition != null)
			{
				node.WhereCondition.Accept(this);
			}
		}

		public override void Visit(SelectAllQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.Variable);
			AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
			bool fromIsColl = isCollectionInQuery;
			if (fromIsColl)
			{
				if (node.Parent is DeclarationNode dclNode)
				{
					node.Type = collectionNameType.ToString();
					node.Name = node.Variable;
				}
				else if (node.Parent is ExpressionNode expNode)
				{
					node.Type = collectionNameType.ToString();
					expNode.QueryName = node.Variable;
					expNode.OverAllType = collectionNameType;
					expNode.Name = node.Variable;
				}
			}
			else
			{
				_symbolTable.FromVarIsNotCollError(node.Variable);
			}
			if (node.WhereCondition != null)
			{
				node.WhereCondition.Accept(this);
			}
		}

		public override void Visit(SelectQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.Variable);
			if (node.Parent != null)
			{
				AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
				bool FromIsColl = isCollectionInQuery;
				if (FromIsColl)
				{
					if (node.Parent is ExpressionNode expNode)
					{
						expNode.OverAllType = collectionNameType;
						expNode.Name = node.Variable;
					}
					node.Type = collectionNameType.ToString();
					node.Name = node.Variable;
				}
				else
				{
					_symbolTable.FromVarIsNotCollError(node.Variable);
				}

				if (node.WhereCondition != null)
				{
					node.WhereCondition.Accept(this);
				}
			}
		}

		public override void Visit(PushQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.VariableCollection);
			AllType? collectionToAddTo = _symbolTable.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);
			node.Type = collectionToAddTo.ToString().ToLower();
			node.VariableToAdd.Accept(this);
			CheckAllowedCast(node.Type_enum, node.VariableToAdd.Type_enum);
		}

		public override void Visit(PopQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.Variable);
			AllType CollectionType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollection) ?? AllType.UNKNOWNTYPE;
			if (!isCollection) {
				_symbolTable.ExpectedCollection();
				return;
			}
			node.Type = CollectionType.ToString().ToLower();
		}

		public override void Visit(EnqueueQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			AllType? collectionToAddTo = _symbolTable.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);
			node.Type = collectionToAddTo.ToString();
			node.VariableToAdd.Accept(this);
			CheckAllowedCast(node.Type_enum, node.VariableToAdd.Type_enum);
		}

		public override void Visit(DequeueQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
            AllType CollectionType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollection) ?? AllType.UNKNOWNTYPE;
            if (!isCollection)
            {
                _symbolTable.ExpectedCollection();
                return;
            }
            node.Type = CollectionType.ToString().ToLower();
		}

		public override void Visit(AddQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			checkCollectionFollowsCollection(node.ToVariable);
			if (node.IsGraph)
			{//control statement for input to graphs
				AllType? TypeOfTargetCollection = _symbolTable.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
				node.Type = TypeOfTargetCollection.ToString();
				bool IsGraphVertexCollection = TypeOfTargetCollection == AllType.VERTEX && isCollectionTargetColl;
				bool isGraphEdgeCollection = TypeOfTargetCollection == AllType.EDGE && isCollectionTargetColl;
				bool isPreDefVerOrEdgeCollInGraph = TypeOfTargetCollection == AllType.GRAPH;
				if (isPreDefVerOrEdgeCollInGraph)
				{//if declarations are added to the graph.
					foreach (AbstractNode edgeOrVertexdcl in node.Dcls)
					{
						edgeOrVertexdcl.Accept(this);
					}
				}
				else if (IsGraphVertexCollection || isGraphEdgeCollection)
				{//if declarations is added to an extended collection on graph - NOT LEGAL
					foreach (AbstractNode vertexOrEdgedcl in node.Dcls)
					{
						if (vertexOrEdgedcl is GraphDeclVertexNode || vertexOrEdgedcl is GraphDeclEdgeNode)
						{
							_symbolTable.DeclarationsCantBeAdded(DeclarationSetPrint(node), node.ToVariable);
							break;
						}
					}
				}
				else
				{
					Console.WriteLine("should not be possible to reach this. IsGraph bool can only be true if its declarations for a graph"
						+ "or vertices/edges added to the graphs original attributes:   .Vertices and .Edges");
				}
			}
			//if the ToVariable is a collection:
			else if (node.IsColl)
			{
				AllType? TypeOfTargetCollection = _symbolTable.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
				node.Type = TypeOfTargetCollection.ToString();
				AllType? typeOfVar = null;

				foreach (var item in node.TypeOrVariable)
				{
					item.Accept(this);

					AbstractNode expressionToAdd = item;
					if ((expressionToAdd is BoolComparisonNode) && expressionToAdd.Children[0] is ExpressionNode)
					{
						typeOfVar = (expressionToAdd.Children[0] as ExpressionNode).OverAllType;
					}
					else if (expressionToAdd is ExpressionNode)
					{
						typeOfVar = (expressionToAdd as ExpressionNode).OverAllType;
					}
					else
					{
						typeOfVar = expressionToAdd.Type_enum;
					}
					bool targetIsGraph = TypeOfTargetCollection == AllType.GRAPH;

					if (isCollectionTargetColl)
					{//non-declarations are added to an extended collection on graph, or simply a collection.
						if (TypeOfTargetCollection == typeOfVar)
						{
							//the expression type is correct corresponding to the type of the target collection.
						}
						else
						{//mismatch of types if the target collection is not of same type of the expression
							_symbolTable.WrongTypeError(item.Name, node.ToVariable);
						}
					}
					else if (targetIsGraph)
					{//if variables are added to the graph.
						bool varIsVertex = typeOfVar == AllType.VERTEX;
						bool varIsEdge = typeOfVar == AllType.EDGE;
						if (varIsEdge || varIsVertex)
						{
							//only edge and vertex variables can be added to a graph.
						}
						else
						{
							_symbolTable.WrongTypeError(node.TypeOrVariable.ToString(), node.ToVariable);
						}
					}
					else
					{
						_symbolTable.TargetIsNotCollError(node.ToVariable);
					}
				}
			}
			else
			{
				Console.WriteLine("Is neither collection or Graph. This should not be possible");
			}
		}

		public override void Visit(WhereNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildrenNewScope(node, BlockType.WhereStatement);
		}

		public override void Visit(GraphDeclVertexNode node)
		{
			_symbolTable.SetCurrentNode(node);
			foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
			{
				item.Value.Parent = node;
				item.Value.Accept(this);
				AllType typeOfKey = _symbolTable.GetAttributeType(item.Key, AllType.VERTEX) ?? AllType.UNKNOWNTYPE;

				CheckAllowedCast(typeOfKey, item.Value.Type_enum);
			}
		}

		public override void Visit(GraphDeclEdgeNode node)
		{
			_symbolTable.SetCurrentNode(node);
			AllType vertexFromType = _symbolTable.RetrieveSymbol(node.VertexNameFrom, false) ?? AllType.UNKNOWNTYPE;
			AllType vertexToType = _symbolTable.RetrieveSymbol(node.VertexNameTo, false) ?? AllType.UNKNOWNTYPE;
			CheckAllowedCast(vertexFromType, vertexToType);
			foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
			{
				item.Value.Parent = node;
				item.Value.Accept(this);
				AllType? typeOfKey = _symbolTable.GetAttributeType(item.Key, AllType.EDGE);
				ExpressionNode expNode;
				if (item.Value is BoolComparisonNode)
				{
					expNode = (ExpressionNode)item.Value.Children[0];
				}
				else
				{
					expNode = item.Value as ExpressionNode;
				}
				if (expNode is ExpressionNode)
				{
					if (typeOfKey != expNode.OverAllType)
					{
						_symbolTable.TypeExpressionMismatch();
					}
				}
			}
		}

		public override void Visit(GraphNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
			foreach (AbstractNode item in node.Vertices)
			{
				item.Accept(this);
			}
			foreach (AbstractNode item in node.Edges)
			{
				item.Accept(this);
			}
		}

		public override void Visit(FunctionNode node)
		{
			_symbolTable.SetCurrentNode(node);
			foreach (AbstractNode item in node.Parameters)
			{
				item.Accept(this);
			}
			VisitChildrenNewScope(node, node.Name);
		}

		public override void Visit(AbstractNode node)
		{
			_symbolTable.SetCurrentNode(node);
			_symbolTable.NotImplementedError(node);
		}

		public override void Visit(IfElseIfElseNode node)
		{
			_symbolTable.SetCurrentNode(node);
			// If conditions
			node.IfCondition.Accept(this);
			// If codeblock
			_symbolTable.OpenScope(BlockType.IfStatement);
			foreach (var item in node.IfCodeBlock.Children)
			{
				item.Accept(this);
			}
			_symbolTable.CloseScope();
			// Elseif statements
			foreach (var item in node.ElseIfList)
			{
				item.Item1.Accept(this);
				_symbolTable.OpenScope(BlockType.ElseifStatement);
				foreach (var child in item.Item2.Children)
				{
					child.Accept(this);
				}
				_symbolTable.CloseScope();
			}
			// Else statement
			_symbolTable.OpenScope(BlockType.ElseStatement);
			if (node.ElseCodeBlock != null)
			{
				foreach (var child in node.ElseCodeBlock.Children)
				{
					child.Accept(this);
				}
			}
			_symbolTable.CloseScope();
		}

		public override void Visit(DeclarationNode node)
		{
			VisitChildren(node);
			_symbolTable.SetCurrentNode(node);
			if (node.Assignment != null)
			{
				node.Assignment.Parent = node;
				node.Assignment.Accept(this);

				VisitChildren(node);
				AllType typeOfVariable = _symbolTable.RetrieveSymbol(node.Name) ?? AllType.UNKNOWNTYPE;
				AbstractNode abNode = node.Assignment;
                AllType typeOfRetreiveVariable = _symbolTable.RetrieveSymbol(abNode.Name, out bool isCollectionRetrieve) ?? AllType.UNKNOWNTYPE;
                CheckAllowedCast(typeOfVariable, abNode.Type_enum);
                if (isCollectionRetrieve)
                {
                    _symbolTable.TargetIsNotCollError(node.Name);
                }
			}
		}

		public override void Visit(BoolComparisonNode node)
		{
			//VisitChildren(node);
			_symbolTable.SetCurrentNode(node);
			if (node.Left != null && node.Right != null)
			{
				// Check if the nodes are boolcomparisons
				if (node.Left is BoolComparisonNode && node.Right is BoolComparisonNode)
				{
					node.Left.Accept(this);
					node.LeftType = node.Left.Type_enum;
					node.Right.Accept(this);
					node.RightType = node.Right.Type_enum;
				}
				if (node.HasChildren)
				{
					// Extract the type from Left and right sides of a bool comparison

					if (node.Left is ExpressionNode)
					{
						node.LeftType = ((ExpressionNode)node.Left).OverAllType ?? default(AllType);
					}
					else if (node.Left.Children[0] is PredicateNode)
					{
						node.LeftType = _symbolTable.RetrieveSymbol(node.Left.Children[0].Name) ?? default(AllType);
					}
					else if (node.Left is BoolComparisonNode)
					{
						node.LeftType = ((ExpressionNode)node.Left.Children[0]).OverAllType ?? default(AllType);

					}

					if (node.Right is ExpressionNode)
					{
						node.RightType = ((ExpressionNode)node.Right).OverAllType ?? default(AllType);
					}
					else if (node.Right.Children[0] is PredicateNode)
					{
						node.RightType = _symbolTable.RetrieveSymbol(node.Right.Children[0].Name) ?? default(AllType);
					}
					else if (node.Right is BoolComparisonNode)
					{
						node.RightType = ((ExpressionNode)node.Right.Children[0]).OverAllType ?? default(AllType);

					}
				}
				if (node.RightType != AllType.UNKNOWNTYPE && node.LeftType != AllType.UNKNOWNTYPE)
				{
					if (node.RightType != node.LeftType)
					{
						if (!((node.RightType == AllType.INT && node.LeftType == AllType.DECIMAL) || (node.RightType == AllType.DECIMAL && node.LeftType == AllType.INT)))
						{
							_symbolTable.WrongTypeConditionError();
						}
					}
				}
			}
			else
			{
				VisitChildren(node);
			}
		}      
		public override void Visit(ExpressionNode node)
		{
			_symbolTable.SetCurrentNode(node);
			// Check if its a string everything should be cast to:
			if (node.ExpressionParts.Where(x => x.Type != null && x.Type_enum == AllType.STRING).Count() > 0)
			{
				node.OverAllType = AllType.STRING;
				foreach (var item in node.ExpressionParts)
				{
					if (item is VariableNode varNode) {
						_symbolTable.RetrieveSymbol(item.Name, out bool isCollection);
						if (isCollection) {
							_symbolTable.CollectionInExpression();
						}
					}
					if (item is OperatorNode opNode)
					{
						if (opNode.Operator != "+")
						{
							_symbolTable.IlligalOperator(opNode.Operator);
						}
					}
					else if (item is ExpressionNode)
					{
						item.Accept(this);
					}
				}
			}

			foreach (var item in node.ExpressionParts.Where(x => x is OperatorNode))
			{
				item.Accept(this);
				if (item is ExpressionNode)
				{
					item.Type = ((ExpressionNode)item).OverAllType.ToString().ToLower();
				}
				if (node.OverAllType == AllType.UNKNOWNTYPE || node.OverAllType == null)
				{
					node.OverAllType = item.Type_enum;
				}
				CheckAllowedCast(item.Type_enum, node.OverAllType ?? AllType.UNKNOWNTYPE, out node.OverAllType, true);
			}
			if (node.Type_enum == AllType.UNKNOWNTYPE)
			{
				node.Type = node.OverAllType.ToString().ToLower();
			}
		}

		public bool CheckAllowedCastExpression(AllType OriginalType, AllType NewType)
        {
            return CheckAllowedCast(OriginalType, NewType, out AllType? hidden);
        }


		public bool CheckAllowedCast(AllType OriginalType, AllType NewType)
		{
			return CheckAllowedCast(OriginalType, NewType, out AllType? hidden);
		}

		public bool CheckAllowedCast(AllType OriginalType, AllType NewType, out AllType? OverAllType, bool Expression = false)
		{
			if (OriginalType == AllType.VOID || NewType == AllType.VOID) {
				_symbolTable.DeclarationCantBeTypeVoid();
			}
			if (OriginalType == NewType)
			{
				OverAllType = OriginalType;
				return true;
			}
			if (CheckDecimalIntCast(OriginalType, NewType, Expression))
			{
				OverAllType = AllType.DECIMAL;
				return true;
			}
			// Check if any of the types is a string, that the other is not a class
			if (OriginalType == AllType.STRING || NewType == AllType.STRING)
			{
				bool Original_IsNot_Class = (_symbolTable.IsClass(OriginalType));
				bool NewType_IsNot_Class = (_symbolTable.IsClass(NewType));
				if (Original_IsNot_Class || NewType_IsNot_Class)
				{
					_symbolTable.CannotCastClass();
					OverAllType = null;
					return false;
				}
				OverAllType = AllType.STRING;
				return true;
			}
			_symbolTable.IlligalCast();
			OverAllType = null;
			return false;
		}

		private bool CheckDecimalIntCast(AllType First, AllType Second, bool Expression)
		{
			if (Expression) {
				if (First == AllType.INT && Second == AllType.DECIMAL)
                {
                    return true;
                }
			}
		    if (First == AllType.DECIMAL && Second == AllType.INT)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void Visit(ReturnNode node)
		{
			VisitChildren(node);
			_symbolTable.SetCurrentNode(node);
			AllType funcType = _symbolTable.RetrieveSymbol(node.FuncName, out bool FuncTypeCollection, false) ?? AllType.UNKNOWNTYPE;
			CheckAllowedCast(funcType, node.Children[0].Type_enum);

		}

		public override void Visit(ForLoopNode node)
		{
			AllType? varDclNodeType;
			_symbolTable.SetCurrentNode(node);
			_symbolTable.OpenScope(BlockType.ForLoop);
			VisitChildren(node);

			if (node.Increment != null && (node.VariableDeclaration != null || node.FromValueNode != null) && node.ToValueOperation != null)
			{
				node.Increment.Accept(this);
				if (node.VariableDeclaration != null)
				{
					node.VariableDeclaration.Accept(this);
				}
				if (node.FromValueNode != null)
				{
					node.FromValueNode.Accept(this);
				}
				node.ToValueOperation.Accept(this);
			}

			if (node.Increment is ExpressionNode incrementNode)
			{
				if (node.VariableDeclaration is VariableDclNode varDclNode)
				{
					varDclNodeType = _symbolTable.RetrieveSymbol(varDclNode.Name);
					if (varDclNodeType != AllType.INT || incrementNode.OverAllType != AllType.INT)
					{
						_symbolTable.WrongTypeConditionError();
					}
				}
				else
				{
					if (node.FromValueNode is ExpressionNode expNode)
					{
						if (expNode.OverAllType != AllType.INT || incrementNode.OverAllType != AllType.INT)
						{
							_symbolTable.WrongTypeConditionError();
						}
					}
					else
					{
						if (node.FromValueNode.Type_enum != AllType.INT || incrementNode.OverAllType != AllType.INT)
						{
							_symbolTable.WrongTypeConditionError();
						}
					}
				}
			}
			_symbolTable.CloseScope();
		}

		public override void Visit(ForeachLoopNode node)
		{
			_symbolTable.SetCurrentNode(node);
			AllType? collectionType = _symbolTable.RetrieveSymbol(node.InVariableName, out bool isCollectionInForeach, false);

			if (node.VariableType_enum == collectionType)
			{
				//collection type and variable type is the same.
			}
			else
			{
				_symbolTable.WrongTypeError(node.VariableName, node.InVariableName);
			}
			_symbolTable.OpenScope(BlockType.ForEachLoop);
			VisitChildren(node);
			_symbolTable.CloseScope();
		}

		public override void Visit(WhileLoopNode node)
		{
			_symbolTable.SetCurrentNode(node);
			node.BoolCompare.Accept(this);
			VisitChildrenNewScope(node, BlockType.WhileLoop);
		}

		public override void Visit(VariableAttributeNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
		}

		public override void Visit(VariableNode node)
		{
			VisitChildren(node);
			_symbolTable.SetCurrentNode(node);

			if (node.Assignment != null)
			{
				AllType? variableExpressionType = _symbolTable.RetrieveSymbol(node.Assignment.Name);
			}


			if (node.Parent is ExpressionNode expNode)
			{
				expNode.OverAllType = _symbolTable.RetrieveSymbol(node.Name, false);
			}
			node.Type = _symbolTable.RetrieveSymbol(node.Name, false).ToString();
		}

		public override void Visit(CodeBlockNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
		}

		public override void Visit(VariableDclNode node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
			if (node.Children.Count > 0) {
				CheckAllowedCast(node.Type_enum, node.Children[0].Type_enum);
            }
		}

		public override void Visit(OperatorNode node)
		{
			_symbolTable.SetCurrentNode(node);
			if (_symbolTable.IsClass(node.Left.Type_enum) && _symbolTable.IsClass(node.Right.Type_enum))
			{
				_symbolTable.ClassOperatorError();
			}
			else
			{
				node.Right.Accept(this);
				node.Left.Accept(this);
				CheckAllowedCast(node.Left.Type_enum, node.Right.Type_enum, out AllType? OverAllType, true);
				node.Type = OverAllType.ToString().ToLower();
			}
			VisitChildren(node);
		}

		public override void Visit(ConstantNode node)
		{
			_symbolTable.SetCurrentNode(node);
			if (node.Parent is ExpressionNode)
			{
				ExpressionNode parentNode = (ExpressionNode)node.Parent;
				parentNode.OverAllType = node.Type_enum;
			}
		}

		public override void Visit(PrintQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			foreach (AbstractNode abnode in node.Children)
			{
				abnode.Accept(this);
				CheckAllowedCast(AllType.STRING, abnode.Type_enum);
			}
		}

		public override void Visit(RunQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
            node.Type = _symbolTable.RetrieveSymbol(node.FunctionName).ToString().ToLower();
			bool isCollection = false;
			VisitChildren(node);
			List<FunctionParameterEntry> funcParamList = _symbolTable.GetParameterTypes(node.FunctionName);
			funcParamList.OrderBy(x => x.ID);
			int i = 0;
			AllType placeholderType = AllType.UNKNOWNTYPE;
			if (node.Parent is ExpressionNode expNode)
			{
				expNode.OverAllType = _symbolTable.RetrieveSymbol(node.FunctionName); ;
			}

			if (node.Children.Count > 0)
			{
				if (node.Children.Count <= funcParamList.Count)
				{
					foreach (AbstractNode child in node.Children)
					{
						if (child is VariableNode varNode)
						{
							placeholderType = _symbolTable.RetrieveSymbol(child.Name, out isCollection) ?? AllType.UNKNOWNTYPE;

							if (funcParamList.Count > 0)
							{
								if (placeholderType != funcParamList[i].Type && funcParamList[i].Collection == isCollection)
								{
									//type error
									_symbolTable.RunFunctionTypeError(child.Name, funcParamList[i].Name);
								}
							}
							else
							{
								//calling function with no formal parameters
								_symbolTable.RunFunctionParameterError();
							}
						}
						else if (child is ConstantNode constNode)
						{
							if (funcParamList.Count == 0)
							{
								_symbolTable.RunFunctionParameterError();
							}
							else
							{
								if (child.Type_enum != funcParamList[i].Type)
								{
									_symbolTable.RunFunctionTypeError(child.Name, funcParamList[i].Name);
									//type error
								}
							}
						}
						++i;
					}
				}
				else
				{
					_symbolTable.RunFunctionParameterError();
				}

			}
			else if (funcParamList.Count > 0)
			{
				//running function without actual parameters, when function has formal parameters
				_symbolTable.RunFunctionParameterError();
			}

		}

		public override void Visit(PredicateCall node)
		{
			_symbolTable.SetCurrentNode(node);
			VisitChildren(node);
			List<AllType> predParaTypes = _symbolTable.GetPredicateParameters(node.Name);
			int ChildCount = node.Children.Count();
			for (int i = 0; i < ChildCount; i++)
			{
				CheckAllowedCast(node.Children[i].Type_enum, predParaTypes[i]);
			}

		}

		public override void Visit(RemoveQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			var type = _symbolTable.RetrieveSymbol(node.Variable, out bool IsCollection);
			if (IsCollection != true)
			{
				_symbolTable.NotCollection(node.Variable);
			}
			if (node.WhereCondition != null)
			{
				(node.WhereCondition as WhereNode).Type = type.ToString().ToLower();
			}
		}

		public override void Visit(RemoveAllQueryNode node)
		{
			_symbolTable.SetCurrentNode(node);
			var type = _symbolTable.RetrieveSymbol(node.Variable, out bool IsCollection);
			if (IsCollection != true)
			{
				_symbolTable.NotCollection(node.Variable);
			}
			if (node.WhereCondition != null)
			{
				(node.WhereCondition as WhereNode).Type = type.ToString().ToLower();
			}
		}
	}
}
