using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
				MyParseMethod(args[0]);
            } else {
                MyParseMethod("");
            }
        }

        public static void MyParseMethod(string FilePath)
        {
            String input = "RUN asdf WITH ();";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            //IParseTree tree = parser.start();
            try
            {
                var cst = parser.start();
                var ast = new NewStuff.BuildAstVisitor().VisitStart(cst);
                var value = new NewStuff.EvaluateQuery().Visit(ast);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
        }
    }
}
