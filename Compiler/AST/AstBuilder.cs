﻿using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
namespace Compiler.AST
{
    internal class AstBuilder : GiraphParserBaseVisitor<AbstractNode>
    {
        public AbstractNode root;
		public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
		{
            root = new StartNode();
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
            ProgramNode PNode = new ProgramNode();
            PNode.AdoptChildren(VisitChildren(context));
            PNode.Name = "PNode";
            return PNode;
		}

		public override AbstractNode VisitFunctionDcl([NotNull] GiraphParser.FunctionDclContext context)
		{
            FunctionNode FNode = new FunctionNode();
			// Extract the Name of the function, and the return type
            FNode.FunctionName = context.children[0].GetText(); // Name
            FNode.ReturnType = context.children[2].GetText(); // Return Type
            int i = 0;
            // Extract the parameters from the function
            while (context.children[i].GetText() != ")") {
                var Child = context.children[4].GetChild(i);
                if (Child != null && Child.GetText() != ",") {
					var first = context.children[4].GetChild(i).GetChild(0).GetText(); // Parameter Type
					var second = context.children[4].GetChild(i).GetChild(1).GetText(); // Parameter Name
					FNode.AddParameter(first, second);
                }
                i++;
            }
            // Access the codeblock related to the function, by ignoring all else.
            int k = 0;
            foreach (var CodeBlockChild in context.children)
            {
                if (k > i) {
				    FNode.AdoptChildren(Visit(CodeBlockChild));
                }
                k++;
            }
			return FNode;
        }

		public override AbstractNode VisitCodeBlock([NotNull] GiraphParser.CodeBlockContext context)
		{
            VertexDclsNode VerDclsNode = new VertexDclsNode();

            return Visit(context.children[1]);
		}

		public override AbstractNode VisitCodeBlockContent([NotNull] GiraphParser.CodeBlockContentContext context)
		{
            return VisitChildren(context);
		}

		public override AbstractNode VisitGraphInitDcl([NotNull] GiraphParser.GraphInitDclContext context)
		{
            GraphNode GNode = new GraphNode();
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
            VertexNode VertexDcl = new VertexNode();
            bool VariableName = false; // If there is a name for the vertex or not. 
            // Check if there is a varaible Name 
            if (context.GetChild(0).GetText() != "(") {
                VertexDcl.Name = context.GetChild(0).GetChild(0).GetText();
                VariableName = true;
            }
            // Checks if there is assignments in the Vertex (First child is either a varaiblename or a '('
            if ((VariableName && context.ChildCount > 3) || (!VariableName && context.ChildCount > 2)) {
                int i = 1;
                if (VariableName) {
                    i++;
                }
                // Read all parameters, skip the last end ")" (therefore -1)
                for (;i < context.ChildCount-1; i++)
                {
                    // Skip comma in VertexDcl Parameters
                    if (context.GetChild(i).GetText() != ",") {
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
            EdgeNode EdgeDcl = new EdgeNode();
            bool EdgeName = false; // If there is a name for the vertex or not. 
            // Check if there is a varaible Name 
            if (context.GetChild(0).GetText() != "(")
            {
                EdgeDcl.Name = context.GetChild(0).GetChild(0).GetText();
                EdgeName = true;
            }

            int VertexNamingIndex = 1;
            if (EdgeName) {
                VertexNamingIndex++;
            }
            EdgeDcl.VertexNameFrom = context.GetChild(VertexNamingIndex).GetText();
            EdgeDcl.VertexNameTo = context.GetChild(VertexNamingIndex+2).GetText();

            // Checks if there is assignments in the Edge (First child is either a varaiblename or a '('
            if ((EdgeName && context.ChildCount > 6) || (!EdgeName && context.ChildCount > 5))
            {
                int i = 4;
                // If there is an Edgename move one token extra forward (to skip the "(")
                if (EdgeName)
                {
                    i++;
                }
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

		public override AbstractNode VisitGraphDclBlock([NotNull] GiraphParser.GraphDclBlockContext context)
		{
			return base.VisitGraphDclBlock(context);
		}

	}
}
