using System;
using System.Linq;
using System.Collections.Generic;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;
namespace Compiler.AST.SymbolTable
{
    public class SymTable
    {
        private Dictionary<string, List<SymbolTableEntry>> _symTable = new Dictionary<string, List<SymbolTableEntry>>();
        private List<Extension> _extensionTable = new List<Extension>();
        private Dictionary<AllType, Dictionary<string, ClassEntry>> _classesTable = new Dictionary<AllType, Dictionary<string, ClassEntry>>();
        private uint _globalDepth;
        private AbstractNode _currentNode;

        public void SetCurrentNode( AbstractNode node) {
            _currentNode = node;
        }

        public SymTable()
        {
            _classesTable.Add(AllType.GRAPH, new Dictionary<string, ClassEntry>());
            _classesTable.Add(AllType.VERTEX, new Dictionary<string, ClassEntry>());
            _classesTable.Add(AllType.EDGE, new Dictionary<string, ClassEntry>());
            ClassEntry VertexFrom = new ClassEntry("VertexFrom", AllType.VERTEX);
            ClassEntry VertexTo = new ClassEntry("VertexTo", AllType.VERTEX);
            _classesTable[AllType.EDGE].Add(VertexFrom.Name, VertexFrom);
            _classesTable[AllType.EDGE].Add(VertexTo.Name, VertexTo); 
        }

        public void EnterSymbol(string name, AllType type)
        {
            if (DeclaredLocally(name))
            {
                Console.WriteLine($"Duplicate definition of {name} at line number {GetLineNumber()}");
            }
            else if (!_symTable.ContainsKey(name))
            {
                _symTable.Add(name, new List<SymbolTableEntry>());
            }
            _symTable[name].Add(new SymbolTableEntry(type, _globalDepth));
        }

        private AllType? RetrieveTypeFromClasses(List<string> names, AllType type, out bool IsCollection, bool ShowErrors = true)
        {
            // [GRAPH/VERTEX/EDGE]-><KEYS>->ClassEntry
            string name = names[0];
            // Check if the class contains a variable with the name given
            if (_classesTable[type].ContainsKey(name))
            {
                var entry = _classesTable[type][name];
                // Check if the type on the names place, is a class type
                if (IsClass(entry.Type))
                {
                    // Check how many names are left to handle, its its more then 1, continue
                    if (names.Count > 1)
                    {
                        // Remove the first name, since its no longer needed
                        names.RemoveAt(0);
                        // Call RetriveTypeFromClasses recursively until the final type is determined
                        return RetrieveTypeFromClasses(names, entry.Type, out IsCollection, ShowErrors);
                    }
                    else
                    {
                        // If this is the last name, return the type of it.
                        IsCollection = entry.Collection;
                        return entry.Type;
                    }
                }
                // If its not a class, just return the type
                else
                {
                    IsCollection = entry.Collection;
                    return _classesTable[type][name].Type;
                }
            }
            else
            {
                if (ShowErrors)
                {
                    Console.WriteLine(name + " is undeclared in the class " + type.ToString());
                }
                IsCollection = false;
                return null;
            }
        }

        public AllType? RetrieveSymbol(string Name, out bool IsCollection, bool ShowErrors = true)
        {
            // Check if its a dot function
            if (Name.Contains('.'))
            {
                // Split the string into the different subnames
                List<string> names = Name.Split('.').ToList();
                // Check if the symbol table contains the first name given, and that it is reachable
                var SymbolTableIEnum = _symTable[names[0]].Where(x => x.Reachable && x.Depth <= _globalDepth);
                // Check if there is any results to get
                if (SymbolTableIEnum.Count() == 0)
                {
                    if (ShowErrors)
                    {
                        Console.WriteLine(names[0] + " is undeclared in this scope! On line:" + GetLineNumber());
                    }
                    IsCollection = false;
                    return null;
                }
                else
                {
                    // Check how many names are left to handle
                    if (names.Count > 1)
                    {
                        // Remove the first element from the list, since it is now handled 
                        names.RemoveAt(0);
                        return RetrieveTypeFromClasses(names, SymbolTableIEnum.First().Type, out IsCollection, ShowErrors);
                    }
                    else
                    {
                        // There was no more names to handle, so the type is returned
                        IsCollection = SymbolTableIEnum.First().IsCollection;
                        return SymbolTableIEnum.First().Type;
                    }
                }
            }
            else
            {
                // Return the type of the variable
                if (_symTable.ContainsKey(Name))
                {
                    var SymTab = _symTable[Name].Where(x => x.Reachable && x.Depth <= _globalDepth);
                    IsCollection = SymTab.First().IsCollection;
                    return SymTab.First().Type;
                }
                else
                {
                    // Write that there was an error, 
                    if (ShowErrors)
                    {
                        Console.WriteLine(Name + " is undeclared in this scope! On line:" + GetLineNumber());
                    }
                    IsCollection = false;
                    return null;
                }
            }
        }

        public bool IsClass(AllType Type)
        {
            switch (Type)
            {
                case AllType.EDGE:
                case AllType.GRAPH:
                case AllType.VERTEX:
                    return true;
                default:
                    return false; ;
            }
        }

        public bool DeclaredLocally(string name)
        {
            bool IsCollection;
            return RetrieveSymbol(name, out IsCollection, false) != null;
        }

        public void OpenScope()
        {
            ++_globalDepth;
        }

        public void CloseScope()
        {
            //Makes variables unreachable, when their scope is exited
            _symTable.Values.Where(x =>
                                      x.Where(
                                          y => y.Depth == _globalDepth).Any()).ToList().ForEach(
                                              w => w.ForEach(
                                                  x => x.Reachable = false));
            --_globalDepth;
        }

        public void ExtendClass(AllType Type, string longAttribute, string shortAttribute)
        {
            ClassEntry Short = new ClassEntry(shortAttribute, Type);
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            _classesTable[Type].Add(shortAttribute, Short);
            _classesTable[Type].Add(longAttribute, Long);
        }

        public void ExtendClass(AllType Type, string longAttribute)
        {
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            _classesTable[Type].Add(longAttribute, Long);
        }

        private int GetLineNumber()
        {
            if (_currentNode != null)
            {
                return _currentNode.LineNumber;
            }
            else
            {
                return -1;
            }
        }
    }
}
