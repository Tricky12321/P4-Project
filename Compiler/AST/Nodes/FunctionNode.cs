using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class FunctionNode : AbstractNode
    {
        public string FunctionName;
        public string ReturnType;
        public List<FunctionParameterNode> Parameters = new List<FunctionParameterNode>();
        public FunctionNode()
        {
            
        }

        public void AddParameter(string ParameterType, string ParameterName) {
            FunctionParameterNode NewParameter = new FunctionParameterNode();
            NewParameter.ParameterName = ParameterName;
            NewParameter.ParameterType = ParameterType;
            Parameters.Add(NewParameter);
        }
    }
}
