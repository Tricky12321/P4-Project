using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using NUnit.Framework;
using System;
using Compiler;
using System.Collections.Generic;

namespace Unittests
{
    [TestFixture()]
    public class TypeCheckerTest
    {
        GiraphParser.StartContext CST;
        AbstractNode AST;
        SymTable SymbolTable;
        List<string> errorlist;

        [SetUp]
        public void Init()
        {
            CST = Program.BuildCST("C:\\Users\\Ezzi\\Source\\Repos\\P4-Project\\Unittests\\kode_TypeChecker.giraph");
            AST = Program.BuildAST(CST);
            SymbolTable = Program.BuildSymbolTable(AST as StartNode);
            Program.TypeCheck(SymbolTable, AST as StartNode);
            errorlist = SymbolTable.getTypeCheckErrorList();
        }

        [Test()]
        public void hehetype()
        {
            Assert.IsTrue(true);
        }
        




    }
}