using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class PredicateNode : AbstractNode
    {
        public string VariableName;
        public List<AbstractNode> Parameters = new List<AbstractNode>();

        public PredicateNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public void AddParameter(string Type, string Name, int LineNumber, int CharIndex) {
            PredicateParameterNode PParaNode = new PredicateParameterNode(LineNumber, CharIndex);
            PParaNode.Type = Type;
            PParaNode.Name = Name;
            Parameters.Add(PParaNode);

        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
