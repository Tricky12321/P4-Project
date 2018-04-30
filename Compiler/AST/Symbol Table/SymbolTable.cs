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
        private Dictionary<AllType, Dictionary<string, ClassEntry>> _classesTable = new Dictionary<AllType, Dictionary<string, ClassEntry>>();
        private Dictionary<string, Dictionary<string, FunctionParameterEntry>> _functionTable = new Dictionary<string, Dictionary<string, FunctionParameterEntry>>();

        private Dictionary<string, List<AllType>> _predicateTable = new Dictionary<string, List<AllType>>();

        // Get/Set to keep track of what is the deepest methods are stored!
        public string CurrentLine => GetLineNumber();

        private uint _globalDepthTrue = 0;
        private List<string> _reservedKeywords = new List<string> {
            "VertexFrom","VertexTo", "Directed","Vertices","Edges","vertex",
            "edge","graph","int","decimal","string","void","bool","if","elseif","else","to",
            "in","for","foreach","return","while","set","select","selectall","from","where",
            "add","collection","run","with","extend","predicate","pop","push","peek","enqueue",
            "dequeue","extractmin","extractmax","print","inf","true","false"
        };

        private uint _globalDepth
        {
            get
            {
                return _globalDepthTrue;
            }
            set
            {
                if (value > _maxDepth)
                {
                    _maxDepth = value;
                    _globalDepthTrue = value;
                }
                else
                {
                    _globalDepthTrue = value;
                }
            }
        }
        private AbstractNode _currentNode;
        public bool errorOccured = false;
        private Scope _currentScope = new Scope(null, 0);
        private uint _maxDepth = 0;

        public void SetCurrentNode(AbstractNode node)
        {
            _currentNode = node;
        }

        public string Prefix => _currentScope.Prefix;

        public string GetName(string Name)
        {
            if (Prefix != "")
            {
                return Prefix + "." + Name;
            }
            else
            {
                return Name;
            }
        }

        public SymTable()
        {
            // Class definitions
            _classesTable.Add(AllType.GRAPH, new Dictionary<string, ClassEntry>());
            _classesTable.Add(AllType.VERTEX, new Dictionary<string, ClassEntry>());
            _classesTable.Add(AllType.EDGE, new Dictionary<string, ClassEntry>());
            _classesTable.Add(AllType.COLLECTION, new Dictionary<string, ClassEntry>());
            // ClassEntries

            ClassEntry VertexFrom = new ClassEntry("VertexFrom", AllType.VERTEX);
            ClassEntry VertexTo = new ClassEntry("VertexTo", AllType.VERTEX);
            ClassEntry Adjacent = new ClassEntry("Adjacent", AllType.VERTEX, true);
            ClassEntry IsAdjacent = new ClassEntry("IsAdjacent", AllType.BOOL);
            ClassEntry EdgeBetween = new ClassEntry("EdgeBetween", AllType.EDGE);
            ClassEntry EdgesBetween = new ClassEntry("EdgesBetween", AllType.EDGE, true);
            ClassEntry IsEmpty = new ClassEntry("IsEmpty", AllType.BOOL);
            ClassEntry Directed = new ClassEntry("Directed", AllType.BOOL);
            ClassEntry Vertices = new ClassEntry("Vertices", AllType.VERTEX, true);
            ClassEntry Edges = new ClassEntry("Edges", AllType.EDGE, true);
            // Method Calls
            _classesTable[AllType.EDGE].Add(VertexFrom.Name, VertexFrom);
            _classesTable[AllType.EDGE].Add(VertexTo.Name, VertexTo);
            _classesTable[AllType.GRAPH].Add("EdgeBetween", EdgeBetween);
            _classesTable[AllType.GRAPH].Add("EdgesBetween", EdgesBetween);
            _classesTable[AllType.GRAPH].Add("Adjacent", Adjacent);
            _classesTable[AllType.GRAPH].Add("IsAdjacent", Adjacent);
            _classesTable[AllType.GRAPH].Add("Directed", Directed);
            _classesTable[AllType.COLLECTION].Add("IsEmpty", IsEmpty);
            // Collections
            _classesTable[AllType.GRAPH].Add("Vertices", Vertices);
            _classesTable[AllType.GRAPH].Add("Edges", Edges);

        }

        private bool CheckReserved(string name) => _reservedKeywords.Contains(name);

        public bool CheckIfDefined(string name)
        {
            // Store the prefix of the current scope
            string prefix = _currentScope.Prefix;
            // Determine what to check for
            string toCheckFor = "";
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
            return _symTable.ContainsKey(toCheckFor);
        }
        /// <summary>
        /// Gets the type of the variable.
        /// This is only for use in the TypeChecker, if the symbol table was constructed properbly
        /// </summary>
        /// <returns>The variable type.</returns>
        /// <param name="name">Name of the variable to find in the symbol table</param>
        /// 
        public AllType GetVariableType(string name)
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
                    return _symTable[toCheckFor].Type;
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
            return _symTable[toCheckFor].Type;
        }
        /// <summary>
        /// Adds a varaible to the symbol table
        /// </summary>
        /// <param name="name">Name of variable</param>
        /// <param name="type">Type of varialbe</param>
        /// <param name="IsCollection">If set to <c>true</c> its a collection.</param>
        public void EnterSymbol(string name, AllType type, bool IsCollection = false)
        {
            if (!CheckReserved(name))
            {

                if (name != null && name != "")
                {

                    if (!_symTable.ContainsKey(GetName(name)))
                    {
                        _symTable.Add(GetName(name), new SymbolTableEntry(type, IsCollection, _globalDepth));
                    }
                    else
                    {
                        AlreadyDeclaredError(name);
                    }
                }
            }
            else
            {
                ReservedKeyword(name);
            }

        }

        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        /// <returns>The attribute type. returns null if it does not exist!, does not support collection</returns>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public AllType? GetAttributeType(string name, AllType type, out bool IsCollection)
        {
            name = name.Trim('\'');
            List<string> names = new List<string>();
            names.Add(name);
            AllType? output = RetrieveTypeFromClasses(names, type, out IsCollection, false);
            return output;
        }

        /// <summary>
        /// Does the same as GetAttribyteType, but ignores the collection requirement
        /// </summary>
        /// <returns>The attribute type.</returns>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public AllType? GetAttributeType(string name, AllType type)
        {
            name = name.Trim('\'');
            bool IsCollection;
            return GetAttributeType(name, type, out IsCollection);
        }

        public bool IsExtended(string Name, AllType Class)
        {
            Name = Name.Trim('\'');
            if (IsClass(Class) && _classesTable[Class].ContainsKey(Name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns if a AttributeDefined in the symbol table
        /// </summary>
        /// <returns><c>true</c>, if defined was attributed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name of the attribute</param>
        /// <param name="type">Must be a class, and used for lookup in symbol table</param>
        public bool AttributeDefined(string name, AllType type)
        {
            if (name == null || name == "")
            {
                return true;
            }
            // Remove ' from attributes
            if (name.Contains("'"))
            {
                name = name.Substring(1, name.Length - 2);
            }
            bool IsCollection;
            List<string> names = new List<string>();
            names.Add(name);
            bool output = RetrieveTypeFromClasses(names, type, out IsCollection, false) != null;
            if (!output)
            {
                AttributeUndeclared(name, type);
            }
            return output;
        }

        /// <summary>
        /// Retrieves the type from classes.
        /// </summary>
        /// <returns>The type from classes.</returns>
        /// <param name="names">Names.</param>
        /// <param name="type">Type.</param>
        /// <param name="IsCollection">If set to <c>true</c> is collection.</param>
        /// <param name="ShowErrors">If set to <c>true</c> show errors.</param>
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
        /// <summary>
        /// Retrieves the symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="Name">Name.</param>
        /// <param name="ShowErrors">If set to <c>true</c> show errors.</param>
        public AllType? RetrieveSymbol(string Name, bool ShowErrors = true)
        {
            bool IsCollection;
            return RetrieveSymbol(Name, out IsCollection, ShowErrors);
        }
        /// <summary>
        /// Retrieves the symbol.
        /// </summary>
        /// <returns>The symbol.</returns>
        /// <param name="Name">Name.</param>
        /// <param name="IsCollection">If set to <c>true</c> is collection.</param>
        /// <param name="ShowErrors">If set to <c>true</c> show errors.</param>
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
                    var Names = Name.Split('.').ToList();
                    if (Names.Count() > 1)
                    {
                        var ClassName = Names[0];
                        Names.RemoveAt(0);
                        var test = GetName(ClassName);
                        var type = RetrieveSymbol(ClassName) ?? default(AllType);

                        var output = RetrieveTypeFromClasses(Names, type, out IsCollection);
                        return output;
                    }

                    IsCollection = false;
                    return null;
                }
            }
            else
            {
                Name = GetName(Name);
                bool match = _symTable.ContainsKey(Name);
                var names = Name.Split('.').ToList();

                while (!match && names.Count > 1)
                {
                    names.RemoveAt(names.Count - 2);
                    Name = "";
                    bool first = true;
                    foreach (var item in names)
                    {
                        if (first)
                        {
                            Name += item;
                            first = false;
                        }
                        else
                        {
                            Name += "." + item;
                        }
                    }
                    if (_symTable.ContainsKey(Name))
                    {
                        match = true;
                        IsCollection = _symTable[Name].IsCollection;
                        var type = _symTable[Name].Type;
                        return type;
                    }
                    else
                    {
                        IsCollection = false;
                        return null;
                    }
                }
                if (match)
                {
                    IsCollection = _symTable[Name].IsCollection;
                    var type = _symTable[Name].Type;
                    return type;
                }
                IsCollection = false;
                return null;
            }
        }

        public bool IsFunctionDeclared(string FunctionName)
        {
            return _symTable.ContainsKey(FunctionName);
        }

        public AllType? FunctionReturnType(string FunctionName)
        {
            if (_symTable.ContainsKey(FunctionName))
            {
                return _symTable[FunctionName].Type;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns if the type given is a class.
        /// </summary>
        /// <returns><c>true</c>, if class was ised, <c>false</c> otherwise.</returns>
        /// <param name="Type">Type.</param>
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
        /// <summary>
        /// Returns if a variable is avalible in the current scope
        /// </summary>
        /// <returns><c>true</c>, if locally was declareded, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool DeclaredLocally(string name)
        {
            return CheckIfDefined(name);
        }
        /// <summary>
        /// Opens a new scope.
        /// </summary>
        /// <param name="type">Type of the scope to open *IMPORTANT*</param>
        public void OpenScope(BlockType type)
        {
            Scope NewScope = new Scope(_currentScope, _globalDepth);
            _currentScope = NewScope;
            _currentScope.AddPrefix(type);
            ++_globalDepth;
        }
        /// <summary>
        /// Opens a new scope for functions or predicates.
        /// </summary>
        /// <param name="value">Value.</param>
        public void OpenScope(string value)
        {
            Scope NewScope = new Scope(_currentScope, _globalDepth);
            _currentScope = NewScope;
            _currentScope.AddPrefix(value);
            ++_globalDepth;
        }
        /// <summary>
        /// Closes the current scope, and sets the current scope, to the previous.
        /// Also sets all variables that were created in the current scope, to unreacheable. 
        /// </summary>
        public void CloseScope()
        {
            if (_currentScope.ParentScope == null)
            {
                Console.WriteLine("Cannot close scope, since its the last scope!");
                errorOccured = true;
            }
            else
            {
                _currentScope = _currentScope.CloseScope();
                _symTable.Values.Where(y => y.Depth == _globalDepth && y.Reachable == true).ToList().ForEach(y => y.Reachable = false);
                --_globalDepth;
            }
        }

        public bool CheckAttributeDefined(string name, AllType Class)
        {
            bool IsCollection;
            List<string> names = new List<string> { name };
            return RetrieveTypeFromClasses(names, Class, out IsCollection, false) != null;
        }

        /// <summary>
        /// Extends the class with a long and a short name
        /// </summary>
        /// <param name="Type">Type.</param>
        /// <param name="longAttribute">Long attribute.</param>
        /// <param name="shortAttribute">Short attribute.</param>
        /// <param name="ClassType">Class type.</param>
        public void ExtendClass(AllType Type, string longAttribute, string shortAttribute, AllType ClassType, bool IsCollection = false)
        {
            if (longAttribute == shortAttribute)
            {
                AttributeIdenticalError(longAttribute, shortAttribute);
            }

            if (CheckAttributeDefined(longAttribute, ClassType))
            {
                AlreadyDeclaredError(longAttribute);
            }

            if (CheckAttributeDefined(longAttribute, ClassType))
            {
                AlreadyDeclaredError(shortAttribute);
            }

            if (errorOccured == false)
            {
                ClassEntry Short = new ClassEntry(shortAttribute, Type, IsCollection);
                ClassEntry Long = new ClassEntry(longAttribute, Type, IsCollection);
                Short.Collection = IsCollection;
                Long.Collection = IsCollection;
                _classesTable[ClassType].Add(shortAttribute, Short);
                _classesTable[ClassType].Add(longAttribute, Long);
            }
        }
        /// <summary>
        /// Extends the class with only one attribute name
        /// </summary>
        /// <param name="Type">Type.</param>
        /// <param name="longAttribute">Long attribute.</param>
        /// <param name="ClassType">Class type.</param>
        /// <param name="IsCollection">If set to <c>true</c> is collection.</param>
        public void ExtendClass(AllType Type, string longAttribute, AllType ClassType, bool IsCollection = false)
        {
            if (CheckAttributeDefined(longAttribute, ClassType))
            {
                AlreadyDeclaredError(longAttribute);
            }
            if (errorOccured == false)
            {
                ClassEntry Long = new ClassEntry(longAttribute, Type, IsCollection);
                Long.Collection = IsCollection;
                _classesTable[ClassType].Add(longAttribute, Long);
            }
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

        public void EnterFunctionParameter(string FunctionName, string ParameterName, AllType ParameterType)
        {
            FunctionParameterEntry FuncParEntry = new FunctionParameterEntry(ParameterName, ParameterType);
            if (!_functionTable.ContainsKey(FunctionName))
            {
                _functionTable.Add(FunctionName, new Dictionary<string, FunctionParameterEntry>());
                _functionTable[FunctionName].Add(ParameterName, FuncParEntry);
            }
            else
            {
                _functionTable[FunctionName].Add(ParameterName, FuncParEntry);
            }
        }

        public AllType? GetParameterType(string FunctionName, string ParameterName)
        {
            if (_functionTable.ContainsKey(FunctionName))
            {
                if (_functionTable[FunctionName].ContainsKey(ParameterName))
                {
                    return _functionTable[FunctionName][ParameterName].Type;
                }
                else
                {
                    UndefinedParameter(ParameterName, FunctionName);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void AddPredicateToList(string PredicateName)
        {
            _predicateTable.Add(GetName(PredicateName), new List<AllType>());
        }

        public void EnterPredicateParameter(string PredicateName, AllType ParameterType)
        {
            CloseScope();
            _predicateTable[GetName(PredicateName)].Add(ParameterType);
            OpenScope(PredicateName);
        }

        public List<AllType> GetPredicateParameters(string PredicateName)
        {
            if (_globalDepth > 0)
            {
                CloseScope();
                PredicateName = GetName(PredicateName);
                OpenScope(PredicateName);
            }

            if (_predicateTable.ContainsKey(PredicateName) && PredicateName.Contains("."))
            {
                return _predicateTable[PredicateName];
            } else {
                var names = PredicateName.Split('.').ToList();
                if (names.Count > 2) {
                    names.RemoveAt(names.Count - 2);
                } else {
                    names.RemoveAt(0);
                }
                string name = "";
                bool first = true;
                foreach (var item in names)
                {
                    if (first) {
                        name += item;
                        first = false;
                    } else {
                        name += "." + item;
                    }
                }

                return GetPredicateParameters(name);
            }
            return null;
        }

        public List<FunctionParameterEntry> GetParameterTypes(string FunctionName)
        {
            var Output = new List<FunctionParameterEntry>();
            if (_functionTable.ContainsKey(FunctionName))
            {
                foreach (var parameter in _functionTable[FunctionName])
                {
                    Output.Add(parameter.Value);
                }
            }
            return Output;
        }

        // -------------------------------------------------------
        // ERRORS:
        // -------------------------------------------------------
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
            Console.WriteLine(name + " is undefined in this scope! - " + GetLineNumber());
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

        public void AttributeIllegal()
        {
            Console.WriteLine("Type of attribute must be of type Integer or Decimal " + GetLineNumber());
            Error();
        }

        public void AttributeNotExtendedOnClass(string attriName, AllType? TypeOfClass)
        {
            Console.WriteLine($"Given attribute: {attriName} is not extended on Class: {TypeOfClass.ToString()} " + GetLineNumber());
            Error();
        }

        public void NoAttriProvidedCollNeedsToBeIntOrDecimalError()
        {
            Console.WriteLine($"If no attribute is provided for assortment, the specified collection must be of type Decimal or Integer " + GetLineNumber());
            Error();
        }

        public void ExtractCollNotIntOrDeciError()
        {
            Console.WriteLine("If a attribute is provided for assortment, the specified collection must not be of type Decimal or Integer " + GetLineNumber());
            Error();
        }

        public void WrongTypeError(string variable1, string variable2)
        {
            Console.WriteLine($"Variable {variable1} and {variable2} are missmatch of types. Line number {GetLineNumber()}");
            Error();
        }

        public void WrongTypeErrorCollection(string variable1, string variable2)
        {
            Console.WriteLine($"Variable {variable1} and {variable2} are missmatch of collection. Line number {GetLineNumber()}");
            Error();
        }

        public void WrongTypeConditionError()
        {
            Console.WriteLine($"There is a type mismatch in the condition on Line number {GetLineNumber()}");
            Error();
        }

        public void AttributeIdenticalError(string variable1, string variable2)
        {
            Console.WriteLine($"Attribute name {variable1} is identical with {variable2} which is illegal! {GetLineNumber()}");
            Error();
        }

        public void BoolAdditionError(string variable1)
        {
            Console.WriteLine($"Cannot add Booleans {variable1}! {GetLineNumber()}");
        }

        public void ReservedKeyword(string name)
        {
            Console.WriteLine($"Variable or Attribute name {name} is a reserved keyword, and cannot be used! {GetLineNumber()}");
            Error();
        }

        public void NoFunctionWithNameDeclaredError(string name)
        {
            Console.WriteLine($"There is no function with the name {name} declared {GetLineNumber()}");
            Error();
        }

        public void DuplicateParameterInFunction(string ParameterName, string FunctionName)
        {
            Console.WriteLine($"There is already a parameter with the name {ParameterName} in function {FunctionName} declared {GetLineNumber()}");
            Error();
        }

        public void UndefinedParameter(string ParameterName, string FunctionName)
        {
            Console.WriteLine($"There is no parameter defined with the name {ParameterName} in function {FunctionName} {GetLineNumber()}");
            Error();
        }

        public void TypeExpressionMismatch()
        {
            Console.WriteLine($"There is a type mismatch in the expression on {GetLineNumber()}");
            Error();
        }

        public void MainHasParameters()
        {
            Console.WriteLine($"The Main function has parameters, which is illegal! {GetLineNumber()}");
            Error();
        }

        public void MainHasWrongReturnType()
        {
            Console.WriteLine($"The Main function has a wrong return type! Only void is allowed! {GetLineNumber()}");
            Error();
        }

        public void MainUndefined()
        {
            Console.WriteLine($"There is no Main function declared, program wont work!");
            Error();
        }

        public void TargetIsNotCollError(string name)
        {
            Console.WriteLine($"Target variable: {name} is not of type collection {GetLineNumber()}");
            Error();
        }

        public void FromVarIsNotCollError(string name)
        {
            Console.WriteLine($"The variable retrieved from: {name} is not of type collection {GetLineNumber()}");
            Error();
        }

        public void UndeclaredFunction(string FunctionName)
        {
            Console.WriteLine($"The function {FunctionName} is undeclared, and can therefore not be used {GetLineNumber()}");
            Error();
        }

        public void NonPrintableError()
        {
            Console.WriteLine($"one or more provided variables or constants is not legal to print. {GetLineNumber()}");
            Error();
        }

        public void FunctionIsVoidError(string FunctionName)
        {
            Console.WriteLine($"Trying to return to void function: {FunctionName}, at {GetLineNumber()}");
            Error();
        }

        public void RunFunctionError(string actualParameter, string formalParameter)
        {
            Console.WriteLine($"Actual parameter: {actualParameter} and formal parameter: {formalParameter} are a type missmatch");
            Error();
        }

        public void AddClassVariablesToScope(AllType type)
        {
            if (IsClass(type))
            {
                foreach (var item in _classesTable[type])
                {
                    // Enters all attributes 
                    EnterSymbol("'" + item.Key + "'", item.Value.Type, item.Value.Collection);
                }
            }
        }
    }
}
