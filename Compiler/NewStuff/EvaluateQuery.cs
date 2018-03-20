using System;
namespace Compiler.NewStuff
{
    internal class EvaluateQuery : AstVisitor<bool>
    {
		public override bool Visit(QueryNode node)
		{
            Console.WriteLine("DU FANDT EN QUERYNODE");
            return true;
		}

        public override bool Visit(ReturnQueryNode node)
		{
            Console.WriteLine("DU FANDT EN RETURNQUERYNODE");
            return true;
			
		}

        public override bool Visit(NoReturnQuery node)
		{
            Console.WriteLine("DU FANDT EN NORETURNQUERYNODE");
            return true;
			
		}
	}
}
