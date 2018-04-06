using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    public class PredicateNode : AbstractNode
    {
        public List<AbstractNode> Parameters = new List<AbstractNode>();

        public PredicateNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }

        public void AddParameter(string Type, string Name, int LineNumber, int CharIndex) {
            ParameterNode PParaNode = new ParameterNode(LineNumber, CharIndex);
            PParaNode.Type = Type;
            PParaNode.Name = Name;
            Parameters.Add(PParaNode);
        }
    }
}
