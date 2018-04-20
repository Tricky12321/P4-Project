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
                        if (variableType == AllType.BOOL && (Attributes.Item3.ExpressionParts.Count > 1 || Attributes.Item3.ExpressionParts.Count == 0))
                        {
                            _createdSymbolTabe.BoolAdditionError(Attributes.Item1.Name);
                            //cant add bools
                            break;
                        }

                        //no type check om operator item
                        if (!(ExpressionPart is OperatorNode))
                        {

                            if (ExpressionPart is ConstantNode constant)
                            {
                                if (!(constant.Type_enum == variableType))
                                {
                                    //Type error
                                    _createdSymbolTabe.WrongTypeError(constant.Name, Attributes.Item1.Name);
                                }
                            }

                            //can be variable og attribute, therefore using VariableAttributeNode
                            else if (ExpressionPart is VariableAttributeNode expressionVariable)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(expressionVariable.Name);

                                if (!(expressionType == variableType))
                                {
                                    _createdSymbolTabe.WrongTypeError(expressionVariable.Name, Attributes.Item1.Name);
                                    // type error
                                }
                            }


                            else if (ExpressionPart is SelectQueryNode selectQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(selectQuery.Variable);

                                if (!(expressionType == variableType))
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(selectQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(selectQuery);
                            }

                            else if (ExpressionPart is SelectAllQueryNode selectAllQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(selectAllQuery.Variable);

                                if (!(expressionType == variableType))
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(selectAllQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(selectAllQuery);
                            }

                            else if (ExpressionPart is DequeueQueryNode dequeueQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(dequeueQuery.Variable);
                                if (!(expressionType == variableType))
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(dequeueQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(dequeueQuery);
                            }

                            else if (ExpressionPart is PopQueryNode popQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(popQuery.Variable);
                                if (!(expressionType == variableType))
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(popQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(popQuery);
                            }

                            else if (ExpressionPart is ExtractMinQueryNode extractMinQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(extractMinQuery.Variable);
                                if (expressionType == variableType)
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(extractMinQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(extractMinQuery);
                            }

                            else if (ExpressionPart is ExtractMaxQueryNode extraxtMaxQuery)
                            {
                                expressionType = _createdSymbolTabe.RetrieveSymbol(extraxtMaxQuery.Variable);
                                if (!(expressionType == variableType))
                                {
                                    // type error
                                    _createdSymbolTabe.WrongTypeError(extraxtMaxQuery.Name, Attributes.Item1.Name);
                                }
                                VisitChildren(extraxtMaxQuery);
                            }


                        }
                    }


                    if (node.InVariable != null)
                    {
                        inVariableType = _createdSymbolTabe.RetrieveSymbol(node.InVariable.Name);
                        if (!(inVariableType == variableType))
                        {
                            // type error
                            _createdSymbolTabe.WrongTypeError(node.InVariable.Name, Attributes.Item1.Name);
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
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                    }
                }
                else
                {
                    //correct error message pls
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
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                    }
                }
                else
                {
                    //TODO correct error pls
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

                collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                nameDeclaredForRetrieve = _createdSymbolTabe.RetrieveSymbol(node.Parent.Name, out bool isCollectionRetrieveVar, false);

                bool isSameType = nameDeclaredForRetrieve.ToString() == collectionNameType.ToString();
                bool bothIsCollection = isCollectionInQuery && isCollectionRetrieveVar;
                bool typeCorrect = isSameType && bothIsCollection;

                if (typeCorrect)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                    }
                }
                else
                {
                    //TODO correct error message pls
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
            if (node.Parent != null)
            {
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                    }
                }
                else
                {
                    //TODO correct error message pls
                }

                if (node.WhereCondition != null)
                {
                    Visit(node.WhereCondition);
                }
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
            if (node.Parent != null)
            {
                AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                    }
                }
                else
                {
                    //TODO find ordenlig error.
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
            AllType? collection = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            bool FromIsColl = isCollectionInQuery;
            if (FromIsColl)
            {
                if (node.Parent is ExpressionNode expNode)
                {
                    expNode.OverAllType = collection;
                }
            }
            else
            {
                _createdSymbolTabe.WrongTypeError(node.Variable, node.Parent.Name);
            }
        }

        private string DeclarationSetPrint(AddQueryNode node)
        {
            StringBuilder dclList = new StringBuilder();
            dclList.Append($"declaration_set(");
            foreach (AbstractNode v in node.Dcls)
            {
                dclList.Append($"{v.Name}, ");
            }
            dclList.Remove(dclList.Length - 2, 2);
            dclList.Append(")");
            return dclList.ToString();
        }

        public override void Visit(AddQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            if (node.IsGraph)
            {//control statement for input to graphs
                AllType? TypeOfTargetCollection = _createdSymbolTabe.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
                bool IsGraphVertexCollection = TypeOfTargetCollection == AllType.VERTEX && isCollectionTargetColl;
                bool isGraphEdgeCollection = TypeOfTargetCollection == AllType.EDGE && isCollectionTargetColl;
                bool isPreDefVerOrEdgeCollInGraph = TypeOfTargetCollection == AllType.GRAPH;
                if (isPreDefVerOrEdgeCollInGraph)
                {//if declarations are added to the graph.
                    foreach (AbstractNode edgeOrVertexdcl in node.Dcls)
                    {
                        if (edgeOrVertexdcl is GraphDeclVertexNode || edgeOrVertexdcl is GraphDeclEdgeNode)
                        {
                            //vertex or edge is added to the graph
                        }
                        else
                        {
                            //error raised, because af dcl is not of type edge or vertex.
                            _createdSymbolTabe.WrongTypeError(edgeOrVertexdcl.Name, node.ToVariable);
                        }
                    }
                }
                else if (IsGraphVertexCollection || isGraphEdgeCollection)
                {//if declarations is added to an extended collection on graph - NOT LEGAL
                    foreach (AbstractNode vertexOrEdgedcl in node.Dcls)
                    {
                        if (vertexOrEdgedcl is GraphDeclVertexNode || vertexOrEdgedcl is GraphDeclEdgeNode)
                        {
                            _createdSymbolTabe.WrongTypeError(DeclarationSetPrint(node), node.ToVariable);
                            break;
                        }
                    }
                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(DeclarationSetPrint(node), node.ToVariable);
                }
            }
            //if the ToVariable is a collection:
            else if (node.IsColl)
            {
                AllType? TypeOfTargetCollection = _createdSymbolTabe.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
                node.TypeOrVariable.Accept(this);
                ExpressionNode expressionToAdd = (ExpressionNode)node.TypeOrVariable;
                AllType? typeOfVar = expressionToAdd.OverAllType;
                bool targetIsGraph = TypeOfTargetCollection == AllType.GRAPH;

                if (isCollectionTargetColl)
                {//non-declarations are added to an extended collection on graph, or simply a collection.
                    if (TypeOfTargetCollection == typeOfVar)
                    {
                        //the expression type is correct corresponding to the type of the target collection.
                    }
                    else
                    {//mismatch of types if the target collection is not of same type of the expression
                        _createdSymbolTabe.WrongTypeError(node.TypeOrVariable.Name, node.ToVariable);
                    }
                }
                else if (targetIsGraph)
                {//if variables are added to the graph.
                    bool varIsVertex = typeOfVar == AllType.VERTEX;
                    bool varIsEdge = typeOfVar == AllType.EDGE;
                    if (varIsEdge || varIsVertex)
                    {
                        //only edge and vertex variables can be added to a graph.
                    }
                    else
                    {
                        _createdSymbolTabe.WrongTypeError(node.TypeOrVariable.ToString(), node.ToVariable);
                    }
                }
                else
                {
                    _createdSymbolTabe.TargetIsNotCollError(node.ToVariable);
                }
            }
            else
            {
                Console.WriteLine("Is neither collection or Graph. This should not be possible");
            }

        }

        public override void Visit(WhereNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            node.Accept(this);
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(GraphNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(FunctionNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildrenNewScope(node, node.Name);
        }

        public override void Visit(AbstractNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(IfElseIfElseNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            // If conditions
            node.IfCondition.Accept(this);
            // If codeblock
            foreach (var item in node.IfCodeBlock.Children)
            {
                item.Accept(this);
            }
            // Elseif statements
            foreach (var item in node.ElseIfList)
            {
                item.Item1.Accept(this);
                foreach (var child in item.Item2.Children)
                {
                    child.Accept(this);
                }
            }
            // Else statement
            if (node.ElseCodeBlock != null)
            {
                foreach (var child in node.ElseCodeBlock.Children)
                {
                    child.Accept(this);
                }
            }
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
            _createdSymbolTabe.SetCurrentNode(node);
            if (node.Assignment != null)
            {
                node.Assignment.Accept(this);
            }
            VisitChildren(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(BoolComparisonNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            AllType type_check;
            bool compare = false;
            if (node.Left != null && node.Right != null)
            {

                // Check if the nodes are boolcomparisons
                if (node.Left is BoolComparisonNode && node.Right is BoolComparisonNode)
                {
                    node.Left.Accept(this);
                    node.Right.Accept(this);
                    compare = true;
                }

                // Extract the type from Left and right sides of a bool comparison
                if (node.Left.Children[0] is ExpressionNode)
                {
                    node.LeftType = ((ExpressionNode)node.Left.Children[0]).OverAllType ?? default(AllType);
                }
                else if (node.Left.Children[0] is PredicateNode)
                {
                    node.LeftType = _createdSymbolTabe.RetrieveSymbol(node.Left.Children[0].Name) ?? default(AllType);
                }

                if (node.Right.Children[0] is ExpressionNode)
                {
                    node.RightType = ((ExpressionNode)node.Right.Children[0]).OverAllType ?? default(AllType);
                }
                else if (node.Right.Children[0] is PredicateNode)
                {
                    node.RightType = _createdSymbolTabe.RetrieveSymbol(node.Right.Children[0].Name) ?? default(AllType);
                }

                if (node.RightType != AllType.UNKNOWNTYPE && node.LeftType != AllType.UNKNOWNTYPE)
                {
                    if (node.RightType != node.LeftType)
                    {
                        if (!((node.RightType == AllType.INT && node.LeftType == AllType.DECIMAL) || (node.RightType == AllType.DECIMAL && node.LeftType == AllType.INT)))
                        {
                            _createdSymbolTabe.WrongTypeConditionError();
                        }
                    }
                }
            }
            else
            {
                VisitChildren(node);
            }
        }

        public override void Visit(ExpressionNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? previousType = null;
            if (node.ExpressionParts.Where(x => x.Type != null && x.Type_enum == AllType.STRING).Count() > 0)
            {
                //ignore the rest of the type checking of expression. An expressionpart is string, therefore everything will turn to a string
            }
            else
            {
                foreach (AbstractNode item in node.ExpressionParts)
                {
                    if (!(item is OperatorNode))
                    {
                        item.Parent = node;
                        item.Accept(this);

                        if (previousType == null)
                        {//first call - setting previous type as the first type encountered
                            previousType = node.OverAllType;
                        }
                        else
                        {
                            if (previousType != node.OverAllType)
                            {//types are different from eachother
                                if ((previousType == AllType.INT && node.OverAllType == AllType.DECIMAL) || (previousType == AllType.DECIMAL && node.OverAllType == AllType.INT))
                                {//types are accepted if one is int and one is decimal
                                    node.OverAllType = AllType.DECIMAL;
                                    //do nothing, but set overalltype to decimal.
                                }
                                else
                                {//types are different from eachother, and do not allow operates between them
                                    _createdSymbolTabe.TypeExpressionMismatch();
                                }
                            }
                            else
                            {//times are of the same time
                                //bools to control which types are not allowed to be operated upon, even if same time.
                                bool bothIsBool = previousType == AllType.BOOL && node.OverAllType == AllType.BOOL;
                                bool bothIsGraph = previousType == AllType.GRAPH && node.OverAllType == AllType.GRAPH;
                                bool bothIsVertex = previousType == AllType.VERTEX && node.OverAllType == AllType.VERTEX;
                                bool bothIsEdge = previousType == AllType.EDGE && node.OverAllType == AllType.EDGE;
                                bool bothIsVoid = previousType == AllType.VOID && node.OverAllType == AllType.VOID;
                                bool bothIsCollection = previousType == AllType.COLLECTION && node.OverAllType == AllType.COLLECTION;

                                if (bothIsBool || bothIsGraph || bothIsVertex || bothIsEdge || bothIsVoid || bothIsCollection)
                                {
                                    _createdSymbolTabe.TypeExpressionMismatch();
                                }
                                else
                                {
                                    //do nothing, both is the same type and are allowed, so everything is fine.
                                }

                            }
                        }
                    }
                }
            }
            if (node.OverAllType == AllType.UNKNOWNTYPE)
            {
                _createdSymbolTabe.TypeExpressionMismatch();
            }
        }

        public override void Visit(ReturnNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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

            _createdSymbolTabe.SetCurrentNode(node);
            //fejl i parser, forloopnode gemmer ikke variablen som er udgangspunkt for loop, hvis den allerede er deklareret
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ForeachLoopNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
            _createdSymbolTabe.SetCurrentNode(node);
            node.BoolCompare.Accept(this);
            VisitChildrenNewScope(node, BlockType.WhileLoop);
        }

        public override void Visit(VariableAttributeNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(VariableNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            ExpressionNode parentNode = (ExpressionNode)node.Parent;
            parentNode.OverAllType = _createdSymbolTabe.RetrieveSymbol(node.Name);
            VisitChildren(node);
        }

        public override void Visit(CodeBlockNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(VariableDclNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(OperatorNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(ConstantNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            ExpressionNode parentNode = (ExpressionNode)node.Parent;
            parentNode.OverAllType = node.Type_enum;
        }

        public override void Visit(PrintQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }

        public override void Visit(RunQueryNode node)
        {
        }
    }
}
