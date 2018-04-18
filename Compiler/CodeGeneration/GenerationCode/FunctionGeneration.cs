using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace Compiler.CodeGeneration.GenerationCode
{
    public class FunctionGeneration
    {
        public StringBuilder Functions;
        public StringBuilder MainBody;
        private string _programFile = "CodeGeneration/Program.cs";
        private string _mainBody = "*****MAINBODY*****";
        private string _functions = "*****FUNCTIONS*****";
        private string _global = "*****GLOBAL*****";
        public FunctionGeneration()
        {
            
        }

        public void FillMainBody() {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_mainBody, MainBody.ToString());
            File.WriteAllText(_programFile, text);
        }

        public void FillFunctions() {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_functions, Functions.ToString());
            File.WriteAllText(_programFile, text);
        }

        public void FillGlobals() {
            string text = File.ReadAllText(_programFile);
            text = text.Replace(_global, "");
            File.WriteAllText(_programFile, text);
        }
        public void FillAll() {
            FillMainBody();
            FillFunctions();
            FillGlobals();
        }
    }
}
