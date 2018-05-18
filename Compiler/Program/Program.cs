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
using Compiler.CodeGeneration.GenerationCode;
using Giraph.Classes;
using System.Reflection;
using System.Globalization;
namespace Compiler
{
	public static class Program
	{
		private static bool _error = false;
		private static bool _ignoreErrors = false;
		public static void Main(string[] args)
		{
			Compile();
        }

		public static void Compile()
		{
			Stopwatch TotalTimer = new Stopwatch();
			TotalTimer.Start();

			PrintCompilerMessage("Giraph Compiler 1.0.1");

			GiraphParser.StartContext CST = BuildCST("kode.giraph");
			ErrorChecker(_error, 9999, "Code error!");
			AbstractNode AST = BuildAST(CST);
			SymTable SymbolTable = BuildSymbolTable(AST as StartNode);
			if (Utilities.GetOS() != OS.MacOS || true)
			{
				TypeCheck(SymbolTable, AST as StartNode);
			}
			//PrettyPrint(AST as StartNode);
			WriteCodeToFiles(AST as StartNode);
			TotalTimer.Stop();
			PrintCompilerMessage($"Total compile timer: {TotalTimer.ElapsedMilliseconds}ms");
			CompileGeneratedCode(); // Compile the C# code.
		}

		public static AbstractNode BuildAST(GiraphParser.StartContext start)
		{
			Stopwatch AstBuildTimer = new Stopwatch();
			AstBuildTimer.Start();
			AbstractNode ast = new AstBuilder().VisitStart(start);
			AstBuildTimer.Stop();
			PrintCompilerMessage("AstBuilder took: " + AstBuildTimer.ElapsedMilliseconds + "ms");
			return ast;
		}

		public static void PrettyPrint(StartNode start)
		{
			Stopwatch PPTimer = new Stopwatch();
			AstPrettyPrintVisitor PPVisitor = new AstPrettyPrintVisitor();
			PPTimer.Start();
			PPVisitor.VisitRoot(start);
			PPTimer.Stop();
			PrintCompilerMessage($"Pretty Printer took: {PPTimer.ElapsedMilliseconds}ms");
			PrintCompilerMessage(PPVisitor.ProgramCode.ToString(), ConsoleColor.Green);
		}

		public static GiraphParser.StartContext BuildCST(string FilePath)
		{
			TextWriter DefaultOut = Console.Out;
			var sw = new StringWriter();
			Console.SetOut(sw);
			Console.SetError(sw);
			Stopwatch CSTTimer = new Stopwatch();
			CSTTimer.Start();
			string input = File.ReadAllText(Utilities.CurrentPath + "/" + FilePath);
			ICharStream stream = CharStreams.fromstring(input);
			ITokenSource lexer = new GiraphLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);
			GiraphParser parser = new GiraphParser(tokens);
			parser.BuildParseTree = true;
			CSTTimer.Stop();
			var output = parser.start();
			string result = sw.ToString();
			Console.SetOut(DefaultOut);
			Console.SetError(DefaultOut);
			PrintCompilerMessage($"CST Builder took: {CSTTimer.ElapsedMilliseconds}ms");
			Console.WriteLine(result.ToString());
			if (result != "")
			{
				_error = true;
			}
			return output;
		}

		public static GiraphParser.StartContext BuildCSTText(string Text)
		{

			Stopwatch CSTTimer = new Stopwatch();
			CSTTimer.Start();
			string input = File.ReadAllText(Text);
			ICharStream stream = CharStreams.fromstring(input);
			ITokenSource lexer = new GiraphLexer(stream);
			ITokenStream tokens = new CommonTokenStream(lexer);
			GiraphParser parser = new GiraphParser(tokens);
			parser.BuildParseTree = true;
			CSTTimer.Stop();
			PrintCompilerMessage($"CST Builder took: {CSTTimer.ElapsedMilliseconds}ms");
			return parser.start();
		}

