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
            AllType? variableType;
            AllType? expressionType;
            AllType? inVariableType;

            foreach (Tuple<VariableAttributeNode, string, ExpressionNode> setQueryElements in node.Attributes)
            {
                variableType = _createdSymbolTabe.RetrieveSymbol(setQueryElements.Item1.Name);
                expressionType = _createdSymbolTabe.RetrieveSymbol(setQueryElements.Item3.Name);
                if (variableType == expressionType)
                {
                    //type correct
                    if (node.InVariable != null)
                    {
                        //to be continue mads making new ting
                    }
                    else
                    {

                    }
                }
                else
                {
                    //type 
                }

            }
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(PredicateNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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

                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            if (node.Parent != null && node.Parent is DeclarationNode)
            {
                bool isCollectionInQuery;
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                bool isCollectionRetrieveVar;
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar, false);

                if (nameDeclaredForRetrieve.ToString() == collectionNameType.ToString() && nameDeclaredForRetrieve.ToString() == node.Type && isCollectionInQuery && isCollectionRetrieveVar)
                {

                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(SelectQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }

            if (node.WhereCondition != null)
            {
                Visit(node.WhereCondition);
            }
        }

        public override void Visit(PushQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            bool isCollectionVarToAdd;
            AllType? varToAdd;
            bool isCollectionInQuery;
            AllType? collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableCollection, out isCollectionInQuery, false);

            if (node.VariableToAdd is ConstantNode)
            {
                ConstantNode test = (ConstantNode)node.VariableToAdd;
                varToAdd = test.Type_enum;
                if (varToAdd == collectionToAddTo && isCollectionInQuery)
                {
                    //constant is fine, collection is fine
                }
                VisitChildren(node.VariableToAdd);
            }
            else
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.variableName, out isCollectionVarToAdd, false);
                if (varToAdd == collectionToAddTo && !isCollectionVarToAdd && isCollectionInQuery)
                {
                    //varie is fine, collection is fine
                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node.variableName, node.VariableCollection);
                }
                VisitChildren(node.VariableToAdd);
            }
        }

        public override void Visit(PopQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }
        }

        public override void Visit(EnqueueQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            bool isCollectionVarToAdd;
            AllType? varToAdd;
            bool isCollectionInQuery;
            AllType? collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableCollection, out isCollectionInQuery, false);

            if (node.VariableToAdd is ConstantNode)
            {
                ConstantNode test = (ConstantNode)node.VariableToAdd;
                varToAdd = test.Type_enum;
                if (varToAdd == collectionToAddTo && isCollectionInQuery)
                {
                    //constant is fine, collection is fine
                }
                VisitChildren(node.VariableToAdd);
            }
            else
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.variableName, out isCollectionVarToAdd, false);
                if (varToAdd == collectionToAddTo && !isCollectionVarToAdd && isCollectionInQuery)
                {
                    //varie is fine, collection is fine
                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node.variableName, node.VariableCollection);
                }
                VisitChildren(node.VariableToAdd);
            }
        }


        public override void Visit(DequeueQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
                    _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
                }
            }
        }

        public override void Visit(AddQueryNode node)
        {//her

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
            /*
            AllType? funcType = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name);
            AllType? returnChild = _createdSymbolTabe.RetrieveSymbol(node.LeftmostChild.Name);

            if (funcType == AllType.VOID)
            {
                //calling return on void function error 
            }
            else if (isRetrunType == isFuncTypeCollection)
            {
                if (funcType == returnType)
                {

                }
                else
                {
                    //ERROR, conflicting function and return type
                }
            }
            else
            {
                //ERROR, one is collection, other isn't
            }
            VisitChildren(node);
            */
        }

        public override void Visit(ForLoopNode node)
        {
            //fejl i parser, forloopnode gemmer ikke variablen som er udgangspunkt for loop, hvis den allerede er deklareret
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ForeachLoopNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            //fejl i parser, mærkelig exception ved codeblock
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
            VisitChildren(node);
        }

        public override void Visit(VariableDclNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

    }
}
