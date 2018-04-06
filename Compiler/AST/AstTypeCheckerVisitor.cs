using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;
using Compiler.AST.SymbolTable;
using System.Diagnostics;

namespace Compiler.AST
{
    public class AstTypeCheckerVisitor : AstVisitorBase
    {
        private AllType collectionRetrieveType = AllType.VOID;
        bool isCollection;

        SymTable _createdSymbolTabe;

        public AstTypeCheckerVisitor(SymTable symbolTable)
        {
            _createdSymbolTabe = symbolTable;
        }



        //-----------------------------Visitor----------------------------------------------
        public override void Visit(ParameterNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(SetQueryNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ExtendNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(PredicateNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

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

                    _createdSymbolTabe.WrongTypeError(node, RetrieveVar, collectionInQuery);
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
                bool isCollectionInQuery;
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetriever;
                AllType? RetrieveVar = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetriever, false);

                if (collection == RetrieveVar && isCollectionInQuery && !isCollectionRetriever)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node, RetrieveVar, collection);
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
                bool isCollectionInQuery;
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery , false);
                bool isCollectionRetrieveVar;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar , false);

                if (nameDeclaredForRetrieve.ToString() == collectionNameType.ToString() && nameDeclaredForRetrieve.ToString() == node.Type && isCollectionInQuery && isCollectionRetrieveVar)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node, nameDeclaredForRetrieve, collectionNameType);
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
                bool isCollectionInQuery;
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetriever;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetriever, false);

                if (collectionNameType.ToString() == nameDeclaredForRetrieve.ToString() && nameDeclaredForRetrieve.ToString() == node.Type && isCollectionInQuery && !isCollectionRetriever)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node, nameDeclaredForRetrieve, collectionNameType);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(PushQueryNode node)
        {
            bool isCollectionVarToAdd;
            AllType? varToAdd;
            bool isCollectionInQuery;
            AllType? collectionToAddTo;

            if (_createdSymbolTabe.DeclaredLocally(node.VariableToAdd) && _createdSymbolTabe.DeclaredLocally(node.VariableAddTo))
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.VariableToAdd, out isCollectionVarToAdd, false);
                collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableAddTo, out isCollectionInQuery, false);

                if (varToAdd == collectionToAddTo && !isCollectionVarToAdd && isCollectionInQuery)
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
                    _createdSymbolTabe.WrongTypeError(node, varToAdd, collectionToAddTo);
                }
            }
            else
            {
                _createdSymbolTabe.NotDeclaredError(node);
            }
        }

        public override void Visit(PopQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                bool isCollectionInQuery;
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetriever;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetriever, false);

                if (collection == nameDeclaredForRetrieve && isCollectionInQuery && !isCollectionRetriever)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node, nameDeclaredForRetrieve, collection);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(EnqueueQueryNode node)
        {
            bool isCollectionVarToAdd;
            AllType? varToAdd;
            bool isCollectionInQuery;
            AllType? collectionToAddTo;

            if (_createdSymbolTabe.DeclaredLocally(node.VariableToAdd) && _createdSymbolTabe.DeclaredLocally(node.VariableTo))
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.VariableToAdd, out isCollectionVarToAdd, false);
                collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableTo, out isCollectionInQuery, false);

                if (varToAdd == collectionToAddTo && !isCollectionVarToAdd && isCollectionInQuery)
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
                    _createdSymbolTabe.WrongTypeError(node, varToAdd, collectionToAddTo);
                }
            }
            else
            {
                _createdSymbolTabe.NotDeclaredError(node);
            }
        }

        public override void Visit(DequeueQueryNode node)
        {
            if (node.Parent != null && node.Parent is ExpressionNode)
            {
                bool isCollectionInQuery;
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetrieve;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieve, false);

                if (collection == nameDeclaredForRetrieve && isCollectionInQuery && !isCollectionRetrieve)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node, nameDeclaredForRetrieve, collection);
                }
            }
            
            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
                
            }
        }

        public override void Visit(AddQueryNode node)
        {//her
            
        }

        public override void Visit(CollectionDeclNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(WhereNode node)
        {
            node.Accept(this);
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(GraphNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(FunctionNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(AbstractNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(IfElseIfElseNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(GraphSetQuery node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(DeclarationNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(BoolComparisonNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ExpressionNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ReturnNode node)
        {
            AllType? funcType = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name);
            AllType? returnChild = _createdSymbolTabe.RetrieveSymbol(node.LeftmostChild.Name);

            if(funcType == AllType.VOID)
            {
                //calling return on void function error 
            }
            else if (returnChild == funcType)
            {

            }
            else
            {
                //conflicting types, on return and function
            }
            VisitChildren(node);
        }

        public override void Visit(ForLoopNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ForeachLoopNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(VariableAttributeNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(VariableNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(CodeBlockNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(VariableDclNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

    }
}