		public static SymTable BuildSymbolTable(StartNode node)
		{
			Stopwatch SymbolTableTimer = new Stopwatch();
			SymbolTableTimer.Start();
			AstSymbolTableCreatorVisitor SymbolTable = new AstSymbolTableCreatorVisitor();
			SymbolTable.BuildSymbolTable(node);
			SymbolTableTimer.Stop();
			PrintCompilerMessage("Building Symbol Table took: " + SymbolTableTimer.ElapsedMilliseconds + "ms");
			if (!SymbolTable.MainDefined)
			{
				SymbolTable.SymbolTable.MainUndefined();
			}
			ErrorChecker(SymbolTable.SymbolTable.errorOccured, 1, "Symbol Table");
			return SymbolTable.SymbolTable;
		}

		public static void TypeCheck(SymTable SymbolTable, StartNode node)
		{
			Stopwatch TypeCheckTimer = new Stopwatch();
			TypeCheckTimer.Start();
			AstTypeCheckerVisitor TypeChecker = new AstTypeCheckerVisitor(SymbolTable);
			TypeChecker.VisitRoot(node);
			TypeCheckTimer.Stop();
			PrintCompilerMessage("Type checking took: " + TypeCheckTimer.ElapsedMilliseconds + "ms");
			ErrorChecker(SymbolTable.errorOccured, 2, "TypeChecker");
		}

		public static void CompileGeneratedCode()
		{
			if (Utilities.GetOS() == OS.MacOS || Utilities.GetOS() == OS.Linux)
			{
				if (File.Exists(Utilities.CurrentPath + "/Compiled_Program.exe"))
				{
					File.Delete(Utilities.CurrentPath + "/Compiled_Program.exe");
				}
				string strCmdText = Utilities.CurrentPath + "/CodeGeneration/Program.cs " + Utilities.CurrentPath + "/CodeGeneration/Classes/* /out:" + Utilities.CurrentPath + "/CodeGeneration/program.exe";
				var process = Process.Start("csc", strCmdText);
				process.WaitForExit();
				FinishCompiler();
				Console.WriteLine("Running program...");
				System.Threading.Thread.Sleep(1000);
				strCmdText = Utilities.CurrentPath + "/Compiled_Program.exe";
				var process2 = Process.Start("mono", strCmdText);
				process.WaitForExit();
				System.Threading.Thread.Sleep(1000);

			}
			else if (Utilities.GetOS() == OS.Windows)
			{
				Process process = new Process();
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.FileName = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\MSBuild\\15.0\\Bin\\Roslyn\\csc.exe";
				startInfo.Arguments = Utilities.CurrentPath + "/CodeGeneration/Program.cs " + Utilities.CurrentPath + "/CodeGeneration/Classes/*";
				process.StartInfo = startInfo;
				process.Start();
				process.WaitForExit();
			}
		}

		public static void WriteCodeToFiles(StartNode node)
		{
			Stopwatch WriteTimer = new Stopwatch();
			WriteTimer.Start();
			PrepareCompiler();
			CodeWriter codeWriter = new CodeWriter();
			CodeGenerator codeGenerator = new CodeGenerator(codeWriter);
			codeGenerator.Visit(node);

			codeWriter.FillAll();
			WriteTimer.Stop();
			PrintCompilerMessage($"Writing Code timer: {WriteTimer.ElapsedMilliseconds}ms");
		}

		private static void PrintCompilerMessage(string text, ConsoleColor color = ConsoleColor.Red)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ResetColor(); ;
		}


		public static void ErrorChecker(bool error, int errorCode, string Message)
		{
			if (error && !_ignoreErrors)
			{
				Console.WriteLine("ERROR... " + Message);
				Environment.Exit(errorCode);
			}
		}


		public static void PrepareCompiler() {
			Utilities.CopyAll(Utilities.CurrentPath + "/CodeGeneration",Utilities.CurrentPath + "/CodeGeneration_backup");
		}

		public static void FinishCompiler() {
			File.Move(Utilities.CurrentPath + "/CodeGeneration/Program.exe", Utilities.CurrentPath + "/Compiled_Program.exe");
			Directory.Delete(Utilities.CurrentPath + "/CodeGeneration", true);
			Directory.Move(Utilities.CurrentPath + "/CodeGeneration_backup", Utilities.CurrentPath + "/CodeGeneration");
		}
	}
}
