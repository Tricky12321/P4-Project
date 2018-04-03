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
        public Stopwatch AstBuildTimer = new Stopwatch();
        public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
        {
            AstBuildTimer.Start();
            root = new StartNode(context.Start.Line);
            // Program+ (Multiple Program children, atleast one)
            foreach (var child in context.children)
            {
                root.AdoptChildren(Visit(child));
            }
            root.Name = "Root";
            AstBuildTimer.Stop();
            Console.WriteLine("AstBuilder took: " + AstBuildTimer.ElapsedMilliseconds + "ms");
            return root;
        }

        public override AbstractNode VisitFunctionDcl([NotNull] GiraphParser.FunctionDclContext context)
        {
            FunctionNode FNode = new FunctionNode(context.Start.Line);
            // Extract the Name of the function, and the return type
            FNode.Name = context.children[0].GetText(); // Name
            FNode.ReturnType = context.children[2].GetText(); // Return Type
            int i = 0;
            // Extract the parameters from the function
            while (context.children[i].GetText() != ")")
            {
                var Child = context.children[4].GetChild(i);
                if (Child != null && Child.GetText() != ",")
                {
                    var first = context.children[4].GetChild(i).GetChild(0).GetText(); // Parameter Type
                    var second = context.children[4].GetChild(i).GetChild(1).GetText(); // Parameter Name
                    FNode.AddParameter(first, second, context.Start.Line);
                }
                i++;
            }
            // Access the codeblock related to the function, by ignoring:
            //
            // Function Name
            // Function Type
            // Function Parameters
            // K is the amount of children in the FunctionCodeBlock
            var CodeBlockEntry = context.GetChild(i + 1);
            int CodeElementsInCodeblock = CodeBlockEntry.ChildCount;
            // J skips the first child "(" and starts with the first child that is actual code
            // Loops though all the children, and ignores the last child ")"
            foreach (var Child in context.codeBlock().codeBlockContent())
            {
                var subChild = Child.GetChild(0);
                var test = subChild.GetText();
                var test2 = Visit(subChild);
                FNode.AdoptChildren(test2);
            }
            return FNode;
        }

        public override AbstractNode VisitQuerySC([NotNull] GiraphParser.QuerySCContext context)
        {
            return Visit(context.GetChild(0));
        }

        public override AbstractNode VisitCodeBlock([NotNull] GiraphParser.CodeBlockContext context)
        {
            //VertexDclsNode VerDclsNode = new VertexDclsNode(context.Start.Line);
            CodeBlockNode CodeNode = new CodeBlockNode(context.Start.Line);
            foreach (var Child in context.codeBlockContent())
            {
                CodeNode.AdoptChildren(Visit(Child.GetChild(0)));
            }
            return CodeNode;
        }

        public override AbstractNode VisitCodeBlockContent([NotNull] GiraphParser.CodeBlockContentContext context)
        {
            return VisitChildren(context);
        }

        public override AbstractNode VisitGraphInitDcl([NotNull] GiraphParser.GraphInitDclContext context)
        {
            GraphNode GNode = new GraphNode(context.Start.Line);
            GNode.Name = context.variable().GetText();
            // Handle all VetexDcl's and add them to the list in the GraphNode
            foreach (var Child in context.graphDclBlock().vertexDcls())
            {
                foreach (var NestedChild in Child.vertexDcl())
                {
                    VertexNode VNode = new VertexNode(context.Start.Line);
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
                    EdgeNode ENode = new EdgeNode(context.Start.Line);
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
            return GNode;
        }

        public override AbstractNode VisitGraphSetQuery([NotNull] GiraphParser.GraphSetQueryContext context)
        {
            GraphSetQuery SetQuery = new GraphSetQuery(context.Start.Line);

            VariableAttributeNode attribute = Visit(context.GetChild(1).GetChild(0)) as VariableAttributeNode;
            ExpressionNode expression = Visit(context.GetChild(1).GetChild(2)) as ExpressionNode;
            string expType = context.GetChild(1).GetChild(1).GetText();
            SetQuery.Attributes = (Tuple.Create<VariableAttributeNode, string, ExpressionNode>(attribute, expType, expression));

            return SetQuery;
        }

        public override AbstractNode VisitWhileLoop([NotNull] GiraphParser.WhileLoopContext context)
        {
            WhileLoopNode WhileNode = new WhileLoopNode(context.Start.Line);
            // Read the boolComparison for the while loop.
            WhileNode.BoolCompare = Visit(context.GetChild(1));
            // Read the codeblock in the whileLoop
            int CodeBlockContentCount = context.GetChild(2).ChildCount;
            for (int i = 0; i < CodeBlockContentCount; i++)
            {
                WhileNode.AdoptChildren(Visit(context.GetChild(2).GetChild(i)));
            }
            return WhileNode;
        }

        public override AbstractNode VisitBoolComparisons([NotNull] GiraphParser.BoolComparisonsContext context)
        {
            BoolComparisonNode BCompare = new BoolComparisonNode(context.Start.Line);
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
            // Checks if there is a left and right statement, because this will indicate that the boolcomparison, has a left bool and right bool, compared by the operator.
            else if (context.right != null && context.left != null && context.boolComparisons() != null)
            {
                BCompare.Left = Visit(context.left);
                BCompare.Right = Visit(context.right);
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
            ExpressionNode ExpNode = new ExpressionNode(context.Start.Line);
            ExpNode.AdoptChildren(Visit(context.GetChild(0)));
            return ExpNode;
        }

        public override AbstractNode VisitVariable([NotNull] GiraphParser.VariableContext context)
        {
            VariableNode VarNode = new VariableNode(context.Start.Line);
            VarNode.Name = context.GetChild(0).GetText();
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
            SetQueryNode SetNode = new SetQueryNode(context.Start.Line);
            Dictionary<string, int> indexList = new Dictionary<string, int>();

            for (int i = 0; i < context.children.Count; i++)
            {
                if (context.children[i] is TerminalNodeImpl)
                {
                    indexList.Add(((TerminalNodeImpl)context.children[i]).ToString(), i);
                }
            }

            int j = 0;
            foreach (var child in context.children)
            {
                if (child.ToString() == "SET" || child.ToString() == "IN" || child is GiraphParser.WhereContext)
                {
                    j++;
                }

                if (!(child is TerminalNodeImpl))
                {
                    if (j == 1)
                    {
                        VariableAttributeNode attribute = Visit(child.GetChild(0)) as VariableAttributeNode;
                        ExpressionNode expression = Visit(child.GetChild(2)) as ExpressionNode;
                        SetNode.Attributes.Add(Tuple.Create<VariableAttributeNode, string, ExpressionNode>(attribute, child.GetChild(1).GetChild(0).ToString(), expression));
                    }
                    else if (j == 2)
                    {
                        SetNode.InVariable = child.GetChild(0).ToString();
                    }
                    else if (j == 3)
                    {
                        SetNode.AdoptChildren(Visit(child));
                    }
                }
            }
            return SetNode;
        }

        public override AbstractNode VisitVarOrconstExpressionExt([NotNull] GiraphParser.VarOrconstExpressionExtContext context)
        {
            ExpressionNode exNode = new ExpressionNode(context.Start.Line);
            exNode.Name = VisitVarOrconstExpressionExtRecursive(context);
            return exNode;
        }

        private string VisitVarOrconstExpressionExtRecursive([NotNull] IParseTree context)
        {
            string expression = string.Empty;
            if (context.ChildCount == 0)
            {
                return context.ToString();
            }
            else
            {
                for (int i = 0; i < context.ChildCount; i++)
                {
                    expression += VisitVarOrconstExpressionExtRecursive(context.GetChild(i));
                }
            }
            
            return expression;
        }

        public override AbstractNode VisitAttribute([NotNull] GiraphParser.AttributeContext context)
        {
            VariableAttributeNode vaNode;
            if (context.GetChild(0).ToString() == "'")
            {
                vaNode = new AttributeNode(context.Start.Line);
            }
            else
            {
                vaNode = new VariableNode(context.Start.Line);
            }

            vaNode.Name = context.GetChild(1).GetChild(0).ToString();

            return vaNode;
        }


        public override AbstractNode VisitVarOrConst([NotNull] GiraphParser.VarOrConstContext context)
        {
            ExpressionNode exNode = new ExpressionNode(context.Start.Line);

            return exNode;
        }

        public override AbstractNode VisitWhere([NotNull] GiraphParser.WhereContext context)
        {
            WhereNode WNode = new WhereNode(context.Start.Line);
            foreach (var Child in context.boolComparisons().children)
            {
                WNode.AdoptChildren(Visit(Child));
            }
            return WNode;
        }

        public override AbstractNode VisitExtend([NotNull] GiraphParser.ExtendContext context)
        {
            ExtendNode ENode = new ExtendNode(context.Start.Line);
            ENode.ExtensionName = context.variable(0).GetText();
            if (context.variable().Length == 2)
            {
                ENode.ExtensionShortName = context.variable(1).GetText();
            }
            ENode.ExtendWithType = context.allTypeWithColl().GetText();
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
            DeclarationNode DclNode = new DeclarationNode(context.Start.Line);
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
            CollectionNode CollDcl = new CollectionNode(context.Start.Line);
            CollDcl.Name = context.variable().GetText();
            CollDcl.Type = context.allType().GetText();
            if (context.collectionAssignment() != null)
            {
                CollDcl.Assignment = Visit(context.collectionAssignment());
            }
            return CollDcl;
        }

        public override AbstractNode VisitCollectionAssignment([NotNull] GiraphParser.CollectionAssignmentContext context)
        {
            return Visit(context.GetChild(0));
        }

        public override AbstractNode VisitIfElseIfElse([NotNull] GiraphParser.IfElseIfElseContext context)
        {
            IfElseIfElseNode IfNode = new IfElseIfElseNode(context.Start.Line);
            IfNode.IfCondition = Visit(context.boolComparisons());
            IfNode.IfCodeBlock = Visit(context.codeBlock());
            if (context.elseifCond() != null)
            {
                // Loop though all the ElseIf(s)
                foreach (var ElseIf in context.elseifCond())
                {
                    // Add their conditions and codeblocks
                    IfNode.ElseIfConditions.Add(Visit(ElseIf.boolComparisons()));
                    IfNode.ElseIfCodeBlocks.Add(Visit(ElseIf.codeBlock()));
                }
            }

            // Else codeblock, First codeblock element, then it adopts the rest, if there are any
            if (context.elseCond() != null)
            {
                // There will never be more then one Else block, and it does not have a boolcomparison
                if (context.elseCond().codeBlock().ChildCount > 0)
                {
                    IfNode.ElseCodeBlock = Visit(context.elseCond().codeBlock());
                }
            }

            return IfNode;
        }

        public override AbstractNode VisitPredicate([NotNull] GiraphParser.PredicateContext context)
        {
            PredicateNode PNode = new PredicateNode(context.Start.Line);
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
                    PNode.AddParameter(ParameterType, ParameterName, context.Start.Line);
                }
            }
            // Adopt the boolcomparisons of the Predicate as children to the PNode
            PNode.AdoptChildren(Visit(context.boolComparisons()));
            return PNode;
        }

        public override AbstractNode VisitSelect([NotNull] GiraphParser.SelectContext context)
        {
            SelectQueryNode SelectNode = new SelectQueryNode(context.Start.Line);
            SelectNode.Type = context.allTypeWithColl().GetText();
            SelectNode.Variable = context.variableFunc().GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                SelectNode.WhereCondition = Visit(context.where());
            }
            return SelectNode;
        }


        public override AbstractNode VisitSelectAll([NotNull] GiraphParser.SelectAllContext context)
        {
            SelectAllQueryNode SelectNode = new SelectAllQueryNode(context.Start.Line);
            SelectNode.Type = context.allTypeWithColl().GetText();
            SelectNode.Variable = context.variableFunc().GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                SelectNode.WhereCondition = Visit(context.where());
            }
            return SelectNode;
        }

        public override AbstractNode VisitEnqueueOP([NotNull] GiraphParser.EnqueueOPContext context)
        {
            EnqueueQueryNode EnqueueNode = new EnqueueQueryNode(context.Start.Line);
            EnqueueNode.VariableTo = context.variable(1).GetText();
            EnqueueNode.VariableToAdd = context.variable(0).GetText();

            if (context.where() != null && context.where().ChildCount > 0)
            {
                EnqueueNode.WhereCondition = Visit(context.where());
            }

            return EnqueueNode;
        }

        public override AbstractNode VisitDequeueOP([NotNull] GiraphParser.DequeueOPContext context)
        {
            DequeueQueryNode DequeueNode = new DequeueQueryNode(context.Start.Line);
            DequeueNode.Variable = context.variable().GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                DequeueNode.WhereCondition = Visit(context.where());
            }
            return DequeueNode;
        }

        public override AbstractNode VisitPopOP([NotNull] GiraphParser.PopOPContext context)
        {
            PopQueryNode PopNode = new PopQueryNode(context.Start.Line);
            PopNode.Variable = context.variable().GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                PopNode.WhereCondition = Visit(context.where());
            }
            return PopNode;
        }

        public override AbstractNode VisitPushOP([NotNull] GiraphParser.PushOPContext context)
        {
            PushQueryNode PushNode = new PushQueryNode(context.Start.Line);
            PushNode.VariableToAdd = context.variable(0).GetText();
            PushNode.VariableAddTo = context.variable(1).GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                PushNode.WhereCondition = Visit(context.where());
            }
            return PushNode;
        }

        public override AbstractNode VisitExtractMinOP([NotNull] GiraphParser.ExtractMinOPContext context)
        {
            ExtractMinQueryNode ExtractQuery = new ExtractMinQueryNode(context.Start.Line);

            ExtractQuery.Variable = context.variable().GetText();
            if (context.attribute() != null && context.attribute().ChildCount > 0)
            {
                ExtractQuery.Attribute = context.attribute().GetText();
            }
            if (context.where() != null && context.where().ChildCount > 0)
            {
                ExtractQuery.WhereCondition = Visit(context.where());
            }
            return ExtractQuery;
        }

        public override AbstractNode VisitExtractMaxOP([NotNull] GiraphParser.ExtractMaxOPContext context)
        {
            ExtractMaxQueryNode ExtractQuery = new ExtractMaxQueryNode(context.Start.Line);

            ExtractQuery.Variable = context.variable().GetText();
            if (context.attribute() != null && context.attribute().ChildCount > 0)
            {
                ExtractQuery.Attribute = context.attribute().GetText();
            }
            if (context.where() != null && context.where().ChildCount > 0)
            {
                ExtractQuery.WhereCondition = Visit(context.where());
            }
            return ExtractQuery;
        }

        public override AbstractNode VisitDequeueOPOneLine([NotNull] GiraphParser.DequeueOPOneLineContext context)
        {
            DequeueQueryNode DequeueNode = new DequeueQueryNode(context.Start.Line);
            DequeueNode.Variable = context.variable().GetText();
            if (context.where() != null && context.where().ChildCount > 0)
            {
                DequeueNode.WhereCondition = Visit(context.where());
            }
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
            VariableDclNode VariableNode = new VariableDclNode(context.Start.Line);
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
            ReturnNode RNode = new ReturnNode(context.Start.Line);
            RNode.AdoptChildren(Visit(context.GetChild(1)));
            return RNode;
        }

        public override AbstractNode VisitForLoop([NotNull] GiraphParser.ForLoopContext context)
        {
            ForLoopNode ForLoop = new ForLoopNode(context.Start.Line);
            var contextInside = context.forCondition().forConditionInside();

            if (contextInside.inlineDcl() != null && contextInside.inlineDcl().ChildCount > 0)
            {
                ForLoop.VariableDeclartion = Visit(contextInside.inlineDcl());
            }
            #region First VarOrConst | Operation 
            //Check if the first is a VarOrConst, if it is, check if its a var or a const
            if (contextInside.varOrConstOperation(0).varOrConst() != null && contextInside.varOrConstOperation(0).varOrConst().ChildCount > 0)
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
            if (contextInside.varOrConstOperation(1).varOrConst() != null && contextInside.varOrConstOperation(1).varOrConst().ChildCount > 0)
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
            VariableDclNode VarDcl = new VariableDclNode(context.Start.Line);
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
            PrintQueryNode PNode = new PrintQueryNode(context.Start.Line);
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
            ForeachLoopNode ForeachNode = new ForeachLoopNode(context.Start.Line);
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
            AddQueryNode AddNode = new AddQueryNode(context.Start.Line);
            // ITS A GRAPH ADD
            if (context.addToGraph() != null)
            {
                AddNode.IsGraph = true;
                if (context.addToGraph().vertexDcls() != null) {
					foreach (var Child in context.addToGraph().vertexDcls().vertexDcl())
					{
						AddNode.Dcls.Add(Visit(Child));
					}
                }
            }
            // ITS A COLLECTION ADD
            else if (context.addToColl() != null)
            {
                
                AddNode.IsColl = true;
                // ITS ALL TYPE
                if (context.addToColl().allType() != null)
                {
                    AddNode.IsType = true;
                }
                // ITS A VARIABLE
                else if (context.addToColl().variable() != null)
                {
                    AddNode.IsVariable = true;
                }
                // ITS A QUERY
                else if (context.addToColl().returnQuery() != null)
                {
                    AddNode.IsQuery = true;
                } else {
                    throw new Exception("Whaaaaat!");
                }
            }
            else
            {
                throw new Exception("Whaaaaaaaat?!");
            }
            return AddNode;
        }
    }
}
