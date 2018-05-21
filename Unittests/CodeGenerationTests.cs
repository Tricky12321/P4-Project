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
			Program.TestMode = true;
            if (Utilities.IsWindows) {
                CodeGeneratorOutputPath = "C:\\Users\\Ezzi\\Source\\Repos\\P4-Project\\Unittests\\CodeGeneration";
            }
            CST = Program.BuildCST("kode_generator.giraph");
            AST = Program.BuildAST(CST);
            Program.WriteCodeToFiles(AST as StartNode);
        }

        public CodeGenerationTest()
        {

        }

        [TestCase("public static void Main", ExpectedResult = true)]
		[TestCase("public static void _nameTest", ExpectedResult = true)]
		[TestCase("public static Graph _nameTest2", ExpectedResult = true)]
		[TestCase("public static int _nameTest3", ExpectedResult = true)]
		[TestCase("public static Vertex _nameTest4", ExpectedResult = true)]
		[TestCase("public static Edge _nameTest5", ExpectedResult = true)]
		[TestCase("public static decimal _nameTest6", ExpectedResult = true)]
		[TestCase("public static bool _nameTest7", ExpectedResult = true)]


		[TestCase("public static _nameasdf void", ExpectedResult = false)]
        [TestCase("grande", ExpectedResult = false)]
        [TestCase("test function som ikke findes", ExpectedResult = false)]
        [TestCase("something that isnt there", ExpectedResult = false)]
        [TestCase("shouldnt be there", ExpectedResult = false)]
        public bool CheckFunction(string FunctionText)
        {
            string ProgramFileContent = File.ReadAllText(ProgramCSFile);
            return ProgramFileContent.Contains(FunctionText);
        }

		[TestCase("bool _namei = false;", ExpectedResult = true)]
		[TestCase("decimal _namei = 0;", ExpectedResult = true)]
		[TestCase("Graph _nameg", ExpectedResult = true)]
		[TestCase("int _namei = 2;", ExpectedResult = true)]
		[TestCase("Vertex _namev;", ExpectedResult = true)]
		[TestCase("Edge _namee;", ExpectedResult = true)]
		[TestCase("decimal _namede;", ExpectedResult = true)]
		[TestCase("bool _nametrueFalseThing;", ExpectedResult = true)]


		[TestCase("asdf _namei = 0;", ExpectedResult = false)]
		[TestCase("skrald _namei = 0;", ExpectedResult = false)]
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
		[TestCase("public int _nameintSomething;", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public decimal _namedecimalSomething;", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Edge _nameedgeSomething", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Vertex _namevertexSomething", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Graph _namegraphSomething", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public bool _nameboolSomething;", AllType.EDGE, ExpectedResult = true)]

		[TestCase("public int _nameintSomething;", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public decimal _namedecimalSomething;", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Edge _nameedgeSomething", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Vertex _namevertexSomething", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Graph _namegraphSomething", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public bool _nameboolSomething;", AllType.GRAPH, ExpectedResult = true)]

		[TestCase("public int _nameintSomething;", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public decimal _namedecimalSomething;", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Edge _nameedgeSomething", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Vertex _namevertexSomething", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Graph _namegraphSomething", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public bool _nameboolSomething;", AllType.VERTEX, ExpectedResult = true)]

		[TestCase("public int _namedoesnotexist;", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public decimal _namenothere;", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public Edge _nameblaah", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Vertex _namekomigenmednogetdererher", AllType.EDGE, ExpectedResult = false)]
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

		[TestCase("public int", "_nameintSomethingLong", "_nameintSom", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public decimal", "_namedecimalSomethingLong", "_namedecSom", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Edge", "_nameedgeSomethingLong", "_nameedgSom", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Vertex", "_namevertexSomethingLong", "_nameverSom", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Graph", "_namegraphSomethingLong", "_namegraSom", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public bool", "_nameboolSomethingLong", "_namebooSom", AllType.GRAPH, ExpectedResult = true)]

		[TestCase("public int", "_nameintSomethingLong", "_nameintSom", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public decimal", "_namedecimalSomethingLong", "_namedecSom", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Edge", "_nameedgeSomethingLong", "_nameedgSom", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Vertex", "_namevertexSomethingLong", "_nameverSom", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Graph", "_namegraphSomethingLong", "_namegraSom", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public bool", "_nameboolSomethingLong", "_namebooSom", AllType.EDGE, ExpectedResult = true)]

		[TestCase("public int", "_nameintSomethingLong", "_nameintSom", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public decimal", "_namedecimalSomethingLong", "_namedecSom", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Edge", "_nameedgeSomethingLong", "_nameedgSom", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Vertex", "_namevertexSomethingLong", "_nameverSom", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Graph", "_namegraphSomethingLong", "_namegraSom", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public bool", "_nameboolSomethingLong", "_namebooSom", AllType.VERTEX, ExpectedResult = true)]
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

		[TestCase("Graph _nameg1 = new Graph();", ExpectedResult = true)]
        [TestCase("Vertex _newVertex_nameg1;", ExpectedResult = true)]
		[TestCase("Vertex _nameva = new Vertex();", ExpectedResult = true)]
		[TestCase("_nameg1._nameVertices.Add(_nameva);", ExpectedResult = true)]
		[TestCase("Vertex _namevb = new Vertex();", ExpectedResult = true)]
		[TestCase("_nameg1._nameEdges.Add(_namex);", ExpectedResult = true)]
		[TestCase("_nameg1._nameEdges.Add(_namez);", ExpectedResult = true)]
        [TestCase("_nameg1._nameVertices.Add(_namevb);", ExpectedResult = true)]
        [TestCase("_nameg1._nameVertices.Add(_namevc);", ExpectedResult = true)]
        [TestCase("_nameg1._nameVertices.Add(_namevd);", ExpectedResult = true)]
        [TestCase("_nameg1._nameVertices.Add(_nameve);", ExpectedResult = true)]
        [TestCase("Edge _newEdge_nameg1;", ExpectedResult = true)]
        [TestCase("Vertex _namevc = new Vertex();", ExpectedResult = true)]
        [TestCase("Vertex _namevd = new Vertex();", ExpectedResult = true)]
        [TestCase("Vertex _nameve = new Vertex();", ExpectedResult = true)]
        [TestCase("Edge _namex = new Edge(_namevb,_namevd);", ExpectedResult = true)]
        [TestCase("Edge _namey = new Edge(_namevd,_nameve);", ExpectedResult = true)]
        [TestCase("Edge _namez = new Edge(_nameva,_namevc);", ExpectedResult = true)]
		[TestCase("_nameg1._nameEdges.Add(_namey);", ExpectedResult = true)]
		[TestCase("_nameg1._nameDirected = false;", ExpectedResult = true)]

		[TestCase("Graph _nameg2 = new Graph();", ExpectedResult = true)]
        [TestCase("Vertex _newVertex_nameg2;", ExpectedResult = true)]
		[TestCase("_newVertex_nameg2 = new Vertex();", ExpectedResult = true)]
		[TestCase("_nameg2._nameVertices.Add(_newVertex_nameg2);", ExpectedResult = true)]
        [TestCase("Edge _newEdge_nameg2;", ExpectedResult = true)]
		[TestCase("_nameg2._nameDirected = false;", ExpectedResult = true)]

		[TestCase("Graph _nameg3 = new Graph();", ExpectedResult = false)]
        [TestCase("Vertex _newVertex_nameg3;", ExpectedResult = false)]
		[TestCase("_nameg3._nameVertices.Add(_newVertex_nameg3);", ExpectedResult = false)]
		[TestCase("_nameg3._nameDirected = false;", ExpectedResult = false)]

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
		[TestCase("while ((_namei))", ExpectedResult = true)]
		[TestCase("while (_namei)", ExpectedResult = true)]
		[TestCase("while ((_namei||true))", ExpectedResult = true)]
		[TestCase("while ((_namei&&true))", ExpectedResult = true)]
		[TestCase("while ((_namei&&_nameasdf&&true))", ExpectedResult = false)]
		[TestCase("while ((_namei&&_nametrueasd))", ExpectedResult = false)]
        [TestCase("while ((i123&&true))", ExpectedResult = false)]
        [TestCase("while ()", ExpectedResult = false)]
        [TestCase("while \n{", ExpectedResult = false)]
        public bool WhileLoopsCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }


		[TestCase("foreach (Vertex _namev in _nameg1._nameVertices)", ExpectedResult = true)]
		[TestCase("foreach (Edge _namee in _nameg1._nameEdges)", ExpectedResult = true)]
        [TestCase("foreach ()", ExpectedResult = false)]
        public bool ForeachCodeTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }


		[TestCase("Collection<Vertex> _namevcolltest1 = new Collection<Vertex>();", ExpectedResult = true)]
		[TestCase("Collection<Edge> _nameecolltest1 = new Collection<Edge>();", ExpectedResult = true)]
		[TestCase("Collection<int> _nameicolltest1 = new Collection<int>();", ExpectedResult = true)]
		[TestCase("Collection<decimal> _namedcolltest1 = new Collection<decimal>();", ExpectedResult = true)]
		[TestCase("Collection<Graph> _namegcolltest1 = new Collection<Graph>();", ExpectedResult = true)]
		[TestCase("Collection<bool> _namebcolltest1 = new Collection<bool>();", ExpectedResult = true)]
		[TestCase("Collection<> _namebcolltest1 = new Collection<bool>();", ExpectedResult = false)]
		[TestCase("Collection<> _namebcolltest1 = new Collection<>();", ExpectedResult = false)]
		[TestCase("Collection<bool> _namebst1 = new Collection<bool>();", ExpectedResult = false)]
        [TestCase("Collection<> = new Collection<bool>();", ExpectedResult = false)]
        public bool CollectionTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

		[TestCase("public Collection<int> _nametestintCollection = new Collection<int>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<decimal>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollection = new Collection<Edge>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<bool>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollection = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = true)]

		[TestCase("public Collection<int> _nametestintCollection = new Collection<int>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<decimal>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollection = new Collection<Edge>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<bool>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollection = new Collection<Graph>();", AllType.EDGE, ExpectedResult = true)]

		[TestCase("public Collection<int> _nametestintCollection = new Collection<int>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<decimal>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollection = new Collection<Edge>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<bool>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollection = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = true)]

        // False positive tests

		[TestCase("public Collection<> _nametestintCollection = new Collection<int>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public <Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public Collection<> _nametestgraphCollection = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = false)]

		[TestCase("public Collection<> _nametestintCollection = new Collection<int>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public <Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public Collection<> _nametestgraphCollection = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = false)]

		[TestCase("public Collection<> _nametestintCollection = new Collection<int>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public Collection<decimal> _nametestdecimalCollection = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public <Vertex> _nametestvertexCollection = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public Collection<bool> _nametestboolCollection = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public Collection<> _nametestgraphCollection = new Collection<Graph>();", AllType.EDGE, ExpectedResult = false)]

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

		[TestCase("public Collection<int> _nametestintCollectionLong = new Collection<int>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionLong = new Collection<decimal>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionLong = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionLong = new Collection<Edge>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionLong = new Collection<bool>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionLong = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<int> _nametestintCollectionShort", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionShort", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionShort", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionShort", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionShort", AllType.VERTEX, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionShort", AllType.VERTEX, ExpectedResult = true)]

		[TestCase("public Collection<int> _nametestintCollectionLong = new Collection<int>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionLong = new Collection<decimal>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionLong = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionLong = new Collection<Edge>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionLong = new Collection<bool>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionLong = new Collection<Graph>();", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<int> _nametestintCollectionShort", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionShort", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionShort", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionShort", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionShort", AllType.EDGE, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionShort", AllType.EDGE, ExpectedResult = true)]

		[TestCase("public Collection<int> _nametestintCollectionLong = new Collection<int>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionLong = new Collection<decimal>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionLong = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionLong = new Collection<Edge>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionLong = new Collection<bool>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionLong = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<int> _nametestintCollectionShort", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<decimal> _nametestdecimalCollectionShort", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Vertex> _nametestvertexCollectionShort", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Edge> _nametestedgeCollectionShort", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<bool> _nametestboolCollectionShort", AllType.GRAPH, ExpectedResult = true)]
		[TestCase("public Collection<Graph> _nametestgraphCollectionShort", AllType.GRAPH, ExpectedResult = true)]

		[TestCase("public Collection<int> _nametestintCollectionLong new Collection<int>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestdecimalCollectionLong =  Collection<decimal>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public <bool> _nametestboolCollectionLong = new Collection<>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public <decimal> ", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.GRAPH, ExpectedResult = false)]
		[TestCase("public <Edge> _nametestedgeCollectionShort", AllType.GRAPH, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestboolCollectionShort", AllType.GRAPH, ExpectedResult = false)]
        [TestCase("public Collection<Graph> asdas", AllType.GRAPH, ExpectedResult = false)]

		[TestCase("public Collection<int> _nametestintCollectionLong new Collection<int>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestdecimalCollectionLong =  Collection<decimal>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public <bool> _nametestboolCollectionLong = new Collection<>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public <decimal> ", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.VERTEX, ExpectedResult = false)]
		[TestCase("public <Edge> _nametestedgeCollectionShort", AllType.VERTEX, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestboolCollectionShort", AllType.VERTEX, ExpectedResult = false)]
        [TestCase("public Collection<Graph> asdf", AllType.VERTEX, ExpectedResult = false)]

		[TestCase("public Collection<int> _nametestintCollectionLong new Collection<int>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestdecimalCollectionLong =  Collection<decimal>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <Vertex>  = new Collection<Vertex>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<>  = new <Edge>();", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public <bool> _nametestintCollectionLong = new Collection<>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase(" Collection<Graph>  = new Collection<Graph>();", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<intasdf> testintCollectionShort", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public <decimal> asdf", AllType.EDGE, ExpectedResult = false)]
        [TestCase("public Collection<Vertex> asdf", AllType.EDGE, ExpectedResult = false)]
		[TestCase("public <Edge> _nametestedgeCollectionShort", AllType.EDGE, ExpectedResult = false)]
		[TestCase(" Collection<> _nametestboolCollectionShort", AllType.EDGE, ExpectedResult = false)]
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


        [TestCase("_namei = false;", ExpectedResult = true)]
        [TestCase("_namek = 10;", ExpectedResult = true)]
        [TestCase("_namek = 12;", ExpectedResult = true)]
        [TestCase("_namek = 1;", ExpectedResult = true)]
        [TestCase("_namek = 5;", ExpectedResult = true)]
        [TestCase("_namek = 9;", ExpectedResult = true)]
        [TestCase("_namek = 4;", ExpectedResult = true)]
        [TestCase("_namek = 3;", ExpectedResult = true)]
		[TestCase("_nameasdf = 3;", ExpectedResult = false)]
		[TestCase("_nameasdjn = asd;", ExpectedResult = false)]
		[TestCase("_name010h101 = 3919u;", ExpectedResult = false)]
        public bool SetQueryTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

		[TestCase("_nameicolltest1.Push(_namek);", ExpectedResult = true)]
        [TestCase("_nameicolltest1.Push(123);", ExpectedResult = true)]
		[TestCase("_namebcolltest1.Push(_namei);", ExpectedResult = true)]
        [TestCase("_namedcolltest1.Push(10);", ExpectedResult = true)]

        [TestCase("_namedcolltest1.Push(asdf);", ExpectedResult = false)]
        [TestCase("_namedcolltest1.Push(xxxx);", ExpectedResult = false)]
        [TestCase("_namedcolltest1.Push(-1000);", ExpectedResult = false)]
        [TestCase("_namedcolltest1.Push();", ExpectedResult = false)]
        public bool AddQueryTest(string Code)
        {
            string FileContent = File.ReadAllText(ProgramCSFile);
            return FileContent.Contains(Code);
        }

    }
}
