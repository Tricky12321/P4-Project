using NUnit.Framework;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;
using Compiler;
namespace Unittests
{
    [TestFixture()]
    public class SymbolTableTests
    {
        GiraphParser.StartContext CST;
        AbstractNode AST;
        SymTable SymbolTable;
        [SetUp]
        public void Init()
        {
            CST = Program.BuildCST("kode.giraph");
            AST = Program.BuildAST(CST);
            SymbolTable = Program.BuildSymbolTable(AST as StartNode);
        }

        public SymbolTableTests()
        {
        }
    }
}
