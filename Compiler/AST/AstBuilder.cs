using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.DatatypeNodes.Graph;
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
            FNode.FunctionName = context.children[0].GetText(); // Name
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
                    FNode.AddParameter(first, second,context.Start.Line);
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
            GNode.Name = context.GetChild(1).GetText();
            // Get into the the codeblocks children to find Vertices and Edges
            int childCounter = context.children[2].ChildCount;
            // Skip VertexDcls (just go to each individual VertexDcl
            for (int i = 0; i < childCounter; i++)
            {
                GNode.AdoptChildren(Visit(context.children[2].GetChild(i))); // Vertices, Edges, SetQuerys
            }
            return GNode;
        }

        public override AbstractNode VisitVertexDcl([NotNull] GiraphParser.VertexDclContext context)
        {
            VertexNode VertexDcl = new VertexNode(context.Start.Line);
            bool VariableName = false; // If there is a name for the vertex or not. 
            // Check if there is a varaible Name 
            if (context.GetChild(0).GetText() != "(")
            {
                VertexDcl.Name = context.GetChild(0).GetChild(0).GetText();
                VariableName = true;
            }
            // Checks if there is assignments in the Vertex (First child is either a varaiblename or a '('
            if ((VariableName && context.ChildCount > 3) || (!VariableName && context.ChildCount > 2))
            {
                int i = VariableName ? 2 : 1;
                // Read all parameters, skip the last end ")" (therefore -1)
                for (; i < context.ChildCount - 1; i++)
                {
                    // Skip comma in VertexDcl Parameters
                    if (context.GetChild(i).GetText() != ",")
                    {
                        // Read VariableName and Value from the parameters
                        string varaibleName = context.GetChild(i).GetChild(0).GetText();
                        string varaibleValue = context.GetChild(i).GetChild(2).GetText();
                        VertexDcl.ValueList.Add(varaibleName, varaibleValue);
                    }
                }
            }
            return VertexDcl;
        }

        public override AbstractNode VisitEdgeDcl([NotNull] GiraphParser.EdgeDclContext context)
        {
            EdgeNode EdgeDcl = new EdgeNode(context.Start.Line);
            bool EdgeName = false; // If there is a name for the vertex or not. 
            // Check if there is a varaible Name 
            if (context.GetChild(0).GetText() != "(")
            {
                EdgeDcl.Name = context.GetChild(0).GetChild(0).GetText();
                EdgeName = true;
            }

            int VertexNamingIndex = 1;
            if (EdgeName)
            {
                VertexNamingIndex++;
            }
            EdgeDcl.VertexNameFrom = context.GetChild(VertexNamingIndex).GetText();
            EdgeDcl.VertexNameTo = context.GetChild(VertexNamingIndex + 2).GetText();

            // Checks if there is assignments in the Edge (First child is either a varaiblename or a '('
            if ((EdgeName && context.ChildCount > 6) || (!EdgeName && context.ChildCount > 5))
            {
                // If there is an Edgename move one token extra forward (to skip the "(")
                int i = EdgeName ? 5 : 4;
                // Read all parameters, skip the last end ")" (therefore -1)
                for (; i < context.ChildCount - 1; i++)
                {
                    // Skip comma in VertexDcl Parameters
                    if (context.GetChild(i).GetText() != ",")
                    {
                        // Read VariableName and Value from the parameters
                        string varaibleName = context.GetChild(i).GetChild(0).GetText();
                        string varaibleValue = context.GetChild(i).GetChild(2).GetText();
                        EdgeDcl.ValueList.Add(varaibleName, varaibleValue);
                    }
                }
            }
            return EdgeDcl;
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
    }
}
