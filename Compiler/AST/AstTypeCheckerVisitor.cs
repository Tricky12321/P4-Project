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

        public void VisitChildrenNewScope(AbstractNode node, string Name)
        {
            if (node != null)
            {
                _createdSymbolTabe.OpenScope(Name);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }
                _createdSymbolTabe.CloseScope();
            }
        }

        public void VisitChildrenNewScope(AbstractNode node, BlockType Type)
        {
            if (node != null)
            {
                _createdSymbolTabe.OpenScope(Type);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }
                _createdSymbolTabe.CloseScope();
            }
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

            if (node.Attributes != null)
            {
                //set query has attributes which is placed in a list
                foreach (Tuple<VariableAttributeNode, string, ExpressionNode> Attributes in node.Attributes)
                {
                    variableType = _createdSymbolTabe.RetrieveSymbol(Attributes.Item1.Name);



                    //item 3 is an expression node, item 3 of that includes the parts of it
                    foreach (var ExpressionPart in Attributes.Item3.ExpressionParts)
                    {
                        // bool should only be set to bool, and no addition of bools is allowed
                        if (variableType == AllType.BOOL && Attributes.Item3.ExpressionParts.Count < 0)
                        {
                            break;
                        }

                        //no type check om operator item
                        if (!(ExpressionPart is OperatorNode))
                        {

                            if (ExpressionPart is ConstantNode constant)
                            {
                                if (constant.Type_enum == variableType)
                                {
                                    // type correct
                                }
                            }

                            //can be variable og attribute, therefore using VariableAttributeNode
                            else if (ExpressionPart is VariableAttributeNode expressionVariable)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(expressionVariable.Name);

                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                            }


                            else if (ExpressionPart is SelectQueryNode selectQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(selectQuery.Variable);

                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(selectQuery);
                            }

                            else if (ExpressionPart is SelectAllQueryNode selectAllQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(selectAllQuery.Variable);
                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(selectAllQuery);
                            }

                            else if (ExpressionPart is DequeueQueryNode dequeueQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(dequeueQuery.Variable);
                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(dequeueQuery);
                            }

                            else if (ExpressionPart is PopQueryNode popQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(popQuery.Variable);
                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(popQuery);
                            }

                            else if (ExpressionPart is ExtractMinQueryNode extractMinQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(extractMinQuery.Variable);
                                if (expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(extractMinQuery);
                            }

                            else if (ExpressionPart is ExtractMaxQueryNode extraxtMaxQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(extraxtMaxQuery.Variable);
                                if(expressionType == variableType)
                                {
                                    // type correct
                                }
                                VisitChildren(extraxtMaxQuery);
                            }

                        }
                    }
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
                AllType? collectionInQuery;
                AllType? RetrieveVar;
                bool isCollectionInQuery;
                bool isCollectionRetrieveVar;


                collectionInQuery = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                RetrieveVar = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar, false);

                bool isSameType = collectionInQuery == RetrieveVar;
                bool varNotCollAndFromIsColl = isCollectionInQuery && !isCollectionRetrieveVar;
                bool typeCorrect = isSameType && varNotCollAndFromIsColl;
                if (typeCorrect)
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
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                AllType? RetrieveVar = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isCollectionRetriever, false);
                bool isSameType = collection == RetrieveVar;
                bool varNotCollAndFromIsColl = isCollectionInQuery && !isCollectionRetriever;
                bool typeCorrect = isSameType && varNotCollAndFromIsColl;

                if (typeCorrect)
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
                AllType? collectionNameType;
                AllType? nameDeclaredForRetrieve;
                bool isCollectionInQuery;
                bool isCollectionRetrieveVar;

                collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out isCollectionInQuery, false);
                nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out isCollectionRetrieveVar, false);

                bool isSameType = nameDeclaredForRetrieve.ToString() == collectionNameType.ToString() && nameDeclaredForRetrieve.ToString() == node.Type.ToUpper();
                bool bothIsCollection = isCollectionInQuery && isCollectionRetrieveVar;
                bool typeCorrect = isSameType && bothIsCollection;

                if (typeCorrect)
                {
                    Console.WriteLine("hello");
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
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isCollectionRetriever, false);
                bool isSameTypeAndCollectionCorrect = nameDeclaredForRetrieve.ToString() == collectionNameType.ToString() && nameDeclaredForRetrieve.ToString() == node.Type;
                bool varNotCollAndFromIsColl = isCollectionInQuery && !isCollectionRetriever;
                bool typeCorrect = isSameTypeAndCollectionCorrect && varNotCollAndFromIsColl;

                if (typeCorrect)
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
            AllType? varToAdd;
            AllType? collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);

            if (node.VariableToAdd is ConstantNode constant)
            {
                varToAdd = constant.Type_enum;
                bool sameType = varToAdd == collectionToAddTo;
                bool typeCorrect = sameType && isCollectionInQuery;
                if (typeCorrect)
                {
                    //constant is fine, collection is fine
                }
                Visit(node.VariableToAdd);
            }
            else
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.variableName, out bool isCollectionVarToAdd, false);
                bool sameType = varToAdd == collectionToAddTo;
                bool VarIsNotCollAndToIsColl = !isCollectionVarToAdd && isCollectionInQuery;
                bool typeCorrect = sameType && VarIsNotCollAndToIsColl;
                if (typeCorrect)
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
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isCollectionRetriever, false);
                bool isSameType = collection == nameDeclaredForRetrieve;
                bool varIsNotCollAndFromIsColl = isCollectionInQuery && !isCollectionRetriever;
                bool typeCorrect = isSameType && varIsNotCollAndFromIsColl;
                if (typeCorrect)
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
            AllType? varToAdd;
            AllType? collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);

            if (node.VariableToAdd is ConstantNode constant)
            {
                varToAdd = constant.Type_enum;
                bool sameType = varToAdd == collectionToAddTo;
                bool typeCorrect = sameType && isCollectionInQuery;
                if (typeCorrect)
                {
                    //constant is fine, collection is fine
                }
                Visit(node.VariableToAdd);
            }
            else
            {
                varToAdd = _createdSymbolTabe.RetrieveSymbol(node.variableName, out bool isCollectionVarToAdd, false);
                bool sameType = varToAdd == collectionToAddTo;
                bool VarIsNotCollAndToIsColl = !isCollectionVarToAdd && isCollectionInQuery;
                bool typeCorrect = sameType && VarIsNotCollAndToIsColl;
                if (typeCorrect)
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
                AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                AllType? nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isCollectionRetriever, false);
                bool isSameType = collection == nameDeclaredForRetrieve;
                bool varIsNotCollAndFromIsColl = isCollectionInQuery && !isCollectionRetriever;
                bool typeCorrect = isSameType && varIsNotCollAndFromIsColl;
                if (typeCorrect)
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
            _createdSymbolTabe.SetCurrentNode(node);
            if (node.IsGraph)
            {//control statement for input to graphs
                AllType? TypeOfTargetCollection = _createdSymbolTabe.GetAttributeType(node.ToVariable, AllType.GRAPH, out bool isCollectionTargetColl);
                bool IsGraphVertexCollection = TypeOfTargetCollection == AllType.VERTEX && isCollectionTargetColl;
                bool isGraphEdgeCollection = TypeOfTargetCollection == AllType.EDGE && isCollectionTargetColl;
                if (IsGraphVertexCollection)
                {
                    bool declarationIsVertex;
                    foreach (VariableDclNode vertexdcl in node.Dcls)
                    {
                        declarationIsVertex = vertexdcl.Type_enum == AllType.VERTEX;
                        if (declarationIsVertex)
                        {
                            //both collection is vertex, and current dcl is a vertexdcl
                            Console.WriteLine("Hello :))");
                        }
                        else
                        {
                            //error raised, because af dcl is not of type vertex.
                            _createdSymbolTabe.WrongTypeError(vertexdcl.Name, node.ToVariable);
                        }
                    }
                }
                else if (isGraphEdgeCollection)
                {
                    bool declarationIsEdge;
                    foreach (VariableDclNode edgedcl in node.Dcls)
                    {
                        declarationIsEdge = edgedcl.Type_enum == AllType.VERTEX;
                        if (declarationIsEdge)
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

                    if (isCollectionTargetColl)
                    {

                    }
                    else
                    {
                        //not a collection - type error
                    }

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
            VisitChildrenNewScope(node, node.Name);
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
            node.Assignment.Accept(this);
            VisitChildren(node);
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
            AllType? funcType = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isReturnTypeCollection, false);
            AllType? returnType = _createdSymbolTabe.RetrieveSymbol(node.LeftmostChild.Name, out bool isFunctionTypeCollection, false);

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
            AllType? collectionType = _createdSymbolTabe.RetrieveSymbol(node.InVariableName, out bool isCollectionInForeach, false);

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
