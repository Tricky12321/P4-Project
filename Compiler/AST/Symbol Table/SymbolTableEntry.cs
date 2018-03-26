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
        public uint Depth;
        public bool Reachable = true;

        public SymbolTableEntry(AllType type, uint depth)
        {
            Type = type;
            Depth = depth;
        }
    }
}
