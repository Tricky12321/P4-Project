using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace Compiler.CodeGeneration.GenerationCode
{
    public class CodeWriter
    {
        public StringBuilder Functions = new StringBuilder();
        public StringBuilder MainBody = new StringBuilder();
        public StringBuilder GraphExtends = new StringBuilder();
        public StringBuilder EdgeExtends = new StringBuilder();
        public StringBuilder VertexExtends = new StringBuilder();


        private string _programFile = "CodeGeneration/Program.cs";
        private string _edgeClassFile = "CodeGeneration/Classes/Edge.cs";
        private string _graphClassFile = "CodeGeneration/Classes/Graph.cs";
        private string _vertexClassFile = "CodeGeneration/Classes/Vertex.cs";
        private string _mainBody = "*****MAINBODY*****";
        private string _functions = "*****FUNCTIONS*****";
        private string _global = "*****GLOBAL*****";
        private string _extend = "//*****EXTEND*****";

        public CodeWriter() { }

        public void FillMainBody()
        {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_mainBody, MainBody.ToString());
            File.WriteAllText(_programFile, text);
        }

        public void FillFunctions()
        {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_functions, Functions.ToString());
            File.WriteAllText(_programFile, text);
        }

        public void FillGlobals()
        {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_global, "");
            File.WriteAllText(_programFile, text);
        }

        public void FillExtends()
        {
            // Graph Extensions
            string text = File.ReadAllText(_graphClassFile);
            text = text.Replace(_extend, GraphExtends.ToString());
            File.WriteAllText(_graphClassFile, text);
            // Edge Extensions
            text = File.ReadAllText(_edgeClassFile);
            text = text.Replace(_extend, EdgeExtends.ToString());
            File.WriteAllText(_edgeClassFile, text);
            // Veretx Extensions
            text = File.ReadAllText(_vertexClassFile);
            text = text.Replace(_extend, VertexExtends.ToString());
            File.WriteAllText(_vertexClassFile, text);
        }

        public void FillAll()
        {
            FillMainBody();
            FillFunctions();
            FillGlobals();
            FillExtends();
        }
    }
}
