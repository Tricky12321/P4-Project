using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
namespace Compiler.CodeGeneration.GenerationCode
{
    public class FunctionGeneration
    {
        public List<StringBuilder> Functions = new List<StringBuilder>();
        public StringBuilder MainBody;
        private string _programFile = "CodeGeneration/Program.cs";
        private string _mainBody = "*****MAINBODY*****";
        private string _functions = "*****FUNCTIONS*****";
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
            text = text.Replace(_functions, "");
            File.WriteAllText(_programFile, text);
        }
    }
}
