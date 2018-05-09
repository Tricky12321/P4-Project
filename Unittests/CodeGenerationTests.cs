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
		string CodeGeneratorOutputPath = Utilities.CurrentPath+"/CodeGeneration";
        string CodeGeneratorClassOutputPath = "/Classes";
        string ProgramCSFile => CodeGeneratorOutputPath + "/Program.cs";
        string GraphFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Graph.cs";
        string VertexFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Vertex.cs";
        string EdgeFile => CodeGeneratorOutputPath + CodeGeneratorClassOutputPath + "/Edge.cs";
        [SetUp]
        public void Init()
        {
            if (Utilities.IsWindows) {
                CodeGeneratorOutputPath = "SMID JERES PATH HER";
            }
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

        [TestCase("bool i = false;", ExpectedResult = true)]
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

        [TestCase("public int", "intSomethingLong", "intSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom", AllType.GRAPH, ExpectedResult = true)]

        [TestCase("public int", "intSomethingLong", "intSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom", AllType.EDGE, ExpectedResult = true)]

        [TestCase("public int", "intSomethingLong", "intSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public decimal", "decimalSomethingLong", "decSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Edge", "edgeSomethingLong", "edgSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Vertex", "vertexSomethingLong", "verSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Graph", "graphSomethingLong", "graSom", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public bool", "boolSomethingLong", "booSom", AllType.VERTEX, ExpectedResult = true)]
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

        [TestCase("Graph g1 = new Graph();", ExpectedResult = true)]
        [TestCase("Vertex _newVertexg1;", ExpectedResult = true)]
        [TestCase("Vertex va = new Vertex();", ExpectedResult = true)]
        [TestCase("g1.Vertices.Add(va);", ExpectedResult = true)]
        [TestCase("Vertex vb = new Vertex();", ExpectedResult = true)]
        [TestCase("g1.Edges.Add(x);", ExpectedResult = true)]
        [TestCase("g1.Edges.Add(z);", ExpectedResult = true)]
        [TestCase("g1.Vertices.Add(vb);", ExpectedResult = true)]
        [TestCase("g1.Vertices.Add(vc);", ExpectedResult = true)]
        [TestCase("g1.Vertices.Add(vd);", ExpectedResult = true)]
        [TestCase("g1.Vertices.Add(ve);", ExpectedResult = true)]
        [TestCase("Edge _newEdgeg1;", ExpectedResult = true)]
        [TestCase("Vertex vc = new Vertex();", ExpectedResult = true)]
        [TestCase("Vertex vd = new Vertex();", ExpectedResult = true)]
        [TestCase("Vertex ve = new Vertex();", ExpectedResult = true)]
        [TestCase("Edge x = new Edge(vb,vd);", ExpectedResult = true)]
        [TestCase("Edge y = new Edge(vd,ve);", ExpectedResult = true)]
        [TestCase("Edge z = new Edge(va,vc);", ExpectedResult = true)]
        [TestCase("g1.Edges.Add(y);", ExpectedResult = true)]
        [TestCase("g1.Directed = false;", ExpectedResult = true)]

        [TestCase("Graph g2 = new Graph();", ExpectedResult = true)]
        [TestCase("Vertex _newVertexg2;", ExpectedResult = true)]
        [TestCase("_newVertexg2 = new Vertex();", ExpectedResult = true)]
        [TestCase("g2.Vertices.Add(_newVertexg2);", ExpectedResult = true)]
        [TestCase("Edge _newEdgeg2;", ExpectedResult = true)]
        [TestCase("g2.Directed = false;", ExpectedResult = true)]

        [TestCase("Graph g3 = new Graph();", ExpectedResult = false)]
        [TestCase("Vertex _newVertexg3;", ExpectedResult = false)]
        [TestCase("g3.Vertices.Add(_newVertexg3);", ExpectedResult = false)]
        [TestCase("g3.Directed = false;", ExpectedResult = false)]

        public bool GraphDeclarationCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

        [TestCase("for (int _i0 = 1; _i0 < 10;_i0 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i1 = 1; _i1 < 1000;_i1 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i2 = 5; _i2 < 10;_i2 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i3 = 5; _i3 < 1000;_i3 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i4 = 5; _i4 < 1000;_i4 += 10)", ExpectedResult = true)]
        [TestCase("for (int _i5 = 100; _i5 < 1234;_i5 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i6 = 9999; _i6 < 10;_i6 += 1)", ExpectedResult = true)]
        [TestCase("for (int _i7 = 9; _i7 < 1234;_i7 += 9123)", ExpectedResult = true)]
        [TestCase("for (int _i8 = 1; _i8 < asdf;_i8 += 9121231233)", ExpectedResult = false)]
        [TestCase("for (int _i9 = 2; _i9 < skrald;_i9 += 31123)", ExpectedResult = false)]
        [TestCase("for (int _i10 = 3; _i10 < asdt;_i10 += 9122123)", ExpectedResult = false)]
        [TestCase("for (int _i11 = 4; _i11 < noget;_i11 += 91123)", ExpectedResult = false)]
        [TestCase("for (int _i12 = 5; _i12 < 1234;_i12 += 2)", ExpectedResult = false)]
        public bool ForLoopsCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

        [TestCase("while ((true))", ExpectedResult = true)]
        [TestCase("while ((i))", ExpectedResult = true)]
        [TestCase("while (i)", ExpectedResult = true)]
        [TestCase("while ((i||true))", ExpectedResult = true)]
        [TestCase("while ((i&&true))", ExpectedResult = true)]
        [TestCase("while ((i&&asdf&&true))", ExpectedResult = false)]
        [TestCase("while ((i&&trueasd))", ExpectedResult = false)]
        [TestCase("while ((i123&&true))", ExpectedResult = false)]
        [TestCase("while ()", ExpectedResult = false)]
        [TestCase("while \n{", ExpectedResult = false)]
        public bool WhileLoopsCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }


        [TestCase("foreach (Vertex v in g1.Vertices)", ExpectedResult = true)]
        [TestCase("foreach (Edge e in g1.Edges)", ExpectedResult = true)]
        [TestCase("foreach ()", ExpectedResult = false)]
        public bool ForeachCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }


        [TestCase("Collection<Vertex> vcolltest1 = new Collection<Vertex>();", ExpectedResult = true)]
        [TestCase("Collection<Edge> ecolltest1 = new Collection<Edge>();", ExpectedResult = true)]
        [TestCase("Collection<int> icolltest1 = new Collection<int>();", ExpectedResult = true)]
        [TestCase("Collection<decimal> dcolltest1 = new Collection<decimal>();", ExpectedResult = true)]
        [TestCase("Collection<Graph> gcolltest1 = new Collection<Graph>();", ExpectedResult = true)]
        [TestCase("Collection<bool> bcolltest1 = new Collection<bool>();", ExpectedResult = true)]
        [TestCase("Collection<> bcolltest1 = new Collection<bool>();", ExpectedResult = false)]
        [TestCase("Collection<> bcolltest1 = new Collection<>();", ExpectedResult = false)]
        [TestCase("Collection<bool> bst1 = new Collection<bool>();", ExpectedResult = false)]
        [TestCase("Collection<> = new Collection<bool>();", ExpectedResult = false)]
        public bool CollectionTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

        [TestCase("public Collection<int> testintCollection = new Collection<int>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<decimal>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollection = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollection = new Collection<Edge>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<bool>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollection = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = true)]

        [TestCase("public Collection<int> testintCollection = new Collection<int>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<decimal>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollection = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollection = new Collection<Edge>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<bool>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollection = new Collection<Graph>();", AllType.EDGE, ExpectedResult = true)]

        [TestCase("public Collection<int> testintCollection = new Collection<int>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<decimal>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollection = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollection = new Collection<Edge>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<bool>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollection = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = true)]

        // False positive tests

        [TestCase("public Collection<> testintCollection = new Collection<int>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <Vertex> testvertexCollection = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<> testgraphCollection = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = false)]

        [TestCase("public Collection<> testintCollection = new Collection<int>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <Vertex> testvertexCollection = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<> testgraphCollection = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = false)]

        [TestCase("public Collection<> testintCollection = new Collection<int>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<decimal> testdecimalCollection = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <Vertex> testvertexCollection = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<bool> testboolCollection = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<> testgraphCollection = new Collection<Graph>();", AllType.EDGE, ExpectedResult = false)]

        public bool ExtendWithCollection(string Code, AllType Class)
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
            return FileContent.Contains(Code);
        }

        [TestCase("public Collection<int> testintCollectionLong = new Collection<int>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionLong = new Collection<decimal>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionLong = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionLong = new Collection<Edge>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionLong = new Collection<bool>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionLong = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<int> testintCollectionShort", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionShort", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionShort", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionShort", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionShort", AllType.VERTEX, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionShort", AllType.VERTEX, ExpectedResult = true)]

        [TestCase("public Collection<int> testintCollectionLong = new Collection<int>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionLong = new Collection<decimal>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionLong = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionLong = new Collection<Edge>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionLong = new Collection<bool>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionLong = new Collection<Graph>();", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<int> testintCollectionShort", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionShort", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionShort", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionShort", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionShort", AllType.EDGE, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionShort", AllType.EDGE, ExpectedResult = true)]

        [TestCase("public Collection<int> testintCollectionLong = new Collection<int>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionLong = new Collection<decimal>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionLong = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionLong = new Collection<Edge>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionLong = new Collection<bool>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionLong = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<int> testintCollectionShort", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<decimal> testdecimalCollectionShort", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Vertex> testvertexCollectionShort", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Edge> testedgeCollectionShort", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<bool> testboolCollectionShort", AllType.GRAPH, ExpectedResult = true)]
        [TestCase("public Collection<Graph> testgraphCollectionShort", AllType.GRAPH, ExpectedResult = true)]

        [TestCase("public Collection<int> testintCollectionLong new Collection<int>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase(" Collection<> testdecimalCollectionLong =  Collection<decimal>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <bool> testboolCollectionLong = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <decimal> ", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <Edge> testedgeCollectionShort", AllType.GRAPH, ExpectedResult = false)]
        [TestCase(" Collection<> testboolCollectionShort", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<Graph> asdas", AllType.GRAPH, ExpectedResult = false)]

        [TestCase("public Collection<int> testintCollectionLong new Collection<int>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase(" Collection<> testdecimalCollectionLong =  Collection<decimal>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <bool> testboolCollectionLong = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <decimal> ", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <Edge> testedgeCollectionShort", AllType.VERTEX, ExpectedResult = false)]
        [TestCase(" Collection<> testboolCollectionShort", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<Graph> asdf", AllType.VERTEX, ExpectedResult = false)]

        [TestCase("public Collection<int> testintCollectionLong new Collection<int>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase(" Collection<> testdecimalCollectionLong =  Collection<decimal>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <bool> testintCollectionLong = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <decimal> asdf", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <Edge> testedgeCollectionShort", AllType.EDGE, ExpectedResult = false)]
        [TestCase(" Collection<> testboolCollectionShort", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<Graph> asdf", AllType.EDGE, ExpectedResult = false)]
        public bool ExtendWithCollection2Names(string Code, AllType Class)
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
            return FileContent.Contains(Code);
        }


        [TestCase("i = false;", ExpectedResult = true)]
        [TestCase("k = 10;", ExpectedResult = true)]
        [TestCase("k = 12;", ExpectedResult = true)]
        [TestCase("k = 1;", ExpectedResult = true)]
        [TestCase("k = 5;", ExpectedResult = true)]
        [TestCase("k = 9;", ExpectedResult = true)]
        [TestCase("k = 4;", ExpectedResult = true)]
        [TestCase("k = 3;", ExpectedResult = true)]
        [TestCase("asdf = 3;", ExpectedResult = false)]
        [TestCase("asdjn = asd;", ExpectedResult = false)]
        [TestCase("010h101 = 3919u;", ExpectedResult = false)]
        public bool SetQueryTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

        [TestCase("icolltest1.Push(k);", ExpectedResult = true)]
        [TestCase("icolltest1.Push(123);", ExpectedResult = true)]
        [TestCase("bcolltest1.Push(i);", ExpectedResult = true)]
        [TestCase("dcolltest1.Push(10);", ExpectedResult = true)]

        [TestCase("dcolltest1.Push(asdf);", ExpectedResult = false)]
        [TestCase("dcolltest1.Push(xxxx);", ExpectedResult = false)]
        [TestCase("dcolltest1.Push(-1000);", ExpectedResult = false)]
        [TestCase("dcolltest1.Push();", ExpectedResult = false)]
        public bool AddQueryTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

    }
}
