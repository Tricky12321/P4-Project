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
        // Unnamed block types
        private uint _forLoopCounter = 0;
        private uint _foreachLoopCounter = 0;
        private uint _whileLoopCounter = 0;
        private uint _ifStatementCounter = 0;
        private uint _elseIfStatementCounter = 0;
        private uint _elseStatementCounter = 0;
        private uint _whereStatementCounter = 0;

        public uint depth;

        // Since prefixes is private, we need to be able to get them, but not set them
        public List<string> GetPrefixes => _prefixes;

        /// <summary>
        ///  Is used to get the prefix list, and a string sperated by dot, which is the notation used in the symbol table
        /// </summary>
        /// <value>The prefix.</value>
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


        /// <summary>
        /// This is used when creating a new scope, this is everytime the symbol table opens a new scope
        /// Its also used to keep track of scopes and prefixes
        /// </summary>
        /// <param name="Parent">Parent scope, used when closeing a scope to return</param>
        /// <param name="depth">Depth is used to determine at what depth variables are declared, makes it easier to debug</param>
        public Scope(Scope Parent, uint depth)
        {
            // Copy the input list, so the previus prefix list is keept
            if (Parent == null) {
                _prefixes = new List<string>();
            } else {
				_prefixes = new List<string>(Parent.GetPrefixes);
            }
            ParentScope = Parent;
            this.depth = depth + 1; // Increase the depth by 1, since we opened a new scope. 
        }
        /// <summary>
        /// Closes the current scopes, and returns the parent scope
        /// </summary>
        /// <returns>The parent scope.</returns>
        public Scope CloseScope()
        {
            return ParentScope;
        }
        // Prefixes for Block to better identify them in the symbol table
        public void AddPrefix(BlockType type)
        {
            // Ensure that prefix is not null, so incase somethings fails, we dont get a null exception
            string prefix = "";
            switch (type)
            {
                // Switch for the different kind og loops, and add their unique identifier
                case BlockType.ForLoop:
                    prefix = "[FOR]";
                    break;
                case BlockType.ForEachLoop:
                    prefix = "[FOREACH]";
                    break;
                case BlockType.WhileLoop:
                    prefix = "[WHILE]";
                    break;
                case BlockType.IfStatement:
                    prefix = "[IF]";
                    break;
                case BlockType.ElseifStatement:
                    prefix = "[ELSEIF]";
                    break;
                case BlockType.ElseStatement:
                    prefix = "[ELSE]";
                    break;
                case BlockType.WhereStatement:
                    prefix = "[WHERE]";
                    break;
            }
            // Add the prefix, to the prefix list, this is to better identify scopes later, and makes it easier to debug
            _prefixes.Add(prefix+GetCounter(type));
        }
        public void AddPrefix(string prefix)
        {
            _prefixes.Add(prefix);

        }


        private uint GetCounter(BlockType type, bool OneStep = false)
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
				case BlockType.IfStatement:
					return _ifStatementCounter++;
                case BlockType.ElseifStatement:
                    return _elseIfStatementCounter++;
                case BlockType.ElseStatement:
                    return _elseStatementCounter++;
                case BlockType.WhereStatement:
                    return _whereStatementCounter++;
            }
            return 0;
        }
    }
}
