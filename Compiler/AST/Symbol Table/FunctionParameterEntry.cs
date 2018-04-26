using System;
namespace Compiler.AST.SymbolTable
{
    public class FunctionParameterEntry
    {
        public string Name;
        public AllType Type;
        public bool Collection = false;
        private static int statId = 0;
        private int _id;

        public int ID
        {
            get { return _id; }
        }

        public FunctionParameterEntry(string Name, AllType Type, bool Collection = false)
        {
            this.Name = Name;
            this.Type = Type;
            this.Collection = Collection;
            _id = statId++;
        }
    }
}
