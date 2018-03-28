using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using Compiler.AST;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            MyParseMethod("code.giraph");
        }

        public static void MyParseMethod(string FilePath)
        {
            string input = File.ReadAllText(FilePath);
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            var cst = parser.start();
            var ast = new AstBuilder().VisitStart(cst);

            /*var visitor = new AstPrettyPrintVisitor();
            visitor.VisitRoot(ast);
            Console.WriteLine(visitor.ProgramCode);*/
        }
    }
}
