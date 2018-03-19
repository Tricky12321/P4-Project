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
            MyParseMethod();
        }

        public static void MyParseMethod()
        {
            String input = "Main -> VOID () {}";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.start();
        }
    }
}
