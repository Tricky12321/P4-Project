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
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using System.IO;
namespace Unittests
{
    [TestFixture()]
    public class CodeGenerationTest
    {
        GiraphParser.StartContext CST;
        AbstractNode AST;
        string CodeGeneratorOutputPath = "CodeGeneration";
        string CodeGeneratorClassOutputPath = "/Classes";
        string ProgramCSFile => CodeGeneratorOutputPath + "/Program.cs";
        string GraphFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Graph.cs";
        string VertexFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Vertex.cs";
        string EdgeFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Edge.cs";
        [SetUp]
        public void Init()
        {
            CST = Program.BuildCST("kode_generator.giraph");
            AST = Program.BuildAST(CST);
            Program.WriteCodeToFiles(AST as StartNode);
        }

        public CodeGenerationTest()
        {

        }

        [TestCase("public static void Main", ExpectedResult = true)]
        [TestCase("public static void Test", ExpectedResult = true)]
        [TestCase("public static Graph Test2", ExpectedResult = true)]
        [TestCase("public static int Test3", ExpectedResult = true)]
        [TestCase("public static Vertex Test4", ExpectedResult = true)]
        [TestCase("public static Edge Test5", ExpectedResult = true)]
        [TestCase("public static decimal Test6", ExpectedResult = true)]
        [TestCase("public static bool Test7", ExpectedResult = true)]


        [TestCase("public static asdf void", ExpectedResult = false)]
        [TestCase("grande", ExpectedResult = false)]
        [TestCase("test function som ikke findes", ExpectedResult = false)]
        [TestCase("something that isnt there", ExpectedResult = false)]
        [TestCase("shouldnt be there", ExpectedResult = false)]
        public bool CheckFunction(string FunctionText)
        {
            string ProgramFileContent = File.ReadAllText(ProgramCSFile);
            return ProgramFileContent.Contains(FunctionText);
        }

        [TestCase("int i = 0;", ExpectedResult = true)]
        [TestCase("decimal i = 0;", ExpectedResult = true)]
        [TestCase("Graph g", ExpectedResult = true)]
        [TestCase("int i = 2;", ExpectedResult = true)]
        [TestCase("Vertex v;", ExpectedResult = true)]
        [TestCase("Edge e;", ExpectedResult = true)]
        [TestCase("decimal de;", ExpectedResult = true)]
        [TestCase("bool trueFalseThing;", ExpectedResult = true)]


        [TestCase("asdf i = 0;", ExpectedResult = false)]
        [TestCase("skrald i = 0;", ExpectedResult = false)]
        [TestCase("nogetderikkeeksistereogerlangt g", ExpectedResult = false)]
        [TestCase("int variablemedgyldgivariabletype = 2;", ExpectedResult = false)]
        [TestCase("int variablemedgyldgivariabletype = ogmedugyldigtvariablenavnstadigmegetlangt;", ExpectedResult = false)]
        [TestCase("Vertex envertexsomsletikkeesksitere;", ExpectedResult = false)]
        [TestCase("Edge Edge;", ExpectedResult = false)]
        [TestCase("nogetsomikkeerenvariabledeclartion;", ExpectedResult = false)]
        [TestCase("nogetsomikkeeksistere i;", ExpectedResult = false)]
        public bool CheckVariableDcl(string ExpectedCode)
        {
            string ProgramFileContent = File.ReadAllText(ProgramCSFile);
            return ProgramFileContent.Contains(ExpectedCode);
        }
        [TestCase("public int intSomething;", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public decimal decimalSomething;", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Edge edgeSomething", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Vertex vertexSomething", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Graph graphSomething", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public bool boolSomething;", AllType.EDGE, ExpectedResult = true)]

        [TestCase("public int intSomething;", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public decimal decimalSomething;", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Edge edgeSomething", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Vertex vertexSomething", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Graph graphSomething", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public bool boolSomething;", AllType.GRAPH, ExpectedResult = true)]

        [TestCase("public int intSomething;", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public decimal decimalSomething;", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Edge edgeSomething", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Vertex vertexSomething", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Graph graphSomething", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public bool boolSomething;", AllType.VERTEX, ExpectedResult = true)]


        [TestCase("public int doesnotexist;", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public decimal nothere;", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Edge blaah", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Vertex komigenmednogetdererher", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public eksisteresletikke graphSomething", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public ikkeenrigtigtype boolSomething;", AllType.EDGE, ExpectedResult = false)]

        [TestCase("public int doesnotexist;", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public decimal nothere;", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Edge blaah", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Vertex komigenmednogetdererher", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public eksisteresletikke graphSomething", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public ikkeenrigtigtype boolSomething;", AllType.GRAPH, ExpectedResult = false)]

        public bool CheckExtendsOneExtension(string ExtensionName, AllType Class)
        {
            string FilePath = "";
            switch (Class)
            {
                case AllType.GRAPH:
                    FilePath = GraphFile;
                    break;
                case AllType.EDGE:
                    FilePath = EdgeFile;
                    break;
                case AllType.VERTEX:
                    FilePath = VertexFile;
                    break;
            }
            string FileContent = File.ReadAllText(FilePath);
            return FileContent.Contains(ExtensionName);
        }

        [TestCase("public int", "intSomethingLong", "intSom"        , AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom"      , AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom"  , AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom"    , AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom"      , AllType.GRAPH, ExpectedResult = true)]

        [TestCase("public int", "intSomethingLong", "intSom"        , AllType.EDGE, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom"      , AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom"  , AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom"    , AllType.EDGE, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom"      , AllType.EDGE, ExpectedResult = true)]

        [TestCase("public int", "intSomethingLong", "intSom"        , AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom"      , AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom"  , AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom"    , AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom"      , AllType.VERTEX, ExpectedResult = true)]
        public bool CheckExtensions2Names(string prefix, string LongName, string Shortname, AllType Class)
        {
            string FilePath = "";
            switch (Class)
            {
                case AllType.GRAPH:
                    FilePath = GraphFile;
                    break;
                case AllType.EDGE:
                    FilePath = EdgeFile;
                    break;
                case AllType.VERTEX:
                    FilePath = VertexFile;
                    break;
            }
            string FileContent = File.ReadAllText(FilePath);
            return FileContent.Contains(prefix + " " + LongName) && FileContent.Contains(prefix + " " + Shortname);
        }

    }
}
