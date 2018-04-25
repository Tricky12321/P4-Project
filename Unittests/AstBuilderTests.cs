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

        [TestCase("v")]
        public void CheckExtendNotes(string extensionName) {
            var ExtendNodes = AST.Children.Where(x => x is ExtendNode).ToList();
            Console.WriteLine(ExtendNodes.Where(x => (x as ExtendNode).ExtensionName == extensionName).Count());
            if (ExtendNodes.Where(x => (x as ExtendNode).ExtensionName == extensionName).Count() == 1) {
                Assert.Pass();
            } else {
                Assert.Fail();
            }
        }


    }
}
