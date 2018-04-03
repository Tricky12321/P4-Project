using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using Compiler.AST;
using Compiler.AST.SymbolTable;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAST("code.giraph");
        }

        public static void BuildAST(string FilePath)
        {
            var ast = new AstBuilder().VisitStart(BuildCST(FilePath));
            var prettyvisitor = new AstPrettyPrintVisitor();
            prettyvisitor.VisitRoot(ast);
            Console.WriteLine(prettyvisitor.ProgramCode);
            /*
            var symboltableVisitor = new AstSymbolTableCreatorVisitor();
            var typecheckerVisitor = new AstTypeCheckerVisitor(symboltableVisitor._symbolTable);
            */
    }

        public static GiraphParser.StartContext BuildCST(string FilePath) {
            string input = File.ReadAllText(FilePath);
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            return parser.start();
        }
    }
}
