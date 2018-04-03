using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.SymbolTable;

namespace Compiler.AST
{
    class AstTypeCheckerVisitor : AstVisitorBase
    {
        private Dictionary<string, List<SymbolTableEntry>> _symbolTable = new Dictionary<string, List<SymbolTableEntry>>();

        public AstTypeCheckerVisitor(Dictionary<string, List<SymbolTableEntry>> symbolTable)
        {
            _symbolTable = symbolTable;
        }

        //-----------------------------Visitor----------------------------------------------
        public override void Visit(FunctionParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StartNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VertexNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SetQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtendNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PredicateNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DequeueQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PopQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SelectAllQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SelectQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PushQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EnqueueQueryNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(PredicateParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(CollectionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(WhereNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EdgeNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(GraphNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ProgramNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FunctionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
