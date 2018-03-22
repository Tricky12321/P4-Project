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
            FNode.ReturnType = context.children[2].GetText();
            int i = 0;
            while (context.children[i].GetText() != ")") {
                var Child = context.children[4].GetChild(i);
                if (Child != null && Child.GetText() != ",") {
					var first = context.children[4].GetChild(i).GetChild(0).GetText();
					var second = context.children[4].GetChild(i).GetChild(1).GetText();
					FNode.AddParameter(first, second);
                }
                i++;
            }
			return FNode;
        }
	}
}
