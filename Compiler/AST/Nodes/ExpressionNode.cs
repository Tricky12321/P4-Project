using System;
using System.Collections.Generic;
using Compiler;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;

namespace Compiler.AST.Nodes
{
    public class ExpressionNode : AbstractNode
    {
        //public List<KeyValuePair<ExpressionPartType, string>> ExpressionParts = new List<KeyValuePair<ExpressionPartType, string>>();
        public List<AbstractNode> ExpressionParts = new List<AbstractNode>();

        public List<AllType> ExpressionTypes = new List<AllType>();

        public ExpressionNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex) { }

        public AllType? OverAllType;

        public bool hasparentheses = false;

        public bool IsCollection;

        public string QueryName;

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

            foreach (AbstractNode part in ExpressionParts)
            {
                placeholderString += part.Name;
            }
            return placeholderString;
        }

        public void FindOverAllType() {
            foreach (var item in ExpressionTypes)
            {
                if (OverAllType == null) {
                    OverAllType = item;
                } else {
                    // There is a potential type mismatch
                    if (item != OverAllType) {
                        if (!((item == AllType.INT && OverAllType == AllType.DECIMAL) || (item == AllType.DECIMAL && OverAllType == AllType.INT))) {
                            OverAllType = AllType.UNKNOWNTYPE;
                        } else {
                            OverAllType = AllType.DECIMAL;
                        }
                    }
                }
            }
        }


    }
}
