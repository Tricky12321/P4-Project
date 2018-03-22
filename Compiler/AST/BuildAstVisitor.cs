using System;
using Antlr4.Runtime.Misc;
using Compiler.AST.Nodes;
namespace Compiler.AST
{
    internal class BuildAstVisitor : GiraphParserBaseVisitor<AbstractNode>
    {
        public AbstractNode root;
		public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
		{
            root = new StartNode();
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
            FNode.FunctionName = context.children[0].GetText();
            FNode.FunctionReturnType = context.children[2].GetText();
            return FNode;
		}
	}
}
