using NUnit.Framework;
using NUnit;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;
using Compiler;
using Newtonsoft.Json;
using Newtonsoft;
namespace Tests
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
			Program.TestMode = true;
            CST = Program.BuildCST("kode.giraph");
            AST = Program.BuildAST(CST);
            SymbolTable = Program.BuildSymbolTable(AST as StartNode);
        }

        [TestCase("i", ExpectedResult = AllType.INT)]
        [TestCase("d", ExpectedResult = AllType.DECIMAL)]
        [TestCase("g1", ExpectedResult = AllType.GRAPH)]
        [TestCase("vertexColl", ExpectedResult = AllType.VERTEX)]
        [TestCase("va", ExpectedResult = AllType.VERTEX)]
        [TestCase("vb", ExpectedResult = AllType.VERTEX)]
        [TestCase("vc", ExpectedResult = AllType.VERTEX)]
        [TestCase("vd", ExpectedResult = AllType.VERTEX)]
        [TestCase("ve", ExpectedResult = AllType.VERTEX)]
        [TestCase("x", ExpectedResult = AllType.EDGE)]
        [TestCase("y", ExpectedResult = AllType.EDGE)]
        [TestCase("z", ExpectedResult = AllType.EDGE)]
        [TestCase("asdf", ExpectedResult = null)]
        [TestCase("srkald", ExpectedResult = null)]
        [TestCase("anton", ExpectedResult = null)]
        [TestCase("hej med dig", ExpectedResult = null)]
        [TestCase("asdf", ExpectedResult = null)]
        [TestCase("hahaha", ExpectedResult = null)]
        public AllType? IsDeclaredInMain(string VariableName)
        {
            SymbolTable.OpenScope("Main");
            var test = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope();
            return test;
        }
        [TestCase("asdf", AllType.EDGE, ExpectedResult = null)]
        [TestCase("srkald", AllType.EDGE, ExpectedResult = null)]
        [TestCase("anton", AllType.EDGE, ExpectedResult = null)]
        [TestCase("hej med dig", AllType.EDGE, ExpectedResult = null)]
        [TestCase("asdf", AllType.EDGE, ExpectedResult = null)]
        [TestCase("hahaha", AllType.EDGE, ExpectedResult = null)]
        [TestCase("v", AllType.EDGE, ExpectedResult = AllType.INT)]
        [TestCase("d", AllType.EDGE, ExpectedResult = AllType.DECIMAL)]
        [TestCase("gr1", AllType.EDGE, ExpectedResult = AllType.GRAPH)]
        [TestCase("vertEdge", AllType.EDGE, ExpectedResult = AllType.VERTEX)]
        [TestCase("vertEdge1", AllType.GRAPH, ExpectedResult = AllType.VERTEX)]
        [TestCase("vertEdge2", AllType.GRAPH, ExpectedResult = AllType.VERTEX)]
        [TestCase("length", AllType.EDGE, ExpectedResult = AllType.INT)]
        public AllType? ExtensionCheck(string VariableName, AllType Class)
        {
            return SymbolTable.GetAttributeType(VariableName, Class);
        }

        [TestCase("val", AllType.GRAPH, ExpectedResult = AllType.DECIMAL)]
        [TestCase("valInt", AllType.GRAPH, ExpectedResult = AllType.INT)]
        [TestCase("v", AllType.GRAPH, ExpectedResult = AllType.DECIMAL)]
        [TestCase("vi", AllType.GRAPH, ExpectedResult = AllType.INT)]

        public AllType? ExtensionCheckLongAndShort(string VariableName, AllType Class)
        {
            return SymbolTable.GetAttributeType(VariableName, Class);
        }


        [TestCase("Test", "i", ExpectedResult = AllType.INT)]
        [TestCase("Test", "d", ExpectedResult = AllType.DECIMAL)]
        [TestCase("Test", "g2", ExpectedResult = AllType.GRAPH)]
        [TestCase("Main", "g2", ExpectedResult = AllType.GRAPH)]
        [TestCase("Test", "g1", ExpectedResult = null)]

        [TestCase("Test", "asdf", ExpectedResult = null)]
        [TestCase("Test", "srkald", ExpectedResult = null)]
        [TestCase("Test", "anton", ExpectedResult = null)]
        [TestCase("Test", "hej med dig", ExpectedResult = null)]
        [TestCase("Test", "asdf", ExpectedResult = null)]
        [TestCase("Test", "hahaha", ExpectedResult = null)]

        [TestCase("Main", "asdf", ExpectedResult = null)]
        [TestCase("Main", "srkald", ExpectedResult = null)]
        [TestCase("Main", "anton", ExpectedResult = null)]
        [TestCase("Main", "hej med dig", ExpectedResult = null)]
        [TestCase("Main", "asdf", ExpectedResult = null)]
        [TestCase("Main", "hahaha", ExpectedResult = null)]

        public AllType? IsDeclaredInOtherFunciton(string function, string variable)
        {
            SymbolTable.OpenScope(function);
            var test = SymbolTable.RetrieveSymbol(variable);
            SymbolTable.CloseScope();
            return test;
        }

        // Insert into symbolTable
        [TestCase("test1", AllType.DECIMAL, true, "asdf")]
        [TestCase("test2", AllType.GRAPH, true, "asdf")]
        [TestCase("test3", AllType.VERTEX, true, "asdf")]
        [TestCase("test4", AllType.EDGE, true, "asdf")]
        [TestCase("test5", AllType.INT, true, "asdf")]
        [TestCase("test6", AllType.BOOL, true, "asdf")]
        [TestCase("test7", AllType.STRING, true, "asdf")]
        public void InsertIntoSymbolTableScope(string varaibleName, AllType type, bool IsCollection, string ScopeName)
        {
            SymbolTable.OpenScope(ScopeName);
            SymbolTable.EnterSymbol(varaibleName, type, IsCollection);
            bool SymResolved;
            var NewType = SymbolTable.RetrieveSymbol(varaibleName, out SymResolved);
            SymbolTable.CloseScope();
            if (NewType == type && SymResolved == IsCollection)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestCase("test1", AllType.DECIMAL, true)]
        [TestCase("test2", AllType.GRAPH, true)]
        [TestCase("test3", AllType.VERTEX, true)]
        [TestCase("test4", AllType.EDGE, true)]
        [TestCase("test5", AllType.INT, true)]
        [TestCase("test6", AllType.BOOL, true)]
        [TestCase("test7", AllType.STRING, true)]
        public void InsertIntoSymbolTableGlobal(string varaibleName, AllType type, bool IsCollection)
        {
            SymbolTable.EnterSymbol(varaibleName, type, IsCollection);
            var NewType = SymbolTable.RetrieveSymbol(varaibleName);
            Assert.AreEqual(NewType, type);
        }

        [TestCase("decimalInIf", ExpectedResult = AllType.DECIMAL)]
        [TestCase("vertexInIf", ExpectedResult = AllType.VERTEX)]
        [TestCase("edgeInIf", ExpectedResult = AllType.EDGE)]
        [TestCase("graphInIf", ExpectedResult = AllType.GRAPH)]
        public AllType? CheckIfScopesInMain(string VariableName)
        {
            SymbolTable.OpenScope("Main");
            SymbolTable.OpenScope(BlockType.IfStatement);
            var returnVal = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            return returnVal;
        }

        [TestCase("NesteddecimalInIf", ExpectedResult = AllType.DECIMAL)]
        [TestCase("NestedvertexInIf", ExpectedResult = AllType.VERTEX)]
        [TestCase("NestededgeInIf", ExpectedResult = AllType.EDGE)]
        [TestCase("NestedgraphInIf", ExpectedResult = AllType.GRAPH)]
        public AllType? CheckNestedIfScopesInMain(string VariableName)
        {
            SymbolTable.OpenScope("Main");
            SymbolTable.OpenScope(BlockType.IfStatement);
            SymbolTable.OpenScope(BlockType.IfStatement);
            var returnVal = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            return returnVal;
        }

        [TestCase("decimalInFor", ExpectedResult = AllType.DECIMAL)]
        [TestCase("vertexInFor", ExpectedResult = AllType.VERTEX)]
        [TestCase("edgeInFor", ExpectedResult = AllType.EDGE)]
        [TestCase("graphInFor", ExpectedResult = AllType.GRAPH)]
        public AllType? CheckForScopesInMain(string VariableName)
        {
            SymbolTable.OpenScope("Main");
            SymbolTable.OpenScope(BlockType.ForLoop);
            var returnVal = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            return returnVal;
        }

        [TestCase("NesteddecimalInFor", ExpectedResult = AllType.DECIMAL)]
        [TestCase("NestedvertexInFor", ExpectedResult = AllType.VERTEX)]
        [TestCase("NestededgeInFor", ExpectedResult = AllType.EDGE)]
        [TestCase("NestedgraphInFor", ExpectedResult = AllType.GRAPH)]
        public AllType? CheckNestedForScopesInMain(string VariableName)
        {
            SymbolTable.OpenScope("Main");
            SymbolTable.OpenScope(BlockType.ForLoop);
            SymbolTable.OpenScope(BlockType.ForLoop);
            var returnVal = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            SymbolTable.CloseScope();
            return returnVal;
        }

        [TestCase("Test1", AllType.DECIMAL, AllType.INT)]
        [TestCase("Test2", AllType.GRAPH, AllType.GRAPH)]
        [TestCase("Test3", AllType.VERTEX, AllType.EDGE)]
        [TestCase("Test4", AllType.EDGE, AllType.INT)]
        [TestCase("Test5", AllType.INT, AllType.VERTEX)]
        [TestCase("Test6",AllType.INT, AllType.GRAPH)]
        public void Check2Variablesin2ScopesWithSameName(string VariableName, AllType FirstType, AllType SecondType)
        {
            SymbolTable.OpenScope("Test");
            SymbolTable.OpenScope(BlockType.IfStatement);
            var FirstRealType = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope(); // Closing the first if statement
            SymbolTable.OpenScope(BlockType.IfStatement);
            var SecondRealType = SymbolTable.RetrieveSymbol(VariableName);
            SymbolTable.CloseScope(); // Closing the last if statement
            SymbolTable.CloseScope(); // Closing the test method scope  
            if (FirstRealType == FirstType && SecondType == SecondRealType) {
                Assert.Pass();
            } else {
                Assert.Fail();
            }
        }



    }
}
