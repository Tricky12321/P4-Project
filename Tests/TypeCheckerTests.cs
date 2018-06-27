using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using NUnit.Framework;
using System;
using Compiler;
using System.Collections.Generic;

namespace Tests
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
			Program.TestMode = true;
			CST = Program.BuildCST("kode_TypeChecker.giraph");
			AST = Program.BuildAST(CST);
			SymbolTable = Program.BuildSymbolTable(AST as StartNode);
 			Program.TypeCheck(SymbolTable, AST as StartNode);
			errorlist = SymbolTable.getTypeCheckErrorList();
		}
		[TestCase("Actual parameter:  and formal parameter: parameter are a type missmatch 142:4")]
        [TestCase("Actual parameter: k and formal parameter: parameter are a type missmatch 139:4")]
        
		public void ActualParameter(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
		[TestCase("Collections are illigal in expressions 107:17")]
		public void CollectionIlligal(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
		[TestCase("Declaration can not be of type void! 114:4")]
        [TestCase("Declaration can not be of type void! 154:27")]
        [TestCase("Declaration can not be of type void! 155:18")]
        [TestCase("Declaration can not be of type void! 70:28")]
        [TestCase("Declaration can not be of type void! 71:21")]
        
		public void DeclarationVoid(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
		[TestCase("Expected a collection 156:4")]
        [TestCase("Expected a collection 157:4")]
        [TestCase("Expected a collection 75:4")]
        [TestCase("Expected a collection 78:4")]
		public void ExpectedCollection(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }

        [TestCase("g1.intSom is not a collection, and therefore remove is not able to be used 68:4")]
		public void ExtendedCollectionError(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("hej2 is not a collection, and therefore remove is not able to be used 174:4")]
        [TestCase("hej2 is not a collection, and therefore remove is not able to be used 175:4")]
		public void NotCollection(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("i is not a collection, and therefore remove is not able to be used 148:4")]
        [TestCase("i is not a collection, and therefore remove is not able to be used 149:4")]
		public void NotACollection(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("Invalid number of parameters in function call 140:4")]
        [TestCase("Invalid number of parameters in function call 141:4")]
        [TestCase("Invalid number of parameters in function call 143:4")]
        [TestCase("Invalid number of parameters in function call 177:4")]
		public void InvalidNumberOfParamters(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("Parameters cannot be of type void113:0")]
        [TestCase("Parameters cannot be of type void152:0")]
		public void ParametersType(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("Target variable: endnuentim is not of type collection 85:8")]
		public void TargetVariable(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("The variable retrieved from: hej2 is not of type collection 154:27")]
        [TestCase("The variable retrieved from: hej2 is not of type collection 155:18")]
        [TestCase("The variable retrieved from: timint is not of type collection 70:28")]
        [TestCase("The variable retrieved from: timint is not of type collection 71:21")]
		public void VariableRetrivedFrom(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("There is a type mismatch or illigal cast 102:19")]
        [TestCase("There is a type mismatch or illigal cast 114:4")]
        [TestCase("There is a type mismatch or illigal cast 117:4")]
        [TestCase("There is a type mismatch or illigal cast 134:19")]
        [TestCase("There is a type mismatch or illigal cast 154:27")]
        [TestCase("There is a type mismatch or illigal cast 164:30")]
        [TestCase("There is a type mismatch or illigal cast 55:19")]
        [TestCase("There is a type mismatch or illigal cast 56:30")]
        [TestCase("There is a type mismatch or illigal cast 70:28")]
        [TestCase("There is a type mismatch or illigal cast 71:21")]
        [TestCase("There is a type mismatch or illigal cast 73:9")]
        [TestCase("There is a type mismatch or illigal cast 74:9")]
        [TestCase("There is a type mismatch or illigal cast 76:12")]
        [TestCase("There is a type mismatch or illigal cast 77:12")]
        [TestCase("There is a type mismatch or illigal cast 87:28")]
        [TestCase("There is a type mismatch or illigal cast 88:13")]
		public void TypeCastError(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("Use of unassigned variable 166:20")]
		public void UnassignedVariable(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        [TestCase("Variable e and g3.Vertices are missmatch of types. Line number 168:4")]
        [TestCase("Variable System.Collections.Generic.List`1[Compiler.AST.Nodes.AbstractNode] and g1 are missmatch of types. Line number 84:8")]
        [TestCase("Variable v and nyhej are missmatch of types. Line number 131:4")]
		public void VariableErrors(string errorMessage)
        {
            Assert.IsTrue(errorlist.Contains(errorMessage));
        }
        

	}
}