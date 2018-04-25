using NUnit.Framework;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
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


        [TestCase("Main")]
        [TestCase("Test")]
        [TestCase("TestFunc")]
        [TestCase("TestFuncTest")]
        public void CheckFunctionExist(string FunctionName) {
            var Counter = AST.Children.Where(x => x is FunctionNode).ToList()
                             .Where(x => (x as FunctionNode).Name == FunctionName)
                             .Count();
            if (Counter == 1) {
                Assert.Pass();
            } else {
				Assert.Fail();
            }
        }

        [TestCase("d", AllType.DECIMAL, "Main", ExpectedResult = true)]
        [TestCase("i", AllType.INT, "Main", ExpectedResult = true)]
        [TestCase("vertexColl", AllType.VERTEX, "Main", ExpectedResult = true)]


        [TestCase("asdf", AllType.VERTEX, "something something", ExpectedResult = false)]
        [TestCase("anton", AllType.INT, "ogjfpbjwfpjwf", ExpectedResult = false)]
        [TestCase("123123", AllType.DECIMAL, "asdf", ExpectedResult = false)]
        [TestCase("pkasnd", AllType.GRAPH, "asdf", ExpectedResult = false)]
        [TestCase("something something",AllType.BOOL, "dont know", ExpectedResult = false)]
        public bool CheckDeclarationNode(string VariableName, AllType ExpectedType, string Function) {
            var Start = AST.Children.Where(x => (x is FunctionNode) && (x as FunctionNode).Name == Function);
            if (Start.Count() == 0) {
                return false;
            }
            var Next = Start.First().Children.Where(x => ((x is VariableDclNode) || (x is DeclarationNode)) && x.Name == VariableName);
            if (Next.Count() == 0) {
                return false;
            }
            if (Next.First().Type_enum == ExpectedType) {
                return true;
            } else {
                return false;
            }
        }

        [TestCase("g1", "Main", ExpectedResult = true)]
        [TestCase("g2", "Main", ExpectedResult = true)]
        [TestCase("g23", "Main", ExpectedResult = false)]
        public bool CheckGraphDclNode(string VariableName, string Function)
        {
            var Start = AST.Children.Where(x => (x is FunctionNode) && (x as FunctionNode).Name == Function).First();
            var Next = Start.Children.Where(x => (x is GraphNode) && x.Name == VariableName).Count();
            if (Next == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
