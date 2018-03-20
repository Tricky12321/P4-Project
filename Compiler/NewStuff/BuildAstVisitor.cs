using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Compiler.NewStuff
{
    internal class BuildAstVisitor : GiraphParserBaseVisitor<AbstractNode>
    {
		/*public override AbstractNode VisitQuery([NotNull] GiraphParser.QueryContext context)
		{
            Console.WriteLine("QUERY NODE YAAAY ^^");
            return new AbstractNode;
		}
		*/
	}
}
