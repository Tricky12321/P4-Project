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
			Program.TestMode = true;
			CST = Program.BuildCST("kode_TypeChecker.giraph");
			AST = Program.BuildAST(CST);
			SymbolTable = Program.BuildSymbolTable(AST as StartNode);
 			Program.TypeCheck(SymbolTable, AST as StartNode);
			errorlist = SymbolTable.getTypeCheckErrorList();
		}

		[TestCase("The parameter: parameter cannot be of type void 115:0")]
		[TestCase("The parameter: anothervoidparameter cannot be of type void 154:0")]
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
		[TestCase("The variable retrieved from: hej2 is not of type collection 156:24")]
		public void selectallErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("The variable retrieved from: timint is not of type collection 72:18")]
		[TestCase("The variable retrieved from: hej2 is not of type collection 156:24")]
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
		[TestCase("The variable retrieved from: hej2 is not of type collection 158:1")]
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
		[TestCase("The variable retrieved from: hej2 is not of type collection 159:1")]
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
		[TestCase("There is a type mismatch in the expression on 167:27")]
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

		[TestCase("Target variable: fratim is not of type collection 71:25")]
		[TestCase("It is not possible to declare a variable with the same variable. Duplicates used: fratim2 100:26")]
		[TestCase("There is a type mismatch in the expression on 102:26")]
		[TestCase("There is a type mismatch in the expression on 103:26")]
		[TestCase("There is a type mismatch in the expression on 104:16")]
		[TestCase("Declaration cant be of type void! 106:1")]
		public void declarationsErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("There is a type mismatch in the expression on 109:25")]
		[TestCase("There is a type mismatch in the expression on 110:25")]
		[TestCase("There is a type mismatch in the expression on 139:12")]
		public void ExpressionErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("Trying to return to void function: voidFunc, at 116:1")]
		[TestCase("Variable wrongreturnFunc and  are missmatch of types. Line number 119:1")]
		[TestCase("Variable collreturnfunc and  are missmatch of collection. Line number 123:1")]
		public void ReturnErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("There is a type mismatch in the condition on Line number 126:1")]
		[TestCase("There is a type mismatch in the condition on Line number 128:10")]
		public void forLoopErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("Variable v and nyhej are missmatch of types. Line number 133:1")]
		[TestCase("Variable e and g3.Vertices are missmatch of types. Line number 171:1")]
		public void foreachErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("Variable  and endnuentim are missmatch of types. Line number 72:18")]
		[TestCase("Variable  and expint are missmatch of types. Line number 109:14")]
		[TestCase("Declaration cant be of type void! 135:1")]
		[TestCase("Variable  and decitest are missmatch of types. Line number 136:18")]
		[TestCase("It is not possible to declare a variable with the same variable. Duplicates used: k 137:9")]
		[TestCase("It is not possible to declare a variable with the same variable. Duplicates used: i 138:11")]
		public void variableDclErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("one or more provided variables or constants is not legal to print. 139:12")]
		[TestCase("one or more provided variables or constants is not legal to print. 173:7")]
		public void printErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("Actual parameter: k and formal parameter: parameter are a type missmatch 141:1")]
		[TestCase("Actual parameter:  and formal parameter: parameter are a type missmatch 144:1")]
		[TestCase("Trying to call a function: voidFunc, without actual parameters145:1")]
		[TestCase("Too many parameters declared in function call 142:1")]
		public void runErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("Actual parameter:  did not match the type of the formal parameter! 148:8")]
		[TestCase("Actual parameter:  did not match the type of the formal parameter! 175:17")]
		public void predicateCallErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("i is not a collection, and therefore remove is not able to be used 150:1")]
		[TestCase("hej2 is not a collection, and therefore remove is not able to be used 177:1")]
		public void removeErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}

		[TestCase("i is not a collection, and therefore remove is not able to be used 151:1")]
		[TestCase("hej2 is not a collection, and therefore remove is not able to be used 178:1")]
		public void removeAllErrors(string errorMessage)
		{
			Assert.IsTrue(errorlist.Contains(errorMessage));
		}






	}
}