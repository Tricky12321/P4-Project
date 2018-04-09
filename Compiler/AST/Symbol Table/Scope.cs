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
        private uint _predicateCounter = 0;
        private BlockType _loopType;
        public uint depth;
        public string Prefix
        {
            get
            {
                StringBuilder strBuild = new StringBuilder();
                foreach (var prefix in _prefixes)
                {
                    strBuild.Append(prefix + ".");
                }
                int length = strBuild.Length;
                return strBuild.ToString().Substring(0,length > 0 ? length-1 : 0);
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

        public Scope CloseScope()
        {
            return ParentScope;
        }

        public void AddPrefix(BlockType type, Scope Parent)
        {
            string prefix = "";
            switch (type)
            {
                case BlockType.ForLoop:
                    prefix = "for";
                    break;
                case BlockType.ForEachLoop:
                    prefix = "foreach";
                    break;
                case BlockType.WhileLoop:
                    prefix = "while";
                    break;
                case BlockType.Predicate:
                    prefix = "predicate";
                    _predicateCounter++;
                    break;
            }
            _prefixes.Add(prefix+GetCounter(type));
        }
        public void AddPrefix(string prefix)
        {
            _prefixes.Add(prefix);

        }

        public List<string> GetPrefixes()
        {
            return _prefixes;
        }

        public uint GetCounter(BlockType type, bool OneStep = false)
        {
            if (ParentScope != null && !OneStep)
            {
                return ParentScope.GetCounter(type, true);
            }
            switch (type)
            {
                case BlockType.ForLoop:
                    return _forLoopCounter++;
                case BlockType.ForEachLoop:
                    return _foreachLoopCounter++;
                case BlockType.WhileLoop:
                    return _whileLoopCounter++;
                case BlockType.Predicate:
                    return _predicateCounter++;
            }
            return 0;
        }
    }
}
