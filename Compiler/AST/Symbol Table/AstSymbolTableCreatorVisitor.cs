using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;

namespace Compiler.AST.SymbolTable
{
    internal class AstSymbolTableCreatorVisitor : AstVisitorBase
    {
        public Dictionary<string, List<SymbolTableEntry>> _symbolTable = new Dictionary<string, List<SymbolTableEntry>>();
        public List<Extension> ExtensionTable = new List<Extension>();
        public Dictionary<AllType, Dictionary<string, ClassEntry>> ClassesTable = new Dictionary<AllType, Dictionary<string, ClassEntry>>();



        private uint _globalDepth;
        private AbstractNode _currentNode;
        public AstSymbolTableCreatorVisitor()
        {
            ClassesTable.Add(AllType.GRAPH, new Dictionary<string, ClassEntry>());
            ClassesTable.Add(AllType.VERTEX, new Dictionary<string, ClassEntry>());
            ClassesTable.Add(AllType.EDGE, new Dictionary<string, ClassEntry>());
            ClassEntry VertexFrom = new ClassEntry("VertexFrom", AllType.VERTEX);
            ClassEntry VertexTo = new ClassEntry("VertexTo", AllType.VERTEX);
            ClassesTable[AllType.EDGE].Add(VertexFrom.Name, VertexFrom);
            ClassesTable[AllType.EDGE].Add(VertexTo.Name, VertexTo);
        }

        public AllType ResolveFuncType(string Type)
        {
            switch (Type)
            {
                case "VOID":
                    return AllType.VOID;
                case "STRING":
                    return AllType.STRING;
                case "BOOL":
                    return AllType.BOOL;
                case "DECIMAL":
                    return AllType.DECIMAL;
                case "INT":
                    return AllType.INT;
                case "GRAPH":
                    return AllType.GRAPH;
                case "EDGE":
                    return AllType.EDGE;
                case "VERTEX":
                    return AllType.VERTEX;
                case "COLLECTION":
                    return AllType.COLLECTION;
            }
            throw new Exception("Unknown type");
        }

        public void ExtendClass(AllType Type, string longAttribute, string shortAttribute) {
            ClassEntry Short = new ClassEntry(shortAttribute, Type);
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            ClassesTable[Type].Add(shortAttribute, Short);
            ClassesTable[Type].Add(longAttribute,Long );
        }

        public void ExtendClass(AllType Type, string longAttribute)
        {
            ClassEntry Long = new ClassEntry(longAttribute, Type);
            ClassesTable[Type].Add(longAttribute, Long);
        }

        public void BuildSymbolTable(AbstractNode root)
        {
            VisitRoot(root);
        }


        public bool IsClass(AllType Type) {
            switch (Type)
            {
                case AllType.EDGE:
                case AllType.GRAPH:
                case AllType.VERTEX:
                    return true;
                default:
                    return false;;
            }
        }

        public int GetLineNumber() {
            if (_currentNode != null) {
				return _currentNode.LineNumber;
            } else {
                return -1;
            }
        }

        private void EnterSymbol(string name, AllType type)
        {
            if (DeclaredLocally(name))
            {
                Console.WriteLine($"Duplicate definition of {name} at line number {GetLineNumber()}");
            }
            else if (!_symbolTable.ContainsKey(name)) {
                _symbolTable.Add(name, new List<SymbolTableEntry>());
            }
            _symbolTable[name].Add(new SymbolTableEntry(type, _globalDepth));
        }

        private AllType? RetriveTypeFromClasses(List<string> names, AllType type, out bool IsCollection) {
            // [GRAPH/VERTEX/EDGE]-><KEYS>->ClassEntry
            string name = names[0];
            // Check if the class contains a variable with the name given
            if (ClassesTable[type].ContainsKey(name)) {
				var entry = ClassesTable[type][name];
                // Check if the type on the names place, is a class type
                if (IsClass(entry.Type)) {
                    // Check how many names are left to handle, its its more then 1, continue
                    if (names.Count > 1) {
                        // Remove the first name, since its no longer needed
                        names.RemoveAt(0);
                        // Call RetriveTypeFromClasses recursively until the final type is determined
                        return RetriveTypeFromClasses(names, entry.Type, out IsCollection);
                    } else {
                        // If this is the last name, return the type of it.
                        IsCollection = entry.Collection;
                        return entry.Type;
                    }
                } 
                // If its not a class, just return the type
                else
                {
					IsCollection = entry.Collection;
                    return ClassesTable[type][name].Type;
                }
            } else {
                Console.WriteLine(name+" is undeclared in the class "+type.ToString());
                IsCollection = false;
                return null;
            }
        }

