﻿using System;
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
        public bool errorOccured = false;
        private AllType collectionRetrieveType = AllType.VOID;
        bool isCollection;

        SymTable _createdSymbolTabe;

        public AstTypeCheckerVisitor(SymTable symbolTable)
        {
            _createdSymbolTabe = symbolTable;
        }

        private void Error()
        {
            errorOccured = true;
        }

        //-----------------------------Visitor----------------------------------------------
        public override void Visit(FunctionParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
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

        #region CollopsVisits

        public override void Visit(ExtractMaxQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                bool isCollectionInQuery;
                AllType? collectionInQuery = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetrieveVar;
                AllType? RetrieveVar = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar, false);

                if (collectionInQuery == RetrieveVar && isCollectionInQuery && !isCollectionRetrieveVar)
                {

                }
                else
                {
                    
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, false);
                AllType? collectionParent = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, false);

                if (collection == collectionParent)
                {

                }
                else
                {
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            if (node.Parent != null && node.Parent is DeclarationNode)
            {
                bool isCollectionRetrieveVar;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar , false);
                bool isCollectionInQuery;
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery , false);

                if (nameDeclaredForRetrieve.ToString() == collectionNameType.ToString() && nameDeclaredForRetrieve.ToString() == node.Type && isCollectionInQuery && isCollectionRetrieveVar)
                {

                }
                else
                {
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(SelectQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, false);
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, false);

                if (collectionNameType.ToString() == nameDeclaredForRetrieve.ToString() && nameDeclaredForRetrieve.ToString() == node.Type)
                {

                }
                else
                {
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(PushQueryNode node)
        {
            AllType? varToAdd;
            AllType? collectionToAddTo;

            if (_createdSymbolTabe.DeclaredLocally(node.VariableToAdd) && _createdSymbolTabe.DeclaredLocally(node.VariableAddTo))
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.VariableToAdd, false);
                collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableAddTo, false);

                if (varToAdd == collectionToAddTo)
                {
                    if (node.WhereCondition != null)
                    {
                        Visit(node.WhereCondition);
                    }
                    else
                    {
                        //very gud :)))
                    }
                }
                else
                {
                    Console.WriteLine($"Variable {varToAdd} and collection {collectionToAddTo} are not of same type, at line number {node.LineNumber}");
                    Error();
                }
            }
            else
            {
                Console.WriteLine($"Variable or collection are not declared at line number {node.LineNumber}");
                Error();
            }
        }

        public override void Visit(PopQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, false);
                AllType? collectionParent = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, false);

                if (collection == collectionParent)
                {

                }
                else
                {
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(EnqueueQueryNode node)
        {
            AllType? varToAdd;
            AllType? collectionToAddTo;

            if (_createdSymbolTabe.DeclaredLocally(node.VariableToAdd) && _createdSymbolTabe.DeclaredLocally(node.VariableTo))
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.VariableToAdd, false);
                collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableTo, false);

                if (varToAdd == collectionToAddTo)
                {
                    if (node.WhereCondition != null)
                    {
                        Visit(node.WhereCondition);
                    }
                    else
                    {
                        //very gud :)))
                    }
                }
                else
                {
                    Console.WriteLine($"Variable {varToAdd} and collection {collectionToAddTo} are not of same type, at line number {node.LineNumber}");
                    Error();
                }
            }
            else
            {
                Console.WriteLine($"Variable or collection are not declared at line number {node.LineNumber}");
                Error();
            }
        }

        public override void Visit(DequeueQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, false);
                AllType? collectionParent = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, false);

                if (collection == collectionParent)
                {

                }
                else
                {
                    Console.WriteLine($"Type incorrect at line number {node.LineNumber}");
                }
            }
            
            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        #endregion

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

        public override void Visit(BoolComparisonNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }
    }
}
