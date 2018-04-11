﻿using System;
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
                Visit(node.VariableToAdd);
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
                Visit(node.VariableToAdd);
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
                Visit(node.VariableToAdd);
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
                Visit(node.VariableToAdd);
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
        {
            bool isCollectionTargetColl;
            AllType? TypeOfTargetCollection = _createdSymbolTabe.RetrieveSymbol(node.ToVariable, out isCollectionTargetColl, false);
            if (node.IsGraph)
            {//control statement for input to graphs
                if (TypeOfTargetCollection == AllType.VERTEX && isCollectionTargetColl)
                {
                    foreach (VariableDclNode vertexdcl in node.Dcls)
                    {
                        if (vertexdcl.Type_enum == AllType.VERTEX)
                        {
                            //both collection is vertex, and current dcl is a vertexdcl
                        }
                        else
                        {
                            //error raised, because af dcl is not of type vertex.
                            _createdSymbolTabe.WrongTypeError(vertexdcl.Name, node.ToVariable);
                        }
                    }
                }
                else if (TypeOfTargetCollection == AllType.EDGE && isCollectionTargetColl)
                {
                    foreach (VariableDclNode edgedcl in node.Dcls)
                    {
                        if (edgedcl.Type_enum == AllType.VERTEX)
                        {
                            //both collection is edge, and current dcl is a edgedcl
                        }
                        else
                        {
                            //error raised, because af dcl is not of type edge.
                            _createdSymbolTabe.WrongTypeError(edgedcl.Name, node.ToVariable);
                        }
                    }
                }
                else
                {//control statement for extended collections on graph
                    StringBuilder dclList = new StringBuilder();
                    dclList.Append($"declaration_set(");
                    foreach (AbstractNode v in node.Dcls)
                    {
                        dclList.Append($"{v.Name}, ");
                    }
                    dclList.Remove(dclList.Length - 2, 2);
                    dclList.Append(")");
                    _createdSymbolTabe.WrongTypeError(dclList.ToString(), node.ToVariable);
                }
            }
            //if the ToVariable is a collection:
            else
            {
                if (_createdSymbolTabe.CheckIfDefined(node.ToVariable))
                {

                }
                else
                {
                    _createdSymbolTabe.UndeclaredError(node.ToVariable);
                }
            }

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
            VisitChildren(node);
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
        {/*
            AllType? targetType = node.Attributes.Item1.Type_enum;
            AllType? setType = AllType.BOOL; //TODO når expression node er færdig, kan vi finde ud af hvad settype er.
            if (targetType == setType)
            {

            }
            else
            {
                _createdSymbolTabe.WrongTypeError(node.Attributes.Item1.Name, node.Attributes.Item3.Name);
            }*/
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
            bool isReturnTypeCollection = false;
            AllType? funcType = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isReturnTypeCollection, false);
            bool isFunctionTypeCollection = false;
            AllType? returnType = _createdSymbolTabe.RetrieveSymbol(node.LeftmostChild.Name, out isFunctionTypeCollection, false);

            if (funcType == AllType.VOID)
            {
                //calling return on void function error 
            }
            else if (isReturnTypeCollection == isFunctionTypeCollection)
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

        }

        public override void Visit(ForLoopNode node)
        {
            //fejl i parser, forloopnode gemmer ikke variablen som er udgangspunkt for loop, hvis den allerede er deklareret
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ForeachLoopNode node)
        {
            bool isCollectionInForeach;
            AllType? collectionType = _createdSymbolTabe.RetrieveSymbol(node.InVariableName, out isCollectionInForeach, false);

            if (node.VariableType_enum == collectionType)
            {
                //collection type and variable type is the same.
            }
            else
            {
                _createdSymbolTabe.WrongTypeError(node.VariableName, node.InVariableName);
            }
            VisitChildren(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(VariableAttributeNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(VariableNode node)
        {
            //måske ikke helt færdig, mangler expression bliver done 
            VisitChildren(node);
        }

        public override void Visit(CodeBlockNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(VariableDclNode node)
        {
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(OperatorNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ConstantNode node)
        {
            throw new NotImplementedException();
        }
    }
}
