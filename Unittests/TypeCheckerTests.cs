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
            CST = Program.BuildCST("kode_TypeChecker.giraph");
            AST = Program.BuildAST(CST);
            SymbolTable = Program.BuildSymbolTable(AST as StartNode);
            Program.TypeCheck(SymbolTable, AST as StartNode);
            errorlist = SymbolTable.getTypeCheckErrorList();
        }

        [TestCase("The parameter: parameter cannot be of type void 115:0")]
        public void functionParameterCantBeVoidTest(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        
        [TestCase("There is a type mismatch in the expression on 56:16")]
        [TestCase("There is a type mismatch in the condition on Line number 57:27")]
        public void setQueryErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("Type of attribute must be of type Integer or Decimal 59:1")]
        [TestCase("Given attribute: 'intSom2' is not extended on Class: EDGE 60:1")]
        [TestCase("If a attribute is provided for assortment, the specified collection must not be of type Decimal or Integer 61:1")]
        [TestCase("If no attribute is provided for assortment, the specified collection must be of type Decimal or Integer 62:1")]
        [TestCase("The variable retrieved from: g1.intSom is not of type collection 63:1")]
        public void extractmaxErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("Type of attribute must be of type Integer or Decimal 65:1")]
        [TestCase("Given attribute: 'intSom2' is not extended on Class: EDGE 66:1")]
        [TestCase("If a attribute is provided for assortment, the specified collection must not be of type Decimal or Integer 67:1")]
        [TestCase("If no attribute is provided for assortment, the specified collection must be of type Decimal or Integer 68:1")]
        [TestCase("The variable retrieved from: g1.intSom is not of type collection 69:1")]
        public void extractminErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("The variable retrieved from: timint is not of type collection 71:25")]
        public void selectallErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("The variable retrieved from: timint is not of type collection 72:18")]
        public void selectErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("Variable  and tim are missmatch of types. Line number 74:17")]
        [TestCase("Variable  and tim are missmatch of types. Line number 75:1")]
        public void pushErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("The variable retrieved from: endnuentim is not of type collection 76:1")]
        public void popErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("Variable  and tim are missmatch of types. Line number 77:20")]
        [TestCase("Variable  and tim are missmatch of types. Line number 78:1")]
        public void enqueueErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("The variable retrieved from: endnuentim is not of type collection 79:1")]
        public void dequeueErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("The declaration set:(vvcv) cannot be added to the collection in the graph! 83:1")]
        [TestCase("Variable  and tim are missmatch of types. Line number 84:5")]
        [TestCase("Variable System.Collections.Generic.List`1[Compiler.AST.Nodes.AbstractNode] and g1 are missmatch of types. Line number 85:5")]
        [TestCase("Target variable: endnuentim is not of type collection 86:5")]
        public void AddErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("There is a type mismatch in the expression on 88:25")]
        public void graphdclVertexErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("There is a type mismatch in the expression on 89:10")]
        [TestCase("There is a type mismatch in the expression on 90:34")]
        public void graphdclEdgeErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("Variable 'Directed' and  are missmatch of types. Line number 96:19")]
        public void graphSetErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void declarationsErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void BoolComparisonErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void ExpressionErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void ReturnErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void forLoopErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void foreachErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void variableDclErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void printErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void runErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void predicateCallErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void removeErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("")]
        public void removeAllErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }






    }
}