using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;

namespace Compiler.AST.Symbol_Table
{
    class AstSymbolTableCreatorVisitor : IAstVisitorBase
    {
        private Dictionary<string, List<SymbolTableEntry>> _symbolTable = new Dictionary<string, List<SymbolTableEntry>>();
        private uint _globalDepth;

        public void BuildSymbolTable(AbstractNode root)
        {
            VisitRoot(root);
        }

        private void EnterSymbol(string name, AllType type)
        {
            if (RetrieveSymbol(name) != null)
            {
                throw new Exception($"Duplicate definition of {name}");
            }
            else if(!_symbolTable.ContainsKey(name))
                _symbolTable.Add(name, new List<SymbolTableEntry>());

            _symbolTable[name].Add(new SymbolTableEntry(type, _globalDepth));
        }

        private SymbolTableEntry RetrieveSymbol(string name)
        {
            List<SymbolTableEntry> entriesWithThisName = _symbolTable[name];
            SymbolTableEntry result = entriesWithThisName.Where(x => x.Depth == _globalDepth).First();
            return result;
        }

        private bool DeclaredLocally(string name)
        {
            return RetrieveSymbol(name) == null;
        }

        private void OpenScope()
        {
            ++_globalDepth;
        }

        private void CloseScope()
        {
            --_globalDepth;
        }

        //All the visit stuff-----------------------------------------
        public void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }

        public void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }
        }

        public void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public void Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionParameterNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(StartNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(ProgramNode node)
        {
            throw new NotImplementedException();
        }



    }
}
