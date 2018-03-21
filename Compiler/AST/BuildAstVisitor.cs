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
            AbstractNode PlaceHolder = VisitChildren(context);
            root.AdoptChildren(PlaceHolder);
            return root;
		}

		public override AbstractNode VisitProgram([NotNull] GiraphParser.ProgramContext context)
		{
            ProgramNode PNode = new ProgramNode();
            PNode.AdoptChildren(VisitChildren(context));
            return PNode;
		}
	}
}
