using NUnit.Framework;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;
using Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;

namespace Unittests
{
    [TestFixture()]
    public class AstBuilderTests
    {

        GiraphParser.StartContext CST;
        AbstractNode AST;
        [SetUp]
        public void Init()
        {
            CST = Program.BuildCST("kode_ast.giraph");
            AST = Program.BuildAST(CST);
        }

        [Test()]
        public void CheckStartNodes()
        {
            if (AST is StartNode) {
                Assert.Pass();
            } else {
                Assert.Fail();
            }
        }

        [TestCase("v", AllType.EDGE)]
        [TestCase("v", AllType.VERTEX)]
        [TestCase("d", AllType.EDGE)]
        [TestCase("gr1", AllType.EDGE)]
        [TestCase("vertEdge", AllType.EDGE)]
        [TestCase("vertVertex", AllType.VERTEX)]
        [TestCase("length",AllType.EDGE)]
        public void CheckExtendNotes(string extensionName, AllType Class) {
            var ExtendNodes = AST.Children.Where(x => x is ExtendNode).ToList();
            var extraction = ExtendNodes.Where(x => (x as ExtendNode).ExtensionName == extensionName
                                  && (x as ExtendNode).ClassToExtend_enum == Class);
            if (extraction.Count() == 1) {
                Assert.Pass();
            } else {
                Assert.Fail();
            }
        }

        [TestCase("val", "v", AllType.GRAPH)]
        [TestCase("valInt","vi",AllType.GRAPH)]
        public void CheckExtendNotesShortNameIncluded(string LongName, string ShortName,AllType Class)
        {
            var ExtendNodes = AST.Children.Where(x => x is ExtendNode).ToList();
            var extraction = ExtendNodes.Where(x => (x as ExtendNode).ExtensionName == LongName
                                               && (x as ExtendNode).ExtensionShortName == ShortName
                                  && (x as ExtendNode).ClassToExtend_enum == Class);
            if (extraction.Count() == 1)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }


        [TestCase("Main", AllType.VOID)]
        [TestCase("Test", AllType.VOID)]
        [TestCase("TestFunc", AllType.BOOL)]
        [TestCase("TestFuncTest",AllType.GRAPH)]
        public void CheckFunctionExist(string FunctionName, AllType ExpectedType) {
            
        }

    }
}