        public AllType? RetrieveSymbol(string Name, out bool IsCollection) {
            // G1.asdf
            // If a name contains a dot, it is a class, and needs to be handled different
            if (Name.Contains('.'))
            {
                // Split the string into the different subnames
                List<string> names = Name.Split('.').ToList();
				// Check if the symbol table contains the first name given, and that it is reachable
                var SymbolTableIEnum = _symbolTable[names[0]].Where(x => x.Reachable && x.Depth <= _globalDepth);
                // Check if there is any results to get
                if (SymbolTableIEnum.Count() == 0) {
                    Console.WriteLine(names[0]+" is undeclared in this scope! On line:"+GetLineNumber());
                    IsCollection = false;
                    return null;
                } else {
                    // Check how many names are left to handle
                    if (names.Count > 1) {
						// Remove the first element from the list, since it is now handled 
						names.RemoveAt(0);
                        return RetriveTypeFromClasses(names, SymbolTableIEnum.First().Type, out IsCollection);
                    } else {
                        // There was no more names to handle, so the type is returned
                        IsCollection = SymbolTableIEnum.First().IsCollection;
                        return SymbolTableIEnum.First().Type;
                    }
                }
            } else {
                // Return the type of the variable
                if (_symbolTable.ContainsKey(Name)) {
                    var SymbolTable = _symbolTable[Name].Where(x => x.Reachable && x.Depth <= _globalDepth);
                    IsCollection = SymbolTable.First().IsCollection;
                    return SymbolTable.First().Type;
                } else {
                    // Write that there was an error, 
                    Console.WriteLine(Name+" is undeclared in this scope! On line:"+GetLineNumber());
                    IsCollection = false;
                    return null;
                }
            }
        }

        /*
        private SymbolTableEntry RetrieveSymbol(string name)
        {
            


            if (_symbolTable.ContainsKey(name)) {
                SymbolTableEntry Entry = _symbolTable[name].Where(x => x.Reachable && x.Depth <= _globalDepth).First();
                if (IsClass(Entry.Type)) {

                    SymbolTableEntry test = ClassesTable
                } else {
                    
                }
            }

            try
            {
                List<SymbolTableEntry> entriesWithThisName = _symbolTable[name];
                SymbolTableEntry result = entriesWithThisName.Where(x => x.Reachable && x.Depth <= _globalDepth).First();
                return result;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
        */
        private bool DeclaredLocally(string name)
        {
            bool IsCollection;
            return RetrieveSymbol(name, out IsCollection) != null;
        }

        private void OpenScope()
        {
            ++_globalDepth;
        }

        private void CloseScope()
        {
            //Makes variables unreachable, when their scope is exited
            _symbolTable.Values.Where(x => 
                                      x.Where(
                                          y => y.Depth == _globalDepth).Any()).ToList().ForEach(
                                              w => w.ForEach(
                                                  x=>x.Reachable = false));
            --_globalDepth;
        }

        //All the visit stuff-----------------------------------------
        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public override void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }
        }

        public override void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public override void Visit(FunctionNode node)
        {
            _currentNode = node;
            AllType type = ResolveFuncType(node.ReturnType);
            string functionName = node.Name;

            EnterSymbol(functionName, type);
            OpenScope();
            foreach (FunctionParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            VisitChildren(node);
            CloseScope();
        }

        public override void Visit(FunctionParameterNode node)
        {
            _currentNode = node;
            AllType parameterType = ResolveFuncType(node.Type);
            EnterSymbol(node.Name, parameterType);
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ProgramNode node)
        {
            /* To be deleted */
            throw new NotImplementedException();
        }

        public override void Visit(GraphNode node)
        {
            _currentNode = node;
            string graphName = node.Name;
            EnterSymbol(graphName, AllType.GRAPH);
            VisitChildren(node);
        }

        public override void Visit(VertexNode node)
        {
            _currentNode = node;
            /* Missing the values of the vertex*/
            string vertexName = node.Name;
            EnterSymbol(vertexName, AllType.VERTEX);
        }

        public override void Visit(EdgeNode node)
        {
            _currentNode = node;
            /* Missing the values of the edge*/
            string edgeName = node.Name;
            EnterSymbol(edgeName, AllType.EDGE);
        }

        public override void Visit(SetQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(WhereNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            _currentNode = node;
            /* Goes through all declared variables of the extended type, and adds the attribute as a recognized term 
             * So if you extend vertices with an int i, and there is a vertex variable named vert,
             * it will now recognize vert.i */

            string longAttributeName = node.ExtensionName;
			AllType attributeType = ResolveFuncType(node.ExtendWithType);
            // If there is a shortname AND a long name, create 2 entries in the class table
            if ( node.ExtensionShortName != null && node.ExtensionShortName.Length > 0) {
                string shortAttributeName = node.ExtensionShortName;
				ExtendClass(attributeType, longAttributeName, shortAttributeName);
            } else {
                // If only a long name is used, ignore the shortAttribute
                ExtendClass(attributeType, longAttributeName);
            }
        }

        #region CollOPSvisits
        public override void Visit(DequeueQueryNode node)
        {
        }

        public override void Visit(EnqueueQueryNode node)
        {
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
        }

        public override void Visit(ExtractMinQueryNode node)
        {
        }

        public override void Visit(PopQueryNode node)
        {
        }

        public override void Visit(PushQueryNode node)
        {
        }

        public override void Visit(SelectAllQueryNode node)
        {
        }

        public override void Visit(SelectQueryNode node)
        {
        }

        #endregion

        public override void Visit(PredicateNode node)
        {
            _currentNode = node;
            string predicateName = node.Name;
            EnterSymbol(predicateName, AllType.BOOL);
            OpenScope();
            foreach (PredicateParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            VisitChildren(node);
            CloseScope();
        }

        public override void Visit(PredicateParameterNode node)
        {
            _currentNode = node;
            EnterSymbol(node.Name, ResolveFuncType(node.Type));
        }

        public override void Visit(CollectionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IfElseIfElseNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GraphSetQuery node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }
    }
}
