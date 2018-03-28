using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;

namespace Compiler.AST.SymbolTable
{
    class SymbolTableEntry
    {
        public AllType Type;
        public AllType CollectionType; /* Only used for COLLECTION to indicate what type of COLLECTION */
        public uint Depth;
        public bool Reachable = true;

        public SymbolTableEntry(AllType type, uint depth)
        {
            Type = type;
            Depth = depth;
        }

        public SymbolTableEntry(AllType type, AllType collectionType, uint depth) : this(type, depth)
        {
            CollectionType = collectionType;
        }
    }
}
