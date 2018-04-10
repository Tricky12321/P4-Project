﻿using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class ExtendNode : AbstractNode
    {
        //EXTEND EDGE INT 'weight':'w' = 0;
        public string ClassToExtend;
        public AllType ClassToExtend_enum => Utilities.FindTypeFromString(ClassToExtend);
        public string ExtendWithType;
        public string ExtensionName;
        public string ExtensionShortName;
        public string ExtensionDefaultValue;
        public ExtendNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
