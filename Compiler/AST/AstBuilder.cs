using System;
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
            FNode.ReturnType = context.allTypeWithColl().GetText(); // Return Type
            // Extract the parameters from the function
            if (context.formalParams() != null)
            {
                foreach (var Parameter in context.formalParams().formalParam())
                {
                    var Type = Parameter.allType().GetText();  // Parameter Type
                    var Name = Parameter.variable().GetText(); // Parameter Name

                    FNode.AddParameter(Type, Name, context.Start.Line, context.Start.Column);
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
                            VNode.ValueList.Add(Attribute.variable().GetText(), Attribute.expression().GetText());
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
                                ENode.ValueList.Add(Attribute.variable().GetText(), Attribute.expression().GetText());
                            }
                        }
                    }
                    GNode.Edges.Add(ENode);
                }
            }

            // Handle the setQuery Nodes, if there are any
            foreach (var Child in context.graphDclBlock().graphSetQuery())
            {
                GNode.AdoptChildren(Visit(Child));
            }

            foreach (var Child in GNode.Children)
            {
                if ((Child as GraphSetQuery).Attributes.Item1.Name == "'Directed'")
                {
                    GNode.Directed = (Child as GraphSetQuery).ToExpressionString() == "true";
                }
            }

            return GNode;
        }

        public override AbstractNode VisitGraphSetQuery([NotNull] GiraphParser.GraphSetQueryContext context)
        {
            GraphSetQuery SetQuery = new GraphSetQuery(context.Start.Line, context.Start.Column);

            VariableAttributeNode attribute = Visit(context.GetChild(1).GetChild(0)) as VariableAttributeNode;

            attribute.ClassType = AllType.GRAPH;
            ExpressionNode expression = Visit(context.GetChild(1).GetChild(2)) as ExpressionNode;
            string expType = context.GetChild(1).GetChild(1).GetText();
            SetQuery.Attributes = (Tuple.Create<VariableAttributeNode, string, ExpressionNode>(attribute, expType, expression));

            return SetQuery;
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

        public override AbstractNode VisitBoolComparisons([NotNull] GiraphParser.BoolComparisonsContext context)
        {
            BoolComparisonNode BCompare = new BoolComparisonNode(context.Start.Line, context.Start.Column);
            // Checks if there is a prefix, if there is, add it to the Node
            if (context.prefix != null)
            {
                BCompare.Prefix = context.prefix.Text;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
            // Checks if there is a Suffix, if there is, add it to the Node
            if (context.suffix != null)
            {
                BCompare.Suffix = context.suffix.Text;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
            // Check if there are left and right "()" around the boolcomparison
            if (context.rightP != null && context.leftP != null && context.boolComparisons() != null)
            {
                BCompare.InsideParentheses = true;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
            // Checks if there is a left and right statement, because this will indicatef that the boolcomparison, has a left bool and right bool, compared by the operator.
            else if (context.right != null && context.left != null && context.boolComparisons() != null)
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

        public override AbstractNode VisitExpression([NotNull] GiraphParser.ExpressionContext context)
        {
            ExpressionNode ExpNode = new ExpressionNode(context.Start.Line, context.Start.Column);
            ExpNode.ExpressionParts = (EvaluateExpression(context));
            //ExpNode.AdoptChildren(Visit(context.GetChild(0)));

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

        public override AbstractNode VisitVariableFunc([NotNull] GiraphParser.VariableFuncContext context)
        {
            return Visit(context.GetChild(0));
        }

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
            if (context.variable() != null) {
                SetNode.InVariable = Visit(context.variable());
				//SetNode.InVariable.Name = context.variable().GetText();
                SetNode.SetAttributes = true;
                foreach (var ExpNode in context.setExpressionAtri())
                {
                    VariableAttributeNode attribute = Visit(ExpNode.attribute()) as VariableAttributeNode;
                    attribute.ClassVariableName = SetNode.InVariable.Name; //  Only set Class Variable if its an attribute
                    attribute.IsAttribute = true;
                    ExpressionNode expression = Visit(ExpNode.expression()) as ExpressionNode;
                    SetNode.Attributes.Add(Tuple.Create(attribute, ExpNode.compoundAssign().GetText(), expression));
                }
            } else {
				// If its variables being set
                SetNode.SetVariables = true;
                foreach (var ExpNode in context.setExpressionVari())
                {
                    VariableAttributeNode attribute = Visit(ExpNode.variable()) as VariableAttributeNode;
                    ExpressionNode expression = Visit(ExpNode.expression()) as ExpressionNode;
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

            //expressionPart = VisitVarOrconstExpressionExtRecursive(context);

            for (int i = 0; i < context.ChildCount; i++)
            {
                expressionPart.Add(Visit(context.GetChild(i)));
            }
            return expressionPart;
        }

        public override AbstractNode VisitExpressionExtension([NotNull] GiraphParser.ExpressionExtensionContext context)
        {
            var switchString = context.GetChild(0).GetType().ToString();
            switch (switchString)
            {
                case "GiraphParser+ConstantContext":
                    ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
                    conNode.Type = ExpressionPartTypeFinder(context.GetChild(0).GetChild(0)).ToString();
                    conNode.Value = context.GetText();
                    return conNode;
                case "GiraphParser+VariableFuncContext":
                    VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
                    varNode.Name = context.GetText();
                    return varNode;
                case "GiraphParser+ReturnQueryContext":
                    return Visit(context.GetChild(0));
                case "Antlr4.Runtime.Tree.TerminalNodeImpl":
                    AbstractNode varAttNode = Visit(context.GetChild(1));
                    varAttNode.Name = context.GetText();
                    return varAttNode;
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
                case "GiraphParser+BoolComparisonsContext":
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
            WNode.AdoptChildren(Visit(context.boolComparisons()));
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
            if (context.constant() != null)
            {
                ENode.ExtensionDefaultValue = context.constant().GetText();
            }
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
            if (context.expression() != null)
            {
                DclNode.Assignment = Visit(context.expression());
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
            IfNode.IfCondition = Visit(context.boolComparisons()) as BoolComparisonNode;
            IfNode.IfCodeBlock = Visit(context.codeBlock()) as CodeBlockNode;
            if (context.elseifCond() != null)
            {
                // Loop though all the ElseIf(s)
                foreach (var ElseIf in context.elseifCond())
                {
                    // Add their conditions and codeblocks
                    IfNode.ElseIfList.Add(Tuple.Create((Visit(ElseIf.boolComparisons()) as BoolComparisonNode), (Visit(ElseIf.codeBlock()) as CodeBlockNode)));
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
            if (context.formalParams().formalParam() != null)
            {
                // If there are any parameters, loop though all of them
                foreach (var Param in context.formalParams().formalParam())
                {
                    string ParameterName = Param.variable().GetText();
                    string ParameterType = Param.allType().GetText();
                    // Add them to the paramter list
                    PNode.AddParameter(ParameterType, ParameterName, context.Start.Line, context.Start.Column);
                }
            }
            // Adopt the boolcomparisons of the Predicate as children to the PNode
            PNode.AdoptChildren(Visit(context.boolComparisons()));
            return PNode;
        }

        public override AbstractNode VisitSelect([NotNull] GiraphParser.SelectContext context)
        {
            SelectQueryNode SelectNode = new SelectQueryNode(context.Start.Line, context.Start.Column);
            //SelectNode.Type = context.allTypeWithColl().GetText();
            SelectNode.Variable = context.variableFunc().GetText();
            if (context.where() != null)
            {
                SelectNode.WhereCondition = Visit(context.where());
            }
            return SelectNode;
        }

        public override AbstractNode VisitSelectAll([NotNull] GiraphParser.SelectAllContext context)
        {
            SelectAllQueryNode SelectNode = new SelectAllQueryNode(context.Start.Line, context.Start.Column);
            //SelectNode.Type = context.allTypeWithColl().GetText();
            SelectNode.Variable = context.variableFunc().GetText();
            if (context.where() != null)
            {
                SelectNode.WhereCondition = Visit(context.where());
            }
            return SelectNode;
        }

        public override AbstractNode VisitEnqueueOP([NotNull] GiraphParser.EnqueueOPContext context)
        {
            EnqueueQueryNode EnqueueNode = new EnqueueQueryNode(context.Start.Line, context.Start.Column);
            EnqueueNode.VariableToAdd = Visit(context.varOrConst());
            EnqueueNode.VariableCollection = context.variable().GetText();
            return EnqueueNode;
        }

        public override AbstractNode VisitDequeueOP([NotNull] GiraphParser.DequeueOPContext context)
        {
            DequeueQueryNode DequeueNode = new DequeueQueryNode(context.Start.Line, context.Start.Column);
            DequeueNode.Variable = context.variable().GetText();
            return DequeueNode;
        }

        public override AbstractNode VisitPopOP([NotNull] GiraphParser.PopOPContext context)
        {
            PopQueryNode PopNode = new PopQueryNode(context.Start.Line, context.Start.Column);
            PopNode.Variable = context.variable().GetText();
            return PopNode;
        }

        public override AbstractNode VisitPushOP([NotNull] GiraphParser.PushOPContext context)
        {
            PushQueryNode PushNode = new PushQueryNode(context.Start.Line, context.Start.Column);
            PushNode.VariableToAdd = Visit(context.varOrConst());
            PushNode.VariableCollection = context.variable().GetText();
            return PushNode;
        }

        public override AbstractNode VisitExtractMinOP([NotNull] GiraphParser.ExtractMinOPContext context)
        {
            ExtractMinQueryNode ExtractQuery = new ExtractMinQueryNode(context.Start.Line, context.Start.Column);

            ExtractQuery.Variable = context.variable().GetText();
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
                VariableNode.AdoptChildren(Visit(context.expression()));
            }
            return VariableNode;
        }

        public override AbstractNode VisitReturnBlock([NotNull] GiraphParser.ReturnBlockContext context)
        {
            ReturnNode RNode = new ReturnNode(context.Start.Line, context.Start.Column);
            RNode.AdoptChildren(Visit(context.GetChild(1)));
            return RNode;
        }

        public override AbstractNode VisitForLoop([NotNull] GiraphParser.ForLoopContext context)
        {
            ForLoopNode ForLoop = new ForLoopNode(context.Start.Line, context.Start.Column);
            var contextInside = context.forCondition().forConditionInside();
            
            if (contextInside.inlineDcl() != null)
            {
                ForLoop.VariableDeclartionType = Utilities.FindTypeFromString(contextInside.inlineDcl().allType().GetText());
                ForLoop.VariableDeclaration = Visit(contextInside.inlineDcl());
            }
            else if(contextInside.varOrConst() != null)
            {
                ForLoop.FromValue = contextInside.varOrConst().variable().GetText();
            }
            #region First VarOrConst | Operation 
            //Check if the first is a VarOrConst, if it is, check if its a var or a const
            if (contextInside.varOrConstOperation(0).varOrConst() != null)
            {
                //CHeck if its a var or const
                // It was a variable
                if (contextInside.varOrConstOperation(0).varOrConst().variable() != null && contextInside.varOrConstOperation(0).varOrConst().variable().ChildCount > 0)
                {
                    ForLoop.ToVariable = true;
                    ForLoop.ToValue = contextInside.varOrConstOperation(0).varOrConst().variable().GetText();
                }
                // It was a const
                else
                {
                    ForLoop.ToConst = true;
                    ForLoop.ToValue = contextInside.varOrConstOperation(0).varOrConst().constant().GetText();
                }
            }
            //Its not a var or const, which means its an operation
            else
            {
                ForLoop.ToOperation = true;
                ForLoop.ToValueOperation = Visit(contextInside.varOrConstOperation(0).operation());
            }
            #endregion
            #region Second VarOrConst | Operation 
            //Check if the first is a VarOrConst, if it is, check if its a var or a const
            if (contextInside.varOrConstOperation(1).varOrConst() != null)
            {
                //CHeck if its a var or const
                // It was a variable
                if (contextInside.varOrConstOperation(1).varOrConst().variable() != null && contextInside.varOrConstOperation(1).varOrConst().variable().ChildCount > 0)
                {
                    ForLoop.IncrementVariable = true;
                    ForLoop.IncrementValue = contextInside.varOrConstOperation(1).varOrConst().variable().GetText();
                }
                // It was a const
                else
                {
                    ForLoop.IncrementConst = true;
                    ForLoop.IncrementValue = contextInside.varOrConstOperation(1).varOrConst().constant().GetText();
                }
            }
            //Its not a var or const, which means its an operation
            else
            {
                ForLoop.IncrementOperation = true;
                ForLoop.IncrementValueOperation = Visit(contextInside.varOrConstOperation(1).operation());
            }
            #endregion
            // Visit all the children of the Codeblock associated with the ForLoop
            foreach (var Child in context.codeBlock().codeBlockContent())
            {
                // Adopt the children
                ForLoop.AdoptChildren(Visit(Child.GetChild(0)));
            }
            return ForLoop;
        }

        public override AbstractNode VisitInlineDcl([NotNull] GiraphParser.InlineDclContext context)
        {
            VariableDclNode VarDcl = new VariableDclNode(context.Start.Line, context.Start.Column);
            VarDcl.Type = context.allType().GetText();
            VarDcl.Name = context.VARIABLENAME().GetText();
            VarDcl.AdoptChildren(Visit(context.operation()));
            return VarDcl;
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
            foreach (var child in context.printOptions().printOption())
            {
                if (child.VARIABLENAME() != null && child.VARIABLENAME().GetText().Length > 0)
                {
                    foreach (var subChild in child.children)
                    {
                        PNode.AdoptChildren(Visit(subChild));
                    }
                }
                else
                {
                    PNode.AdoptChildren(Visit(child.GetChild(0)));
                }
            }
            return PNode;
        }

        public override AbstractNode VisitForeachLoop([NotNull] GiraphParser.ForeachLoopContext context)
        {
            ForeachLoopNode ForeachNode = new ForeachLoopNode(context.Start.Line, context.Start.Column);
            ForeachNode.VariableName = context.foreachCondition().variable().GetText();
            ForeachNode.VariableType = context.foreachCondition().allType().GetText();
            ForeachNode.InVariableName = context.foreachCondition().variableFunc().GetText();
            // Check the where condition
            if (context.where() != null && context.where().GetText().Length > 0)
            {
                ForeachNode.WhereCondition = Visit(context.where());
            }
            // Visit the children of the codeblock
            foreach (var Child in context.codeBlock().children)
            {
                ForeachNode.AdoptChildren(Visit(Child.GetChild(0)));
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
                if (context.addToColl().expression() != null)
                {
                    AddNode.IsType = true;
                    AddNode.TypeOrVariable = Visit(context.addToColl().expression());
                }
                else if (context.addToColl().expression() != null)
                {
                    AddNode.IsQuery = true;
                    AddNode.Query = Visit(context.addToColl().expression());

                }
                else
                {
                    throw new Exception("Whaaaaat!");
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
                if (context.addToColl().where() != null)
                {
                    AddNode.WhereCondition = Visit(context.addToColl().where());
                }
            }
            else
            {
                throw new Exception("Whaaaaaaaat?!");
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
                    VarNode.AdoptChildren(Visit(Child));
                }
            }
            return VarNode;
        }

        public override AbstractNode VisitErrorNode(IErrorNode node)
        {
            throw new Exception("Error at " + node.GetText() + " " + node.Parent.SourceInterval);
        }

        public override AbstractNode VisitVarOrFuncOrConst([NotNull] GiraphParser.VarOrFuncOrConstContext context)
        {
            return Visit(context.GetChild(0));
        }

		public override AbstractNode VisitRunFunction([NotNull] GiraphParser.RunFunctionContext context)
		{
            RunQueryNode node = new RunQueryNode(context.Start.Line, context.Start.Column);
            node.FunctionName = context.variable().GetText();
            foreach (var item in context.varOrConst())
            {
                if (item.variable() != null) {
                    VariableNode varNode = new VariableNode(context.Start.Line, context.Start.Column);
                    varNode.Name = item.variable().GetText();
                    node.AdoptChildren(varNode);
                } else {
                    ConstantNode conNode = new ConstantNode(context.Start.Line, context.Start.Column);
                    conNode.Value = item.constant().GetText();
                    conNode.Type = ExpressionPartTypeFinder(item.constant().GetChild(0)).ToString();
                }
            }
            return node;
		}
	}
}
