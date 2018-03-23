using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class FunctionNode : AbstractNode
    {
        public string FunctionName;
        public string ReturnType;
        public List<FunctionParameterNode> Parameters = new List<FunctionParameterNode>();
        public FunctionNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public void AddParameter(string ParameterType, string ParameterName, int LineNumber) {
            FunctionParameterNode NewParameter = new FunctionParameterNode(LineNumber);
            NewParameter.ParameterName = ParameterName;
            NewParameter.ParameterType = ParameterType;
            Parameters.Add(NewParameter);
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
