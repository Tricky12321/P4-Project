using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class PredicateNode : AbstractNode
    {
        public string VariableName;
        public List<AbstractNode> Parameters = new List<AbstractNode>();

        public PredicateNode(int LineNumber) : base(LineNumber)
        {
            
        }

        public void AddParameter(string Type, string Name, int LineNumber) {
            PredicateParameterNode PParaNode = new PredicateParameterNode(LineNumber);
            PParaNode.Type = Type;
            PParaNode.Name = Name;
            Parameters.Add(PParaNode);

        }
    }
}
