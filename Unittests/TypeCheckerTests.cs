using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using NUnit.Framework;
using System;
using Compiler;
namespace Unittests
{
    [TestFixture()]
    public class TypeCheckerTest
    {
        GiraphParser.StartContext CST;
        AbstractNode AST;
        SymTable SymbolTable;

        [SetUp]
        public void Init()
        {
            CST = Program.BuildCST("kode_TypeChecker.giraph");
            AST = Program.BuildAST(CST);
            SymbolTable = Program.BuildSymbolTable(AST as StartNode);
            Program.TypeCheck(SymbolTable, AST as StartNode);
        }

        [Test()]
        public void hehetype()
        {
            Assert.IsTrue(true);
        }




    }
}