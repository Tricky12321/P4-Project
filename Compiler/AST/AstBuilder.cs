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
                root.AdoptChildren(Visit(child));
            }
            root.Name = "Root";
            return root;
        }

        public override AbstractNode VisitProgram([NotNull] GiraphParser.ProgramContext context)
        {
            ProgramNode PNode = new ProgramNode(context.Start.Line);
            PNode.AdoptChildren(VisitChildren(context));
            PNode.Name = "PNode";
            return PNode;
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
            for (int j = 1; j < CodeElementsInCodeblock; j++)
            {
                FNode.AdoptChildren(Visit(CodeBlockEntry.GetChild(j)));
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
                    VNode.Name = NestedChild.variable().GetText();
                    foreach (var Attribute in NestedChild.assignment())
                    {
                        VNode.ValueList.Add(Attribute.variable().GetText(), Attribute.expression().GetText());
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
                    ENode.Name = NestedChild.variable(0).GetText();
                    ENode.VertexNameFrom = NestedChild.variable(1).GetText();
                    ENode.VertexNameTo = NestedChild.variable(2).GetText();
                    foreach (var Attribute in NestedChild.assignment())
                    {
                        ENode.ValueList.Add(Attribute.variable().GetText(), Attribute.expression().GetText());
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
            return base.VisitWhileLoop(context);
        }

        public override AbstractNode VisitBoolComparisons([NotNull] GiraphParser.BoolComparisonsContext context)
        {
            BoolComparisonNode BCompare = new BoolComparisonNode(context.Start.Line);
            // If there is a left and right side, This is what will be used
            if (context.right != null && context.left != null)
            {
                BCompare.Left = Visit(context.GetChild(0));
                BCompare.ComparisonOperator = context.GetChild(1).GetText();
                BCompare.Right = Visit(context.GetChild(2));
                BCompare.NextNodeBool = false;
            }
            else
            {
                // If there is no Left or Right, this means that the end of this branch is a
                if (context.GetChild(1) != null)
                {
                    BCompare.NextNode = Visit(context.GetChild(1));
                }
                else
                {
                    BCompare.NextNode = Visit(context.GetChild(0));
                }
                BCompare.NextNodeBool = true;
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
            VarNode.VariableName = context.GetChild(0).GetText();
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
            foreach (var setAtri in context.setExpressionAtri())
            {
                string AttributeName = setAtri.attribute().GetChild(1).GetText();
                string AttributeValue = setAtri.varOrConst().GetChild(0).GetText();
                SetNode.Attributes.Add(AttributeName, AttributeValue);
            }
            SetNode.Name = context.variable().GetText();
            SetNode.WhereCondition = Visit(context.where());

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
            if (context.variable().Length == 2) {
                ENode.ExtensionShortName = context.variable(1).GetText();
            }
            ENode.ExtendWithType = context.allTypeWithColl().GetText();
            ENode.ClassToExtend = context.objects().GetText();
            if (context.constant() != null) {
				ENode.ExtensionDefaultValue = context.constant().GetText();
            }
			return ENode;
		}

		public override AbstractNode VisitDcls([NotNull] GiraphParser.DclsContext context)
		{
            return Visit(context);
		}
		public override AbstractNode VisitObjectDcl([NotNull] GiraphParser.ObjectDclContext context)
		{
            DeclarationNode DclNode = new DeclarationNode(context.Start.Line);
            DclNode.Type = context.objects().GetText();
            DclNode.Name = context.variable().GetText();
            DclNode.Assignment = Visit(context.expression());
            return DclNode;
		}
	}
}
