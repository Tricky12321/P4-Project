﻿    using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class AddQueryNode : AbstractNode
    {
        
        public bool IsGraph = false;
        public bool IsColl = false;

        // Graph
        public List<AbstractNode> Dcls = new List<AbstractNode>();

        //Coll
        public bool IsVariable = false;
        public bool IsType = false;
        public bool IsQuery = false;

        public AddQueryNode(int LineNumber) : base (LineNumber)
        {
            
        }
    }
}
