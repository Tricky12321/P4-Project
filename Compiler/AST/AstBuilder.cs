﻿using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Exceptions;
using Antlr4.Runtime;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
namespace Compiler.AST
{
	internal class AstBuilder : GiraphParserBaseVisitor<AbstractNode>
	{
		private string _funcName;
		public AbstractNode root;
		public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
		{
			root = new StartNode(context.Start.Line, context.Start.Column);
			// Program+ (Multiple Program children, atleast one)
			foreach (var child in context.children)
			{
				root.AdoptChildren(Visit(child));
			}
			root.Name = "Root";
			return root;
		}

		public override AbstractNode VisitFunctionDcl([NotNull] GiraphParser.FunctionDclContext context)
		{
			FunctionNode FNode = new FunctionNode(context.Start.Line, context.Start.Column);
			// Extract the Name of the function, and the return type
			FNode.Name = context.variable().GetText(); // Name
			_funcName = FNode.Name;
			FNode.ReturnType = context.allTypeWithColl().allType().GetText(); // Return Type
			FNode.IsCollection = context.allTypeWithColl().COLLECTION() != null;
			// Extract the parameters from the function
			if (context.formalParams() != null)
			{
				foreach (var Parameter in context.formalParams().formalParam())
				{
					var Type = Parameter.allTypeWithColl().allType().GetText();  // Parameter Type
					var Name = Parameter.variable().GetText(); // Parameter Name
					bool IsCollection = Parameter.allTypeWithColl().COLLECTION() != null;
					FNode.AddParameter(Type, Name, IsCollection, context.Start.Line, context.Start.Column);
				}
			}
			foreach (var Child in context.codeBlock().codeBlockContent())
			{
				FNode.AdoptChildren(Visit(Child.GetChild(0)));
			}
			return FNode;
		}

