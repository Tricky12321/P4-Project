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
        private Dictionary<string, SymbolTableEntry> _symTable = new Dictionary<string, SymbolTableEntry>();
        private List<Extension> _extensionTable = new List<Extension>();
        private Dictionary<AllType, Dictionary<string, ClassEntry>> _classesTable = new Dictionary<AllType, Dictionary<string, ClassEntry>>();
        private uint _globalDepth;
        private AbstractNode _currentNode;
        public bool errorOccured = false;
        private Scope _currentScope = new Scope(null, 0, new List<string>());

        public void SetCurrentNode(AbstractNode node)
        {
            _currentNode = node;
        }

        public string Prefix => _currentScope.Prefix;

        public string GetName(string Name)
        {
            if (Prefix != "") {
                return Prefix +"."+ Name;
            } else {
                return Name;
            }
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


        public bool CheckIfDefined(string name)
        {
            // Store the prefix of the current scope
            string prefix = _currentScope.Prefix;
            // Determine what to check for
            string toCheckFor;
            if (prefix != "")
            {
                toCheckFor = prefix + "." + name;
            }
            else
            {
                toCheckFor = name;
            }
            // Loop, until there is only the name left to check, this is because we check all scopes above this, to ensure a variable isnt declared
            while (toCheckFor != name)
            {
                
                if (_symTable.ContainsKey(toCheckFor))
                {
                    return true;
                }

                int DotIndex = prefix.LastIndexOf('.');
                if (prefix.Contains("."))
                {
                    prefix = prefix.Substring(0, DotIndex);
                    toCheckFor = prefix + "." + name;
                }
                else
                {
                    toCheckFor = name;
                }
            }
            // Now that there is only the name left to check, check that too
            if (_symTable.ContainsKey(toCheckFor))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EnterSymbol(string name, AllType type)
        {
            if (!_symTable.ContainsKey(GetName(name)))
            {
                _symTable.Add(GetName(name), new SymbolTableEntry(type, _globalDepth));
            }
            else
            {
                AlreadyDeclaredError(name);
            }
        }


        public bool AttributeDefined(string name, AllType type) {
            bool IsCollection;
            List<string> names = new List<string>();
            names.Add(name);
            bool output = RetrieveTypeFromClasses(names, type, out IsCollection, false) != null;
            if (!output) {
                AttributeUndeclared(name, type);
            }
            return output;
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

        public AllType? RetrieveSymbol(string Name, bool ShowErrors = true) {
            bool IsCollection;
            return RetrieveSymbol(Name, out IsCollection, ShowErrors);
        }

        public AllType? RetrieveSymbol(string Name, out bool IsCollection, bool ShowErrors = true)
        {
            // Check if its a dot function
            if (Name.Contains('.'))
            {
                // Split the string into the different subnames
                List<string> names = Name.Split('.').ToList();
                // Check if the symbol table contains the first name given, and that it is reachable
                if (_symTable.ContainsKey(names[0]))
                {
                    if (ShowErrors)
                    {
                        UndeclaredError(names[0]);
                        Console.WriteLine(names[0] + " is undeclared in this scope! On line:" + GetLineNumber());
                    }
                    IsCollection = false;
                    return null;
                }
                // Check if there is any results to get
                else
                {
                    var SymbolTableIEnum = _symTable[names[0]];
                    // Check how many names are left to handle
                    if (names.Count > 1)
                    {
                        // Remove the first element from the list, since it is now handled 
                        names.RemoveAt(0);
                        return RetrieveTypeFromClasses(names, SymbolTableIEnum.Type, out IsCollection, ShowErrors);
                    }
                    else
                    {
                        // There was no more names to handle, so the type is returned
                        IsCollection = SymbolTableIEnum.IsCollection;
                        return SymbolTableIEnum.Type;
                    }
                }
            }
            else
            {
                // Return the type of the variable
                if (_symTable.ContainsKey(Name) && _symTable[Name].Reachable)
                {
                    var SymTab = _symTable[Name];
                    IsCollection = SymTab.IsCollection;
                    return SymTab.Type;
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
            return CheckIfDefined(name);
        }

        public void OpenScope(BlockType type)
        {
            Scope NewScope = new Scope(_currentScope, _globalDepth, _currentScope.GetPrefixes());
            _currentScope = NewScope;
            _currentScope.AddPrefix(type, NewScope);
            ++_globalDepth;
        }

        public void OpenScope(string value)
        {
            Scope NewScope = new Scope(_currentScope, _globalDepth, _currentScope.GetPrefixes());
            _currentScope = NewScope;
            _currentScope.AddPrefix(value);
            ++_globalDepth;
        }

        public void CloseScope()
        {
            if (_currentScope.ParentScope == null)
            {
                Console.WriteLine("Cannot close scope, since its the last scope!");
            }
            else
            {
                _currentScope = _currentScope.CloseScope();
                _symTable.Values.Where(y => y.Depth == _globalDepth && y.Reachable == true).ToList().ForEach(y => y.Reachable = false);
                --_globalDepth;
            }
        }

        public void ExtendClass(AllType Type, string longAttribute, string shortAttribute, AllType ClassType)
        {
            ClassEntry Short = new ClassEntry(shortAttribute, Type);
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            _classesTable[ClassType].Add(shortAttribute, Short);
            _classesTable[ClassType].Add(longAttribute, Long);
        }

        public void ExtendClass(AllType Type, string longAttribute, AllType ClassType)
        {
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            _classesTable[ClassType].Add(longAttribute, Long);
        }

        private string GetLineNumber()
        {
            if (_currentNode != null)
            {
                return _currentNode.LineNumber + ":" + _currentNode.CharIndex;
            }
            else
            {
                return "";
            }
        }



        // ERRORS:

        private void Error()
        {
            errorOccured = true;
        }

        public void NotImplementedError(AbstractNode node)
        {
            Console.WriteLine("This node is visited, but its not implemented! - " + node.ToString());
            Error();
        }

        public void AlreadyDeclaredError(string name)
        {
            Console.WriteLine(name + " is already declared! - " + GetLineNumber());
            Error();
        }

        public void UndeclaredError(string name)
        {
            Console.WriteLine(name + " is defined in this scope! - " + GetLineNumber());
            Error();
        }

        public void NotDeclaredError()
        {
            Console.WriteLine($"Variable or collection not declared at line number {GetLineNumber()}");
            Error();
        }

        public void AttributeUndeclared(string name, AllType type)
        {
            Console.WriteLine($"Attribute {name} does not exist in class {type} {GetLineNumber()}");
            Error();
        }

        public void WrongTypeError(AllType? variable1, AllType? variable2)
        {
            Console.WriteLine($"Variable {variable1} and collection {variable2} are missmatch of types. Line number {GetLineNumber()}");
            Error();
        }
    }
}
