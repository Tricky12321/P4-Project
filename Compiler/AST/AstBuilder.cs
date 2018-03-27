using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.Exceptions;
using Antlr4.Runtime;
using System.Text.RegularExpressions;
namespace Compiler.AST
{
    internal class AstBuilder : GiraphParserBaseVisitor<AbstractNode>
    {
        public AbstractNode root;
        public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
        {
            root = new StartNode(context.Start.Line);
            // Program+ (Multiple Program children, atleast one)
            foreach (var child in context.children)
            {
                root.AdoptChildren(Visit(child.GetChild(0)));
            }
            root.Name = "Root";
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
            foreach (var Child in context.codeBlock().children)
            {
                FNode.AdoptChildren(Visit(Child));
            }

            /*
            foreach (var CodeBlockChild in context.children)
            {
                if (k > i) {
                    FNode.AdoptChildren(Vist)
				    FNode.AdoptChildren(Visit(CodeBlockChild));
                }
                k++;
            }
            */
            return FNode;
        }

        public override AbstractNode VisitCodeBlock([NotNull] GiraphParser.CodeBlockContext context)
        {
            VertexDclsNode VerDclsNode = new VertexDclsNode(context.Start.Line);
            return Visit(context.children[1]);
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
                foreach (var NestedChild in Child.children)
                {
                    GNode.AdoptChildren(Visit(NestedChild)); // Vertices, Edges, SetQuerys
                }
            }
            return GNode;
        }

        public override AbstractNode VisitGraphSetQuery([NotNull] GiraphParser.GraphSetQueryContext context)
        {
            GraphSetQuery SetQuery = new GraphSetQuery(context.Start.Line);
            SetQuery.AttributeName = context.GetChild(1).GetChild(0).GetText();
            SetQuery.AttributeValue = context.GetChild(1).GetChild(2).GetText();
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
            // If there is a left and right side, This is what will be used
            if (context.prefix != null)
            {
                BCompare.Prefix = context.prefix.Text;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
            if (context.suffix != null)
            {
                BCompare.Suffix = context.suffix.Text;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
            if (context.rightP != null && context.leftP != null && context.boolComparisons() != null)
            {
                BCompare.InsideParentheses = true;
                BCompare.AdoptChildren(Visit(context.boolComparisons(0)));
            }
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
            /*
            if (context.setExpressionAtri() != null) {
                foreach (var setAtri in context.setExpressionAtri())
                {
                    string AttributeName = setAtri.attribute()..GetText();
                    string AttributeAssignmentOp = setAtri.compoundAssign().GetText();
                    AbstractNode AttributeValue = setAtri.;
                    SetNode.Attributes.Add(AttributeName, ());
                }
                SetNode.Name = context.variable().GetText();
            } 
            else if (context.setExpressionVari() != null) {
                foreach (var setVari in context.setExpressionVari())
                {
                    string GetterVariable = setVari.variable().GetText();
                    string Operator = setVari.compoundAssign().GetText();
                    AbstractNode SetterVariable = setVari.varOrconstExpressionExt().GetText();
                    SetNode.Attributes.Add(AttributeName, AttributeValue);
                }
                SetNode.Name = context.variable().GetText();
            }

            if (context.where() != null) {
				SetNode.WhereCondition = Visit(context.where());
            }
            */
            return SetNode;
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
            DeclarationNode CollDcl = new DeclarationNode(context.Start.Line);
            CollDcl.Name = context.variable().GetText();
            CollDcl.CollectionDcl = true;
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
            IfNode.IfCondition = context.boolComparisons(0);
            return base.VisitIfElseIfElse(context);
        }
    }
}