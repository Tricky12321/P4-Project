using System;
using System.Collections.Generic;
using Compiler;

namespace Compiler.AST.Nodes
{
    public class ExpressionNode : AbstractNode
    {
        //public List<KeyValuePair<ExpressionPartType, string>> ExpressionParts = new List<KeyValuePair<ExpressionPartType, string>>();
        public List<Tuple<string, AbstractNode, string>> ExpressionParts = new List<Tuple<string, AbstractNode, string>>();

        public ExpressionNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex) { }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }

        public string ExpressionString()
        {
            string placeholderString = string.Empty;
            /*foreach (KeyValuePair<ExpressionPartType, string> part in ExpressionParts)
            {
                placeholderString += part.Value.ToString();
            }*/

            foreach (Tuple<string, AbstractNode, string> part in ExpressionParts)
            {
                placeholderString += part.Item2.Name.ToString();
            }
            return placeholderString;
        }

    }
}
