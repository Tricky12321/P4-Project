using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
//using Compiler.Nodes;

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
            String input = "Main -> VOID (VERTEX TestV, VERTEX TestV2) {}";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            var cst = parser.start();
            var ast = new AST.BuildAstVisitor().VisitStart(cst);
            //ASTCreator<AbstractNode> ASTCreator = new ASTCreator<AbstractNode>();
            //ASTCreator.VisitStart(cst);


            Console.WriteLine();
        }
    }
}
