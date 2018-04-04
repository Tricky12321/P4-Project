using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using Compiler.AST;
using Compiler.AST.SymbolTable;
using Compiler.AST.Nodes;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var CST = BuildCST("code.giraph");
            var AST = BuildAST(CST);
            var SymbolTable = BuildSymbolTable(AST as StartNode);
            PrettyPrint(AST as StartNode);
            Console.ReadKey();
        }

        public static AbstractNode BuildAST(GiraphParser.StartContext start)
        {
            AbstractNode ast = new AstBuilder().VisitStart(start);
            return ast;
        }

        public static void PrettyPrint(StartNode start) {
            AstPrettyPrintVisitor visitor = new AstPrettyPrintVisitor();
            visitor.VisitRoot(start);
            Console.WriteLine(visitor.ProgramCode);
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

        public static AstSymbolTableCreatorVisitor BuildSymbolTable(StartNode node) {
            AstSymbolTableCreatorVisitor SymbolTable = new AstSymbolTableCreatorVisitor();
            SymbolTable.VisitRoot(node);
            return SymbolTable;
        }
    }
}
