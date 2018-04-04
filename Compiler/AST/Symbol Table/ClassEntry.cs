using System;
namespace Compiler.AST.SymbolTable
{
    public class ClassEntry
    {
        public string Name;
        public AllType Type;
        public bool Collection = false;
        public ClassEntry(string Name, AllType Type, bool Collection = false)
        {
            this.Name = Name;
            this.Type = Type;
            this.Collection = Collection;
        }
    }
}
