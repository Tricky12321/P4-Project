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
using System.Diagnostics;
using System.Management;
using Compiler.CodeGeneration.GenerationCode;
using Giraph.Classes;
namespace Compiler
{
    class Program
    {
        public static void Main(string[] args)
        {
            Compile();
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
            WriteCodeToFiles(AST as StartNode);
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
            if (!SymbolTable.MainDefined) {
                SymbolTable.SymbolTable.MainUndefined();
            }

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
            if (Utilities.GetOS() == OS.MacOS || Utilities.GetOS() == OS.Linux) {
				string strCmdText = "CodeGeneration/Program.cs CodeGeneration/Classes/*";
				Process.Start("csc", strCmdText);
                strCmdText = "Program.exe";
                Process.Start("mono", strCmdText);
            } else if (Utilities.GetOS() == OS.Windows) {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\MSBuild\\15.0\\Bin\\Roslyn\\csc.exe";
                startInfo.Arguments = "CodeGeneration/Program.cs CodeGeneration/Classes/*";
                process.StartInfo = startInfo;
                process.Start();
            } 
        }

        public static void WriteCodeToFiles(StartNode node) {
            Stopwatch WriteTimer = new Stopwatch();
            WriteTimer.Start();
            CodeWriter codeWriter = new CodeWriter();
            CodeGenerator codeGenerator = new CodeGenerator(codeWriter);
            codeGenerator.Visit(node);
            codeWriter.FillAll();
            WriteTimer.Stop();
            Console.WriteLine($"Writing Code timer: {WriteTimer.ElapsedMilliseconds}ms");
        }


        public static void Test() {
            
        }
    }
}
