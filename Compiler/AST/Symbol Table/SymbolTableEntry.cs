﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;

namespace Compiler.AST.SymbolTable
{
    class SymbolTableEntry
    {
        public AllType Type;
        public uint Depth;
        public bool Reachable = true;
        public bool IsCollection = false;
		public bool IsAssigned = false;
        public SymbolTableEntry(AllType type, uint depth)
        {
            Type = type;
            Depth = depth;
        }

        public SymbolTableEntry(AllType type, bool IsCollection, uint depth) : this(type, depth)
        {
            this.IsCollection = IsCollection;
        }

		public override string ToString()
		{
            string collection = IsCollection ? "COLLECTION" : "";
            string colName;
            colName = collection;
            return colName+" "+Type.ToString()+"|Depth:"+Depth+"|Reach:"+Reachable.ToString();
		}
	}
}
