using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class ExtendNode : AbstractNode
    {
        //EXTEND EDGE INT 'weight':'w' = 0;
        public string ClassToExtend;
        public string ExtendWithType;
        public string ExtensionName;
        public string ExtensionShortName;
        public string ExtensionDefaultValue;
        public ExtendNode(int LineNumber) : base(LineNumber)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
