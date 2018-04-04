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
        private uint _globalDepth;
        
        public AstSymbolTableCreatorVisitor()
        {
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

        public void BuildSymbolTable(AbstractNode root)
        {
            VisitRoot(root);
        }

        private void EnterSymbol(string name, AllType type, int linenumber)
        {
            if (DeclaredLocally(name))
            {
                Console.WriteLine($"Duplicate definition of {name} at line number {linenumber}");
            }
            else if (!_symbolTable.ContainsKey(name))
                _symbolTable.Add(name, new List<SymbolTableEntry>());

            _symbolTable[name].Add(new SymbolTableEntry(type, _globalDepth));
        }

        private SymbolTableEntry RetrieveSymbol(string name)
        {
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

        private bool DeclaredLocally(string name)
        {
            return RetrieveSymbol(name) != null;
        }

        private void OpenScope()
        {
            ++_globalDepth;
        }

        private void CloseScope()
        {
            //Makes variables unreachable, when their scope is exited
            foreach (List<SymbolTableEntry> list in _symbolTable.Values)
            {
                foreach (SymbolTableEntry entry in list)
                {
                    if (entry.Depth == _globalDepth)
                        entry.Reachable = false;
                }
            }
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
            AllType type = ResolveFuncType(node.ReturnType);
            string functionName = node.Name;

            EnterSymbol(functionName, type, node.LineNumber);
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
            AllType parameterType = ResolveFuncType(node.Type);
            EnterSymbol(node.Name, parameterType, node.LineNumber);
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
            string graphName = node.Name;
            EnterSymbol(graphName, AllType.GRAPH, node.LineNumber);
            VisitChildren(node);
        }

        public override void Visit(VertexNode node)
        {
            /* Missing the values of the vertex*/
            string vertexName = node.Name;
            EnterSymbol(vertexName, AllType.VERTEX, node.LineNumber);
        }

        public override void Visit(EdgeNode node)
        {
            /* Missing the values of the edge*/
            string edgeName = node.Name;
            EnterSymbol(edgeName, AllType.EDGE, node.LineNumber);
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
            /* Goes through all declared variables of the extended type, and adds the attribute as a recognized term 
             * So if you extend vertices with an int i, and there is a vertex variable named vert,
             * it will now recognize vert.i */
            string longAttributeName = node.ExtensionName;
            string shortAttributeName = node.ExtensionShortName;
            AllType attributeType = ResolveFuncType(node.ExtendWithType);

            foreach (KeyValuePair<string, List<SymbolTableEntry>> pair in _symbolTable)
            {
                foreach (SymbolTableEntry entry in pair.Value)
                {
                    if (entry.Type == ResolveFuncType(node.ClassToExtend))
                    {
                        EnterSymbol($"{pair.Key}.{longAttributeName}", attributeType, node.LineNumber);
                        EnterSymbol($"{pair.Key}.{shortAttributeName}", attributeType, node.LineNumber);
                    }
                }
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
            string predicateName = node.Name;
            EnterSymbol(predicateName, AllType.BOOL, node.LineNumber);
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
            EnterSymbol(node.Name, ResolveFuncType(node.Type), node.LineNumber);
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
