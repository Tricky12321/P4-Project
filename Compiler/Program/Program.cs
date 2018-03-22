﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
//using Compiler.Nodes;

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
            //String input = "Main -> VOID (VERTEX TestV, VERTEX TestV2) {}";
            string input = File.ReadAllText(FilePath);
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            var cst = parser.start();
            var ast = new AST.AstBuilder().VisitStart(cst);
            //ASTCreator<AbstractNode> ASTCreator = new ASTCreator<AbstractNode>();
            //ASTCreator.VisitStart(cst);


            Console.WriteLine();
        }
    }
}
