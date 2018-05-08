using NUnit.Framework;
using System;
using Compiler.AST;
using Compiler.CodeGeneration;
using Compiler.AST.Nodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;
using Compiler;
namespace Unittests
{
    [TestFixture()]
    public class UtilitiesTest
    {
        [TestCase("int", ExpectedResult = AllType.INT)]
        [TestCase("bool", ExpectedResult = AllType.BOOL)]
        [TestCase("graph", ExpectedResult = AllType.GRAPH)]
        [TestCase("vertex", ExpectedResult = AllType.VERTEX)]
        [TestCase("edge", ExpectedResult = AllType.EDGE)]
        [TestCase("decimal", ExpectedResult = AllType.DECIMAL)]
        [TestCase("asdf", ExpectedResult = AllType.UNKNOWNTYPE)]
        [TestCase("ints", ExpectedResult = AllType.UNKNOWNTYPE)]
        [TestCase("dicimals", ExpectedResult = AllType.UNKNOWNTYPE)]
        [TestCase("grande int", ExpectedResult = AllType.UNKNOWNTYPE)]
        [TestCase("haha", ExpectedResult = AllType.UNKNOWNTYPE)]
        public AllType GetTypeFromStringTest(string Value)
        {
            return Utilities.FindTypeFromString(Value);
        }

        [Test]
        public void TestOS()
        {
            if (Utilities.IsLinux || Utilities.IsMacOS || Utilities.IsWindows)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
