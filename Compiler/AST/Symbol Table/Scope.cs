using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Compiler.AST.SymbolTable
{
    public class Scope
    {
        public Scope ParentScope;
        private List<string> _prefixes = new List<string>();
        private uint _forLoopCounter = 0;
        private uint _foreachLoopCounter = 0;
        private uint _whileLoopCounter = 0;
        private LoopType _loopType;
        public uint depth;
        public string Prefix { get {
                StringBuilder strBuild = new StringBuilder();
                foreach (var prefix in _prefixes)
                {
                    strBuild.Append(prefix + ".");
                }
                return strBuild.ToString();
            }
        }

        public Scope(Scope Parent, uint depth, List<string> prefixes)
        {
            // Copy the input list, so all references are dropped
            _prefixes = prefixes.ToArray().ToList();
            ParentScope = Parent;
            this.depth = depth;
            this.depth++;
        }

        public Scope CloseScope() {
            return ParentScope;
        }

        public void AddPrefix(LoopType type)
        {
            string prefix = "";
            switch (type)
            {
                case LoopType.ForEachLoop:
                    _foreachLoopCounter++;
                    prefix = "for"+_foreachLoopCounter;
                    break;
                case LoopType.ForLoop:
                    _forLoopCounter++;
                    prefix = "foreach" + _foreachLoopCounter;

                    break;
                case LoopType.WhileLoop:
                    _whileLoopCounter++;
                    prefix = "while" + _foreachLoopCounter;
                    break;
                default:
                    break;
            }
                _prefixes.Add(prefix);
        }
        public void AddPrefix(string prefix) {
            _prefixes.Add(prefix);

        }

        public List<string> GetPrefixes() {
            return _prefixes;
        }
    }
}
