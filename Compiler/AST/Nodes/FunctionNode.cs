using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class FunctionNode : AbstractNode
    {
        public string ReturnType;
        public List<ParameterNode> Parameters = new List<ParameterNode>();
        public FunctionNode(int LineNumber, int CharIndex) : base(LineNumber,CharIndex)
        {
            
        }

        public void AddParameter(string ParameterType, string ParameterName, int LineNumber, int CharIndex) {
            ParameterNode NewParameter = new ParameterNode(LineNumber, CharIndex);
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
