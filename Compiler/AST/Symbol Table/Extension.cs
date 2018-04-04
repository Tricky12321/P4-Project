using System;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
namespace Compiler.AST.SymbolTable
{
    public class Extension
    {
        public string LongName;
        public string ShortName;
        public AllType Type;
        public Extension()
        {
            
        }
    }
}
