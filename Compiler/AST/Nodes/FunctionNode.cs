using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class FunctionNode : AbstractNode
    {
        public string ReturnType;
        public List<FunctionParameterNode> Parameters = new List<FunctionParameterNode>();
        public FunctionNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public void AddParameter(string ParameterType, string ParameterName, int LineNumber, int CharIndex) {
            FunctionParameterNode NewParameter = new FunctionParameterNode(LineNumber, CharIndex);
            NewParameter.Name = ParameterName;
            NewParameter.Type = ParameterType;
            Parameters.Add(NewParameter);
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
