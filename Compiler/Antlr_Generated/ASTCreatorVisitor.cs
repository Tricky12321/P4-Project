using System;
using Antlr4.Runtime.Misc;
using Compiler.Nodes;


namespace Compiler
{
    public class ASTCreator<AbstractNode> : GiraphParserBaseVisitor<AbstractNode>
    {
        public override AbstractNode VisitStart([NotNull] GiraphParser.StartContext context)
		{
            return VisitChildren(context);
        }

        public override AbstractNode VisitProgram([NotNull] GiraphParser.ProgramContext context)
		{
            return VisitChildren(context);
		}

        public override AbstractNode VisitFunctionDcl([NotNull] GiraphParser.FunctionDclContext context)
		{
            return base.VisitFunctionDcl(context);
		}

        public override AbstractNode VisitVariable([NotNull] GiraphParser.VariableContext context)
        {
            return base.VisitVariable(context);
        }
	}
}