		public override AbstractNode VisitQuerySC([NotNull] GiraphParser.QuerySCContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitCodeBlock([NotNull] GiraphParser.CodeBlockContext context)
		{
			//VertexDclsNode VerDclsNode = new VertexDclsNode(context.Start.Line, context.Start.Column);
			CodeBlockNode CodeNode = new CodeBlockNode(context.Start.Line, context.Start.Column);
			foreach (var Child in context.codeBlockContent())
			{
				CodeNode.AdoptChildren(Visit(Child.GetChild(0)));
			}
			return CodeNode;
		}

		public override AbstractNode VisitCodeBlockContent([NotNull] GiraphParser.CodeBlockContentContext context)
		{
			throw new Exception("You should not end here, this is codeBlockContent...");
		}

		public override AbstractNode VisitGraphInitDcl([NotNull] GiraphParser.GraphInitDclContext context)
		{
			GraphNode GNode = new GraphNode(context.Start.Line, context.Start.Column);
			GNode.Name = context.variable().GetText();
			// Handle all VetexDcl's and add them to the list in the GraphNode
			foreach (var Child in context.graphDclBlock().vertexDcls())
			{
				foreach (var NestedChild in Child.vertexDcl())
				{
					GraphDeclVertexNode VNode = new GraphDeclVertexNode(context.Start.Line, context.Start.Column);
					if (NestedChild.variable() != null)
					{
						VNode.Name = NestedChild.variable().GetText();
					}
					if (NestedChild.assignment() != null)
					{
						foreach (var Attribute in NestedChild.assignment())
						{
							VNode.ValueList.Add(Attribute.variable().GetText(), Visit(Attribute.boolCompOrExp()));
						}
					}
					GNode.Vertices.Add(VNode);
				}
			}
			// Handle all edgeDcl's and add them to the list in the GraphNode
			foreach (var Child in context.graphDclBlock().edgeDcls())
			{
				foreach (var NestedChild in Child.edgeDcl())
				{
					GraphDeclEdgeNode ENode = new GraphDeclEdgeNode(context.Start.Line, context.Start.Column);
					// If there is a name for the Edge
					if (NestedChild.variable().GetLength(0) > 2)
					{
						ENode.Name = NestedChild.variable(0).GetText(); // Edge Name
						ENode.VertexNameFrom = NestedChild.variable(1).GetText(); // Vertex From
						ENode.VertexNameTo = NestedChild.variable(2).GetText(); // Vertex To
					}
					else
					{
						ENode.VertexNameFrom = NestedChild.variable(0).GetText(); // Vertex From
						ENode.VertexNameTo = NestedChild.variable(1).GetText(); // Vertex To
					}
					// Checks if there are any assignments
					if (NestedChild.assignment() != null)
					{
						foreach (var Attribute in NestedChild.assignment())
						{
							// This is in order to ignore the attributes that are without 
							if (Attribute.variable() != null)
							{
								ENode.ValueList.Add(Attribute.variable().GetText(), Visit(Attribute.boolCompOrExp()));
							}
						}
					}
					GNode.Edges.Add(ENode);
				}
			}
			return GNode;
		}

		public override AbstractNode VisitWhileLoop([NotNull] GiraphParser.WhileLoopContext context)
		{
			WhileLoopNode WhileNode = new WhileLoopNode(context.Start.Line, context.Start.Column);
			// Read the boolComparison for the while loop.
			WhileNode.BoolCompare = Visit(context.GetChild(1));
			// Read the codeblock in the whileLoop
			foreach (var Child in context.codeBlock().codeBlockContent())
			{
				WhileNode.AdoptChildren(Visit(Child.GetChild(0)));
			}

			return WhileNode;
		}

		public override AbstractNode VisitBoolCompOrExp([NotNull] GiraphParser.BoolCompOrExpContext context)
		{
			BoolComparisonNode BCompare = new BoolComparisonNode(context.Start.Line, context.Start.Column);
			BCompare.Type = "bool";
			// Checks if there is a prefix, if there is, add it to the Node
			// TODO: Se om dette virker...
			if (context.expression() != null)
			{
				AbstractNode output = Visit(context.expression());
				if (output is ExpressionNode expNode && context.rightP != null && context.leftP != null) {
					expNode.hasparentheses = true;
				}
				return output;
			}

			if (context.prefix != null)
			{
				BCompare.Prefix = context.prefix.Text;
				BCompare.AdoptChildren(Visit(context.boolCompOrExp(0)));
			}
			// Check if there are left and right "()" around the boolcomparison
			if (context.rightP != null && context.leftP != null && context.boolCompOrExp() != null)
			{
				BCompare.InsideParentheses = true;
				var output = Visit(context.boolCompOrExp(0));
				BCompare.AdoptChildren(output);
                if (output is ExpressionNode expNode) {
                    expNode.hasparentheses = true;
                        return expNode;
                }
			}
			// Checks if there is a left and right statement, because this will indicatef that the boolcomparison, has a left bool and right bool, compared by the operator.
			else if (context.right != null && context.left != null && context.boolCompOrExp() != null)
			{
				BCompare.Left = Visit(context.left);
				BCompare.Right = Visit(context.right);
				BCompare.Left.Parent = BCompare;
				BCompare.Right.Parent = BCompare;

				if (context.BOOLOPERATOR() != null)
				{
					BCompare.ComparisonOperator = context.BOOLOPERATOR().GetText();
				}
				else if (context.andOr() != null)
				{
					BCompare.ComparisonOperator = context.andOr().GetText();
				}
			}
			// A boolcomparison can end in an expression or a predicate, this is handled here. 
			else
			{
				if (context.predi != null)
				{
					BCompare.AdoptChildren(Visit(context.predi));
				}
				else if (context.exp != null)
				{
					var test = Visit(context.exp);
					BCompare.AdoptChildren(test);
				}
			}
			return BCompare;
		}

		public override AbstractNode VisitExpression([NotNull] GiraphParser.ExpressionContext context)
		{
			ExpressionNode ExpNode = new ExpressionNode(context.Start.Line, context.Start.Column);
			ExpNode.ExpressionParts = EvaluateExpression(context);
			if (ExpNode.ExpressionParts.Count == 1) {
				return ExpNode.ExpressionParts[0];
			}
			return ExpNode;
		}

		public override AbstractNode VisitVariable([NotNull] GiraphParser.VariableContext context)
		{
			VariableNode VarNode = new VariableNode(context.Start.Line, context.Start.Column);
			VarNode.Name = context.GetText();
			foreach (var child in context.children)
			{
				if (child.GetText() != ".")
				{
					VarNode.variableParts.Add(child.GetText());
				}
			}
			return VarNode;
		}

		/*public override AbstractNode VisitVariableFunc([NotNull] GiraphParser.VariableFuncContext context)
        {
            return Visit(context.GetChild(0));
        }*/

		public override AbstractNode VisitLoopDcl([NotNull] GiraphParser.LoopDclContext context)
		{

			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitQuery([NotNull] GiraphParser.QueryContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitNoReturnQuery([NotNull] GiraphParser.NoReturnQueryContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitReturnQuery([NotNull] GiraphParser.ReturnQueryContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitSetQuery([NotNull] GiraphParser.SetQueryContext context)
		{
			SetQueryNode SetNode = new SetQueryNode(context.Start.Line, context.Start.Column);
			// If its Attributes being set
			if (context.variable() != null)
			{
				SetNode.InVariable = Visit(context.variable());
				//SetNode.InVariable.Name = context.variable().GetText();
				SetNode.SetAttributes = true;
				foreach (var ExpNode in context.setExpressionAtriSim())
				{
					VariableAttributeNode attribute = Visit(ExpNode.attribute()) as VariableAttributeNode;
					attribute.ClassVariableName = SetNode.InVariable.Name; //  Only set Class Variable if its an attribute
					attribute.IsAttribute = true;
					AbstractNode expression = Visit(ExpNode.simpleBoolCompOrExp());
					SetNode.Attributes.Add(Tuple.Create(attribute, ExpNode.compoundAssign().GetText(), expression));
				}
			}
			else
			{
				// If its variables being set
				SetNode.SetVariables = true;
				foreach (var ExpNode in context.setExpressionVari())
				{
					VariableAttributeNode attribute = Visit(ExpNode.variable()) as VariableAttributeNode;
					AbstractNode expression = Visit(ExpNode.boolCompOrExp()) as AbstractNode;
					SetNode.Attributes.Add(Tuple.Create(attribute, ExpNode.compoundAssign().GetText(), expression));
				}
			}
			if (context.where() != null)
			{
				SetNode.WhereCondition = Visit(context.where());
			}
			return SetNode;
		}

		private List<AbstractNode> EvaluateExpression([NotNull] IParseTree context)
		{
			List<AbstractNode> expressionPart = new List<AbstractNode>();

			for (int i = 0; i < context.ChildCount; i++)
			{
				// If its an operator, the operator needs to know its left and right stuff
				if (context.GetChild(i) is GiraphParser.SimpleOperatorsContext || context.GetChild(i) is GiraphParser.AdvancedOperatorsContext || context.GetChild(i) is GiraphParser.OperatorContext) {
					OperatorNode OperatorNode = Visit(context.GetChild(i)) as OperatorNode;
					OperatorNode.Left = expressionPart[i - 1];
					OperatorNode.Right = Visit(context.GetChild(i + 1));
					expressionPart.Add(OperatorNode);
					expressionPart.Add(OperatorNode.Right);
					i++;
				} else {
					expressionPart.Add(Visit(context.GetChild(i)));
                }
			}
			return expressionPart;
		}

		public override AbstractNode VisitExpressionStart([NotNull] GiraphParser.ExpressionStartContext context)
		{
			ExpressionNode expNode = new ExpressionNode(context.Start.Line, context.Start.Column);
			expNode.ExpressionParts = EvaluateExpression(context);
			if (expNode.ExpressionParts.Count == 1) {
				return expNode.ExpressionParts[0];
			}
			return expNode;
		}

		public override AbstractNode VisitExpressionAdvanced([NotNull] GiraphParser.ExpressionAdvancedContext context)
		{
			ExpressionNode expNode = new ExpressionNode(context.Start.Line, context.Start.Column);
			expNode.ExpressionParts = EvaluateExpression(context);
			if (expNode.ExpressionParts.Count == 1)
            {
                return expNode.ExpressionParts[0];
            }
            return expNode;
		}

		public override AbstractNode VisitSimpleExpressionStart([NotNull] GiraphParser.SimpleExpressionStartContext context)
        {
            ExpressionNode expNode = new ExpressionNode(context.Start.Line, context.Start.Column);
            expNode.ExpressionParts = EvaluateExpression(context);
            if (expNode.ExpressionParts.Count == 1)
            {
                return expNode.ExpressionParts[0];
            }
            return expNode;
        }

        public override AbstractNode VisitSimpleExpressionAdvanced([NotNull] GiraphParser.SimpleExpressionAdvancedContext context)
        {
            ExpressionNode expNode = new ExpressionNode(context.Start.Line, context.Start.Column);
            expNode.ExpressionParts = EvaluateExpression(context);
            if (expNode.ExpressionParts.Count == 1)
            {
                return expNode.ExpressionParts[0];
            }
            return expNode;
        }



		public override AbstractNode VisitOperand([NotNull] GiraphParser.OperandContext context)
		{
			var switchString = context.GetChild(0).GetType().ToString();
			switch (switchString)
			{
				case "GiraphParser+ConstantContext":
					ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
					conNode.Type = ExpressionPartTypeFinder(context.GetChild(0).GetChild(0)).ToString();
					conNode.Value = context.GetText();
					return conNode;
				case "GiraphParser+VariableContext":
					VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
					varNode.Name = context.GetText();
					return varNode;
				case "GiraphParser+ReturnQueryContext":
					return Visit(context.GetChild(0));
				case "Antlr4.Runtime.Tree.TerminalNodeImpl":
					AbstractNode varAttNode = Visit(context.GetChild(1));

					if (context.GetChild(0).GetText().Contains('('))
					{
						(varAttNode as ExpressionNode).hasparentheses = true;
					}

					varAttNode.Name = context.GetText();
					return varAttNode;
				case "GiraphParser+AttributeContext":
				case "GiraphParser+RunFunctionContext":
					AbstractNode attNode = Visit(context.GetChild(0));
					return attNode;
			}
			//Skal returnere en constnode eller en varnode;
			throw new VisitVarOrConstWrongTypeException("Fejl i Mads' Kode igen!!");
		}

		public override AbstractNode VisitOperator([NotNull] GiraphParser.OperatorContext context)
		{
			OperatorNode opNode = new OperatorNode(context.Start.Line, context.Start.Column);
			opNode.Operator = context.GetText();
			return opNode;
		}

		public override AbstractNode VisitSimpleOperators([NotNull] GiraphParser.SimpleOperatorsContext context)
		{
			OperatorNode opNode = new OperatorNode(context.Start.Line, context.Start.Column);
            opNode.Operator = context.GetText();
            return opNode;
		}

		public override AbstractNode VisitAdvancedOperators([NotNull] GiraphParser.AdvancedOperatorsContext context)
		{
			OperatorNode opNode = new OperatorNode(context.Start.Line, context.Start.Column);
            opNode.Operator = context.GetText();
            return opNode;
		}

		/*private List<AbstractNode> VisitVarOrconstExpressionExtRecursive([NotNull] IParseTree context)
        {
            List<AbstractNode> expression = new List<AbstractNode>();

            if (context.GetType().ToString() == "GiraphParser+VariableContext")
            {
                string placeholderString = string.Empty;
                for (int i = 0; i < context.ChildCount; i++)
                {
                    placeholderString += context.GetChild(i).GetText();
                    expression.AddRange(VisitVarOrconstExpressionExtRecursive(context.GetChild(i)));
                }
                List<AbstractNode> listPlaceholder = new List<AbstractNode>();
                listPlaceholder.Add(new OperatorNode(1,1));
                return listPlaceholder;
            }


            if (context.ChildCount == 0)
            {
                List<AbstractNode> expressionPlaceholder = new List<AbstractNode>();

                expressionPlaceholder.Add(new OperatorNode(1,2));
                return expressionPlaceholder;
                //context.ToString();
            }
            else
            {
                for (int i = 0; i < context.ChildCount; i++)
                {
                    expression.AddRange(VisitVarOrconstExpressionExtRecursive(context.GetChild(i)));
                }
            }
            return expression;
        }*/

		private ExpressionPartType ExpressionPartTypeFinder(IParseTree context)
		{
			string type = context.GetType().ToString();
			switch (type)
			{
				case "GiraphParser+BoolContext":
				case "GiraphParser+SimpleBoolCompOrExpContext":
				case "GiraphParser+BoolCompOrExpContext":
					return ExpressionPartType.BOOL;
				case "GiraphParser+FloatnumContext":
					return ExpressionPartType.DECIMAL;
				case "GiraphParser+IntegerContext":
					return ExpressionPartType.INT;
				case "GiraphParser+SimpleOperatorsContext":
					return ExpressionPartType.OPERATOR;
				case "GiraphParser+StringContext":
					return ExpressionPartType.STRING;
				case "GiraphParser+AdvancedOperatorsContext":
					return ExpressionPartType.ADVANCED_OPERATOR;
				case "GiraphParser+VariableContext":
				case "GiraphParser+ObjectsContext":
					return ExpressionPartType.VARIABLE;
				case "GiraphParser+SelectContext":
				case "GiraphParser+PopOPContext":
				case "GiraphParser+PushOPContext":
				case "GiraphParser+EnqueueOPContext":
				case "GiraphPArser+DequeueOPContext":
				//case "GiraphParser+ObjectsContext":
				case "GiraphParser+WhereContext":
				case "GiraphParser+ExtractMaxOPContext":
				case "GiraphParser+ExtractMinOPContext":
				case "GiraphParser+DequeueOPContext":
					return ExpressionPartType.QUERYTYPE;
				case "GiraphParser+AttributeContext":
					return ExpressionPartType.ATTRIBUTE;
			}
			throw new WrongExpressionPartTypeFoundException($"Typen: {type} har ikke en case i typefinder!!");
		}

		public override AbstractNode VisitAttribute([NotNull] GiraphParser.AttributeContext context)
		{
			VariableAttributeNode vaNode;

			if (context.GetChild(0).ToString() == "'")
			{
				vaNode = new AttributeNode(context.Start.Line, context.Start.Column);
			}
			else
			{
				vaNode = new VariableNode(context.Start.Line, context.Start.Column);
			}
			vaNode.Name = context.GetText();

			return vaNode;
		}


		public override AbstractNode VisitVarOrConst([NotNull] GiraphParser.VarOrConstContext context)
		{
			AbstractNode exNode = new ExpressionNode(context.Start.Line, context.Start.Column);

			switch (context.GetChild(0).GetType().ToString())
			{
				case "GiraphParser+ConstantContext":
					ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
					conNode.Value = context.GetText();
					conNode.Type = getContextTypeRecurcive(context);
					return conNode;
				case "GiraphParser+VariableContext":
					VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
					varNode.Name = context.GetText();
					return varNode;
			}
			//Skal returnere en constnode eller en varnode;
			throw new VisitVarOrConstWrongTypeException("Fejl i Mads' Kode igen!!");
		}

		private string getContextTypeRecurcive(IParseTree context)
		{
			if (context.GetChild(0).GetType().ToString().Contains("TerminalNodeImpl"))
			{
				return getConstTypeFromCSTNodeContext(context.GetType().ToString());
			}

			return getContextTypeRecurcive(context.GetChild(0));
		}

		private string getConstTypeFromCSTNodeContext(string nodeType)
		{
			switch (nodeType)
			{
				case "GiraphParser+FloatnumContext":
					return "decimal";
				case "GiraphParser+IntegerContext":
					return "int";
				case "GiraphParser+BoolContext":
					return "bool";
				case "GiraphParser+StringContext":
					return "string";
			}
			throw new WrongExpressionPartTypeFoundException("Spørg Mads");
		}

		public override AbstractNode VisitWhere([NotNull] GiraphParser.WhereContext context)
		{
			WhereNode WNode = new WhereNode(context.Start.Line, context.Start.Column);
			WNode.AdoptChildren(Visit(context.simpleBoolCompOrExp()));
			/*foreach (var Child in context.boolComparisons().children)
            {
                WNode.AdoptChildren(Visit(Child));
            }*/
			return WNode;
		}

		public override AbstractNode VisitExtend([NotNull] GiraphParser.ExtendContext context)
		{
			ExtendNode ENode = new ExtendNode(context.Start.Line, context.Start.Column);
			ENode.ExtensionName = context.variable(0).GetText();
			if (context.variable().Length == 2)
			{
				ENode.ExtensionShortName = context.variable(1).GetText();
			}
			ENode.IsCollection = context.allTypeWithColl().COLLECTION() != null;
			ENode.ExtendWithType = context.allTypeWithColl().allType().GetText();
			ENode.ClassToExtend = context.objects().GetText();
			return ENode;
		}

		public override AbstractNode VisitDcls([NotNull] GiraphParser.DclsContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitSingleObjectDcl([NotNull] GiraphParser.SingleObjectDclContext context)
		{
			DeclarationNode DclNode = new DeclarationNode(context.Start.Line, context.Start.Column);
			DclNode.Type = context.objects().GetText();
			DclNode.Name = context.variable().GetText();
			if (context.boolCompOrExp() != null)
			{
				DclNode.Assignment = Visit(context.boolCompOrExp());
			}
			return DclNode;
		}

		public override AbstractNode VisitCollectionDcl([NotNull] GiraphParser.CollectionDclContext context)
		{
			DeclarationNode dclNode = new DeclarationNode(context.Start.Line, context.Start.Column);
			dclNode.CollectionDcl = true;
			dclNode.Type = context.allType().GetText();
			dclNode.Name = context.variable().GetText();
			if (context.collectionAssignment() != null)
			{
				dclNode.Assignment = Visit(context.collectionAssignment());
				dclNode.Assignment.Parent = dclNode;
			}
			return dclNode;
		}

		public override AbstractNode VisitCollectionAssignment([NotNull] GiraphParser.CollectionAssignmentContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitIfElseIfElse([NotNull] GiraphParser.IfElseIfElseContext context)
		{
			IfElseIfElseNode IfNode = new IfElseIfElseNode(context.Start.Line, context.Start.Column);
			IfNode.IfCondition = Visit(context.boolCompOrExp()) as BoolComparisonNode;
			IfNode.IfCodeBlock = Visit(context.codeBlock()) as CodeBlockNode;
			if (context.elseifCond() != null)
			{
				// Loop though all the ElseIf(s)
				foreach (var ElseIf in context.elseifCond())
				{
					// Add their conditions and codeblocks
					IfNode.ElseIfList.Add(Tuple.Create((Visit(ElseIf.boolCompOrExp()) as BoolComparisonNode), (Visit(ElseIf.codeBlock()) as CodeBlockNode)));
				}
			}

			// Else codeblock, First codeblock element, then it adopts the rest, if there are any
			if (context.elseCond() != null)
			{
				// There will never be more then one Else block, and it does not have a boolcomparison
				if (context.elseCond().codeBlock().ChildCount > 0)
				{
					IfNode.ElseCodeBlock = Visit(context.elseCond().codeBlock()) as CodeBlockNode;
				}
			}
			return IfNode;
		}

		public override AbstractNode VisitPredicate([NotNull] GiraphParser.PredicateContext context)
		{
			PredicateNode PNode = new PredicateNode(context.Start.Line, context.Start.Column);
			PNode.Name = context.variable().GetText();
			// Check if there is any parameters
			if (context.predicateParams() != null && context.predicateParams().predicateParam() != null)
			{
				// If there are any parameters, loop though all of them
				foreach (var Param in context.predicateParams().predicateParam())
				{
					string ParameterName = Param.variable().GetText();
					string ParameterType = Param.allType().GetText();
					// Add them to the paramter list
					PNode.AddParameter(ParameterType, ParameterName, context.Start.Line, context.Start.Column);
				}
			}
			// Adopt the boolcomparisons of the Predicate as children to the PNode
			PNode.AdoptChildren(Visit(context.simpleBoolCompOrExp()));
			return PNode;
		}

		public override AbstractNode VisitSelect([NotNull] GiraphParser.SelectContext context)
		{
			SelectQueryNode SelectNode = new SelectQueryNode(context.Start.Line, context.Start.Column);
			SelectNode.Type = getContextType(context.variable());
			SelectNode.Variable = context.variable().GetText();
			SelectNode.Name = SelectNode.Variable;
			if (context.where() != null)
			{
				SelectNode.WhereCondition = Visit(context.where());
			}
			return SelectNode;
		}

		public override AbstractNode VisitSelectAll([NotNull] GiraphParser.SelectAllContext context)
		{
			SelectAllQueryNode SelectNode = new SelectAllQueryNode(context.Start.Line, context.Start.Column);
			SelectNode.Type = getContextType(context.variable());
			SelectNode.Variable = context.variable().GetText();
			SelectNode.Name = SelectNode.Variable;
			if (context.where() != null)
			{
				SelectNode.WhereCondition = Visit(context.where());
			}
			return SelectNode;
		}

		private string getContextType(IParseTree context)
		{
			string switchString = context.GetType().ToString();
			switch (switchString)
			{
				case "GiraphParser+VariableContext":
					IParseTree childString = context.GetChild(0).GetChild(context.GetChild(0).ChildCount - 1);
					if (childString == null)
					{
						return "void";
					}

					switch (childString.GetText())
					{
						case "Vertices":
							return "vertex";
						case "Edges":
							return "edge";
						default:
							return "void";
					}
			}
			throw new WrongExpressionPartTypeFoundException("Spørg Mads");
		}

		public override AbstractNode VisitEnqueueOP([NotNull] GiraphParser.EnqueueOPContext context)
		{
			EnqueueQueryNode EnqueueNode = new EnqueueQueryNode(context.Start.Line, context.Start.Column);
			EnqueueNode.VariableToAdd = Visit(context.boolCompOrExp());
			EnqueueNode.VariableCollection = context.variable().GetText();
			return EnqueueNode;
		}

		public override AbstractNode VisitDequeueOP([NotNull] GiraphParser.DequeueOPContext context)
		{
			DequeueQueryNode DequeueNode = new DequeueQueryNode(context.Start.Line, context.Start.Column);
			DequeueNode.Variable = context.variable().GetText();
			DequeueNode.Name = DequeueNode.Variable;
			return DequeueNode;
		}

		public override AbstractNode VisitPopOP([NotNull] GiraphParser.PopOPContext context)
		{
			PopQueryNode PopNode = new PopQueryNode(context.Start.Line, context.Start.Column);
			PopNode.Variable = context.variable().GetText();
			PopNode.Name = PopNode.Variable;
			return PopNode;
		}

		public override AbstractNode VisitPushOP([NotNull] GiraphParser.PushOPContext context)
		{
			PushQueryNode PushNode = new PushQueryNode(context.Start.Line, context.Start.Column);
			var output = Visit(context.boolCompOrExp());
			PushNode.VariableToAdd = output;
			PushNode.VariableCollection = context.variable().GetText();
			return PushNode;
		}

		public override AbstractNode VisitExtractMinOP([NotNull] GiraphParser.ExtractMinOPContext context)
		{
			ExtractMinQueryNode ExtractQuery = new ExtractMinQueryNode(context.Start.Line, context.Start.Column);

			ExtractQuery.Variable = context.variable().GetText();
			ExtractQuery.Name = ExtractQuery.Variable;
			if (context.attribute() != null)
			{
				ExtractQuery.Attribute = context.attribute().GetText();
			}
			if (context.where() != null)
			{
				ExtractQuery.WhereCondition = Visit(context.where());
			}
			return ExtractQuery;
		}

		public override AbstractNode VisitExtractMaxOP([NotNull] GiraphParser.ExtractMaxOPContext context)
		{
			ExtractMaxQueryNode ExtractQuery = new ExtractMaxQueryNode(context.Start.Line, context.Start.Column);

			ExtractQuery.Variable = context.variable().GetText();
			ExtractQuery.Name = ExtractQuery.Variable;
			if (context.attribute() != null)
			{
				ExtractQuery.Attribute = context.attribute().GetText();
			}
			if (context.where() != null)
			{
				ExtractQuery.WhereCondition = Visit(context.where());
			}
			return ExtractQuery;
		}

		public override AbstractNode VisitDequeueOPOneLine([NotNull] GiraphParser.DequeueOPOneLineContext context)
		{
			DequeueQueryNode DequeueNode = new DequeueQueryNode(context.Start.Line, context.Start.Column);
			DequeueNode.Variable = context.variable().GetText();
			DequeueNode.Name = DequeueNode.Variable;
			return DequeueNode;
		}

		public override AbstractNode VisitComments([NotNull] GiraphParser.CommentsContext context)
		{
			return base.VisitComments(context);
		}

		public override AbstractNode VisitCommentLine([NotNull] GiraphParser.CommentLineContext context)
		{
			return base.VisitCommentLine(context);
		}

		public override AbstractNode VisitVariableDcl([NotNull] GiraphParser.VariableDclContext context)
		{
			VariableDclNode VariableNode = new VariableDclNode(context.Start.Line, context.Start.Column);
			VariableNode.Type = context.TYPE().GetText();
			VariableNode.Name = context.variable().GetText();
			if (context.EQUALS() != null)
			{
				VariableNode.AdoptChildren(Visit(context.boolCompOrExp()));
			}
			return VariableNode;
		}

		public override AbstractNode VisitReturnBlock([NotNull] GiraphParser.ReturnBlockContext context)
		{
			ReturnNode RNode = new ReturnNode(context.Start.Line, context.Start.Column);
			RNode.FuncName = _funcName;
			RNode.AdoptChildren(Visit(context.GetChild(1)));
			return RNode;
		}

		public override AbstractNode VisitForLoop([NotNull] GiraphParser.ForLoopContext context)
		{
			ForLoopNode ForLoop = new ForLoopNode(context.Start.Line, context.Start.Column);
			var contextInside = context.forCondition().forConditionInside();
			BoolComparisonNode boolComparison = new BoolComparisonNode(context.Start.Line, context.Start.Column);
			boolComparison.ComparisonOperator = "<";
			if (contextInside.forConditionStart().forConditionDcl() != null)
			{
				ForLoop.VariableDeclaration = Visit(contextInside.forConditionStart().forConditionDcl()); ;
				VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
				varNode.Name = (ForLoop.VariableDeclaration as VariableDclNode).Name;
				varNode.Type = (ForLoop.VariableDeclaration as VariableDclNode).Type;
				boolComparison.Left = varNode;
				ForLoop.ToValueOperation = boolComparison;
			}
			else if (contextInside.forConditionStart().expression() != null)
			{
				ForLoop.FromValueNode = Visit(contextInside.forConditionStart().expression());
				ForLoop.ToValueOperation = Visit(contextInside.expression(0));
			}

			boolComparison.Right = Visit(contextInside.expression(0));
			if (contextInside.expression(1) != null)
			{
				ForLoop.Increment = Visit(contextInside.expression(1));
			}
			else
			{
				ExpressionNode expNode = new ExpressionNode(context.Start.Line, context.Start.Column);
				ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
				conNode.Type = "int";
				conNode.Value = "1";
				expNode.ExpressionParts.Add(conNode);

				ForLoop.Increment = expNode;
				//ForLoop.Increment
			}

			// Visit all the children of the Codeblock associated with the ForLoop
			foreach (var Child in context.codeBlock().codeBlockContent())
			{
				// Adopt the children
				ForLoop.AdoptChildren(Visit(Child.GetChild(0)));
			}
			return ForLoop;
		}

		public override AbstractNode VisitCollReturnOps([NotNull] GiraphParser.CollReturnOpsContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitCollNoReturnOps([NotNull] GiraphParser.CollNoReturnOpsContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override AbstractNode VisitPrint([NotNull] GiraphParser.PrintContext context)
		{
			PrintQueryNode PNode = new PrintQueryNode(context.Start.Line, context.Start.Column);
			PNode.AdoptChildren(Visit(context.boolCompOrExp()));
			return PNode;
		}

		public override AbstractNode VisitForeachLoop([NotNull] GiraphParser.ForeachLoopContext context)
		{
			ForeachLoopNode ForeachNode = new ForeachLoopNode(context.Start.Line, context.Start.Column);
			ForeachNode.VariableName = context.foreachCondition().variable(0).GetText();
			ForeachNode.VariableType = context.foreachCondition().allType().GetText();
			ForeachNode.InVariableName = context.foreachCondition().variable(1).GetText();
			// Check the where condition
			if (context.where() != null && context.where().GetText().Length > 0)
			{
				ForeachNode.WhereCondition = Visit(context.where());
			}
			// Visit the children of the codeblock
			foreach (var Child in context.codeBlock().children)
			{
				if (Child.GetChild(0) != null)
				{
					ForeachNode.AdoptChildren(Visit(Child.GetChild(0)));
				}
			}
			return ForeachNode;
		}

		public override AbstractNode VisitAddQuery([NotNull] GiraphParser.AddQueryContext context)
		{
			AddQueryNode AddNode = new AddQueryNode(context.Start.Line, context.Start.Column);
			// ITS A GRAPH ADD
			if (context.addToGraph() != null)
			{
				AddNode.IsGraph = true;
				if (context.addToGraph().vertexDcls() != null)
				{
					foreach (var Child in context.addToGraph().vertexDcls().vertexDcl())
					{
						AddNode.Dcls.Add(Visit(Child));
					}
				}
				var test = context.addToGraph().edgeDcls();
				if (context.addToGraph().edgeDcls() != null)
				{
					foreach (var Child in context.addToGraph().edgeDcls().edgeDcl())
					{
						AddNode.Dcls.Add(Visit(Child));
					}
				}
				// Shared
				AddNode.ToVariable = context.addToGraph().variable().GetText();

			}
			// ITS A COLLECTION ADD
			else if (context.addToColl() != null)
			{
				AddNode.IsColl = true;
				// ITS ALL TYPE
				AddNode.TypeOrVariable.Add(Visit(context.addToColl().collExpression().boolCompOrExp()));
				if (context.addToColl().collExpressionExt() != null)
				{
					foreach (var item in context.addToColl().collExpressionExt())
					{
						AddNode.TypeOrVariable.Add(Visit(item.collExpression().boolCompOrExp()));
					}
				}
				// Shared
				if (AddNode.IsVariable)
				{
					AddNode.ToVariable = context.addToColl().variable().GetText();
				}
				else
				{
					AddNode.ToVariable = context.addToColl().variable().GetText();
				}
			}
			return AddNode;
		}

		public override AbstractNode VisitEdgeDcl([NotNull] GiraphParser.EdgeDclContext context)
		{
			GraphDeclEdgeNode VarNode = new GraphDeclEdgeNode(context.Start.Line, context.Start.Column);
			int i = 0;
			if (context.GetChild(0).GetText() != "(")
			{
				VarNode.Name = context.variable(i++).GetText();
			}
			VarNode.VertexNameFrom = context.variable(i++).GetText();
			VarNode.VertexNameTo = context.variable(i++).GetText();

			// Visit all assignments and add them as children, if there are any
			if (context.assignment() != null)
			{
				foreach (var Child in context.assignment())
				{
					VarNode.ValueList.Add(Child.variable().GetText(), Visit(Child.boolCompOrExp()));
					VarNode.AdoptChildren(Visit(Child));
				}
			}
			return VarNode;
		}

		public override AbstractNode VisitVertexDcl([NotNull] GiraphParser.VertexDclContext context)
		{
			GraphDeclVertexNode VarNode = new GraphDeclVertexNode(context.Start.Line, context.Start.Column);

			if (context.GetChild(0).GetText() != "(")
			{
				VarNode.Name = context.variable().GetText();
			}

			if (context.assignment() != null)
			{
				foreach (var Child in context.assignment())
				{
					VarNode.ValueList.Add(Child.variable().GetText(), Visit(Child.boolCompOrExp()));
					VarNode.AdoptChildren(Visit(Child));
				}
			}
			return VarNode;
		}

		public override AbstractNode VisitErrorNode(IErrorNode node)
		{
			throw new Exception("Error at " + node.GetText() + " " + node.Parent.SourceInterval);
		}

		public override AbstractNode VisitRunFunction([NotNull] GiraphParser.RunFunctionContext context)
		{
			RunQueryNode node = new RunQueryNode(context.Start.Line, context.Start.Column);
			node.FunctionName = context.variable().GetText();
			foreach (var item in context.varOrConst())
			{
				if (item.variable() != null)
				{
					VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
					varNode.Name = item.variable().GetText();
					node.AdoptChildren(varNode);
				}
				else
				{
					ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
					conNode.Value = item.constant().GetText();
					conNode.Type = ExpressionPartTypeFinder(item.constant().GetChild(0)).ToString();
					node.AdoptChildren(conNode);
				}
			}
			return node;
		}
		public override AbstractNode VisitForConditionDcl([NotNull] GiraphParser.ForConditionDclContext context)
		{
			VariableDclNode VariableNode = new VariableDclNode(context.Start.Line, context.Start.Column);
			VariableNode.Type = context.TYPE().GetText();
			VariableNode.Name = context.variable().GetText();
			if (context.EQUALS() != null)
			{
				VariableNode.AdoptChildren(Visit(context.simpleBoolCompOrExp()));
			}
			return VariableNode;
		}

		public override AbstractNode VisitPredicateCall([NotNull] GiraphParser.PredicateCallContext context)
		{
			PredicateCall predicateCall = new PredicateCall(context.Start.Line, context.Start.Column);
			predicateCall.Name = context.variable().GetText();
			if (context.parameters() != null)
			{
				foreach (var item in context.parameters().simpleBoolCompOrExp())
				{
					predicateCall.AdoptChildren(Visit(item));
				}
			}

			return predicateCall;
		}

		public override AbstractNode VisitRemoveQuery([NotNull] GiraphParser.RemoveQueryContext context)
		{
			RemoveQueryNode removeQueryNode = new RemoveQueryNode(context.Start.Line, context.Start.Column);
			removeQueryNode.Variable = context.variable().GetText();
			if (context.where() != null)
			{
				removeQueryNode.WhereCondition = Visit(context.where());
			}
			return removeQueryNode;
		}

		public override AbstractNode VisitRemoveAllQuery([NotNull] GiraphParser.RemoveAllQueryContext context)
		{
			RemoveAllQueryNode removeAllQueryNode = new RemoveAllQueryNode(context.Start.Line, context.Start.Column);
			removeAllQueryNode.Variable = context.variable().GetText();
			if (context.where() != null)
			{
				removeAllQueryNode.WhereCondition = Visit(context.where());
			}
			return removeAllQueryNode;
		}

		public override AbstractNode VisitSimpleOperand([NotNull] GiraphParser.SimpleOperandContext context)
		{
			var switchString = context.GetChild(0).GetType().ToString();
			switch (switchString)
			{
				case "GiraphParser+ConstantContext":
					ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
					conNode.Type = ExpressionPartTypeFinder(context.GetChild(0).GetChild(0)).ToString();
					conNode.Value = context.GetText();
					return conNode;
				case "GiraphParser+VariableContext":
					VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
					varNode.Name = context.GetText();
					return varNode;
				case "Antlr4.Runtime.Tree.TerminalNodeImpl":
					ExpressionNode varAttNode = Visit(context.GetChild(1)) as ExpressionNode;
					varAttNode.hasparentheses = true;
					varAttNode.Name = context.GetText();
					return varAttNode;
				case "GiraphParser+AttributeContext":
					AbstractNode attNode = Visit(context.GetChild(0));
					return attNode;
			}
			//Skal returnere en constnode eller en varnode;
			throw new VisitVarOrConstWrongTypeException("Fejl i Mads' Kode igen!!");
		}

		public override AbstractNode VisitSimpleExpression([NotNull] GiraphParser.SimpleExpressionContext context)
		{
			ExpressionNode ExpNode = new ExpressionNode(context.Start.Line, context.Start.Column);
            ExpNode.ExpressionParts = EvaluateExpression(context);
            if (ExpNode.ExpressionParts.Count == 1)
            {
                return ExpNode.ExpressionParts[0];
            }
            return ExpNode;
		}

		public override AbstractNode VisitSimpleBoolCompOrExp([NotNull] GiraphParser.SimpleBoolCompOrExpContext context)
		{
			BoolComparisonNode BCompare = new BoolComparisonNode(context.Start.Line, context.Start.Column);
			BCompare.Type = "bool";
			// Checks if there is a prefix, if there is, add it to the Node
			if (context.simpleExpression() != null)
            {
				AbstractNode output = Visit(context.simpleExpression());
                if (output is ExpressionNode expNode && context.rightP != null && context.leftP != null)
                {
                    expNode.hasparentheses = true;
                }
                return output;
            }
			if (context.prefix != null)
			{
				BCompare.Prefix = context.prefix.Text;
				BCompare.AdoptChildren(Visit(context.simpleBoolCompOrExp(0)));
			}
			// Check if there are left and right "()" around the boolcomparison
			if (context.rightP != null && context.leftP != null && context.simpleBoolCompOrExp() != null)
			{
				BCompare.InsideParentheses = true;
				BCompare.AdoptChildren(Visit(context.simpleBoolCompOrExp(0)));
			}
			// Checks if there is a left and right statement, because this will indicatef that the boolcomparison, has a left bool and right bool, compared by the operator.
			else if (context.right != null && context.left != null && context.simpleBoolCompOrExp() != null)
			{
				BCompare.Left = Visit(context.left);
				BCompare.Right = Visit(context.right);
				BCompare.Left.Parent = BCompare;
				BCompare.Right.Parent = BCompare;

				if (context.BOOLOPERATOR() != null)
				{
					BCompare.ComparisonOperator = context.BOOLOPERATOR().GetText();
				}
				else if (context.andOr() != null)
				{
					BCompare.ComparisonOperator = context.andOr().GetText();
				}
			}
			// A boolcomparison can end in an expression or a predicate, this is handled here. 
			else
			{
				if (context.predi != null)
				{
					BCompare.AdoptChildren(Visit(context.predi));
				}
				else if (context.exp != null)
				{
					BCompare.AdoptChildren(Visit(context.exp));
				}
			}
			return BCompare;
		}
	}
}
