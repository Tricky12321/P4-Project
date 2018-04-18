﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;
using Compiler.AST;
using Compiler.AST.SymbolTable;
using Compiler.AST.Nodes;
using System.Diagnostics;
using System.Management;
namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Compile();
            //CompileGeneratedCode();
        }

        private static OS GetOS() {
            if (Utilities.IsWindows) {
                return OS.Windows;
            } else if (Utilities.IsMacOS) {
                return OS.MacOS;
            } else if (Utilities.IsLinux) {
                return OS.Linux;
            } else {
                return OS.Unknown;
            }
        }

        public static void Compile() {
            Stopwatch TotalTimer = new Stopwatch();
            TotalTimer.Start();
            Console.WriteLine("Giraph Compiler 5000");
            GiraphParser.StartContext CST = BuildCST("kode.giraph");
            AbstractNode AST = BuildAST(CST);
            SymTable SymbolTable = BuildSymbolTable(AST as StartNode);
            TypeCheck(SymbolTable, AST as StartNode);
            //PrettyPrint(AST as StartNode);
            TotalTimer.Stop();
            Console.WriteLine($"Total compile timer: {TotalTimer.ElapsedMilliseconds}ms");
        }

        public static AbstractNode BuildAST(GiraphParser.StartContext start)
        {
            Stopwatch AstBuildTimer = new Stopwatch();
            AstBuildTimer.Start();
            AbstractNode ast = new AstBuilder().VisitStart(start);
            AstBuildTimer.Stop();
            Console.WriteLine("AstBuilder took: " + AstBuildTimer.ElapsedMilliseconds + "ms");
            return ast;
        }

        public static void PrettyPrint(StartNode start)
        {   
            Stopwatch PPTimer = new Stopwatch();
            AstPrettyPrintVisitor PPVisitor = new AstPrettyPrintVisitor();
            PPTimer.Start();
            PPVisitor.VisitRoot(start);
            PPTimer.Stop();
            Console.WriteLine($"Pretty Printer took: {PPTimer.ElapsedMilliseconds}ms");
            Console.WriteLine(PPVisitor.ProgramCode);
        }

        public static GiraphParser.StartContext BuildCST(string FilePath) {
            Stopwatch CSTTimer = new Stopwatch();
            CSTTimer.Start();
            string input = File.ReadAllText(FilePath);
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new GiraphLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            GiraphParser parser = new GiraphParser(tokens);
            parser.BuildParseTree = true;
            CSTTimer.Stop();
            Console.WriteLine($"CST Builder took: {CSTTimer.ElapsedMilliseconds}ms");
            return parser.start();
        }

        public static SymTable BuildSymbolTable(StartNode node) {
            Stopwatch SymbolTableTimer = new Stopwatch();
            SymbolTableTimer.Start();
            AstSymbolTableCreatorVisitor SymbolTable = new AstSymbolTableCreatorVisitor();
            SymbolTable.BuildSymbolTable(node);
            SymbolTableTimer.Stop();
            Console.WriteLine("Building Symbol Table took: "+SymbolTableTimer.ElapsedMilliseconds + "ms");
            return SymbolTable.SymbolTable;
        }

        public static void TypeCheck(SymTable SymbolTable, StartNode node) {
            Stopwatch TypeCheckTimer = new Stopwatch();
            TypeCheckTimer.Start();
            AstTypeCheckerVisitor TypeChecker = new AstTypeCheckerVisitor(SymbolTable);
            TypeChecker.VisitRoot(node);
            TypeCheckTimer.Stop();
            Console.WriteLine("Type checking took: " + TypeCheckTimer.ElapsedMilliseconds + "ms");
        }

        public static void CompileGeneratedCode() {
            if (File.Exists("CodeGeneration/Program.exe")) {
                File.Delete("CodeGeneration/Program.exe");
            }

            if (GetOS() == OS.MacOS || GetOS() == OS.Linux) {
				string strCmdText;
                strCmdText = "CodeGeneration/Program.cs CodeGeneration/Classes/*";
				Process.Start("mcs", strCmdText);
            } else if (GetOS() == OS.Windows) {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "csc.exe";
                startInfo.Arguments = "CodeGeneration/Program.cs CodeGeneration/Classes/*";
                process.StartInfo = startInfo;
                process.Start();
            } 
        }


    }
}
