using NUnit.Framework;
using NUnit;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;


namespace Tests
{
    [TestFixture()]
    public class AstBuilderTests
    {

        GiraphParser.StartContext CST;
        AbstractNode AST;
        [SetUp]
        public void Init()
        {
			Program.TestMode = true;
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
        [TestCase("msomething something",AllType.BOOL, "dont know", ExpectedResult = false)]
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

        [TestCase("p", AllType.BOOL, ExpectedResult = true)]
        [TestCase("as", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("bs", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("ds", AllType.DECIMAL, ExpectedResult = true)]
        [TestCase("es", AllType.INT, ExpectedResult = true)]
        [TestCase("gs", AllType.EDGE, ExpectedResult = true)]

        [TestCase("gs", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("es", AllType.DECIMAL, ExpectedResult = false)]
        [TestCase("gs", AllType.INT, ExpectedResult = false)]
        [TestCase("es", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("es", AllType.EDGE, ExpectedResult = false)]
        [TestCase("gs", AllType.BOOL, ExpectedResult = false)]
        [TestCase("as", AllType.EDGE, ExpectedResult = false)]
        public bool CheckPredicateDefinitions1Parameter(string PredicateName, AllType ParameterType) {
            var Start = AST.Children.Where(x => (x is PredicateNode) && (x as PredicateNode).Name == PredicateName).First();
            if (Start != null) {
				if ((Start as PredicateNode).Parameters[0].Type_enum == ParameterType) {
					return true;
				} else {
					return false;
				}
            }
            return false;
        }

        [TestCase("psv", AllType.GRAPH, AllType.INT, ExpectedResult = true)]
        [TestCase("psg", AllType.EDGE, AllType.DECIMAL, ExpectedResult = true)]
        [TestCase("psd", AllType.VERTEX, AllType.INT, ExpectedResult = true)]
        [TestCase("psa", AllType.BOOL, AllType.BOOL, ExpectedResult = true)]
        [TestCase("pse", AllType.DECIMAL, AllType.EDGE, ExpectedResult = true)]

        [TestCase("psg", AllType.GRAPH, AllType.GRAPH, ExpectedResult = false)]
        [TestCase("psa", AllType.DECIMAL, AllType.GRAPH, ExpectedResult = false)]
        [TestCase("psd", AllType.INT,AllType.EDGE, ExpectedResult = false)]
        [TestCase("psg", AllType.VERTEX,AllType.GRAPH, ExpectedResult = false)]
        [TestCase("psa", AllType.EDGE,AllType.GRAPH, ExpectedResult = false)]
        [TestCase("psd", AllType.BOOL,AllType.EDGE, ExpectedResult = false)]
        [TestCase("psa", AllType.EDGE,AllType.GRAPH, ExpectedResult = false)]
        public bool CheckPredicateDefinitions2Parameters(string PredicateName, AllType ParameterType1, AllType ParameterType2)
        {
            var Start = AST.Children.Where(x => (x is PredicateNode) && (x as PredicateNode).Name == PredicateName).First();
            if (Start != null)
            {
                if ((Start as PredicateNode).Parameters[0].Type_enum == ParameterType1 && (Start as PredicateNode).Parameters[1].Type_enum == ParameterType2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
