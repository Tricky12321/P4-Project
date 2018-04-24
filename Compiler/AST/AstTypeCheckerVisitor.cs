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

        public override void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                if (child != null)
                {
                    child.Accept(this);
                }
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

        public override void Visit(SetQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? variableType = null;
            AllType? inVariableType;
            string variableName;

            if (node.Attributes != null)
            {
                //set query has attributes which is placed in a list
                foreach (Tuple<VariableAttributeNode, string, ExpressionNode> Attributes in node.Attributes)
                {
                    variableName = Attributes.Item1.Name;
                    variableType = _createdSymbolTabe.RetrieveSymbol(variableName);

                    if (Attributes.Item3 != null)
                    {
                        Attributes.Item3.Accept(this);
                    }

                    if (Attributes.Item3.OverAllType != variableType)
                    {
                        //type error
                        _createdSymbolTabe.WrongTypeError(variableName, Attributes.Item3.Name);
                    }
                }
            }
            /*if (node.InVariable != null)
            {
                inVariableType = _createdSymbolTabe.RetrieveSymbol(node.InVariable.Name);
                if (!(inVariableType == variableType))
                {
                    // type error
                    _createdSymbolTabe.WrongTypeError(node.InVariable.Name, variableType.ToString());
                }
            }
            */
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(PredicateNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
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
            AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            bool fromIsColl = isCollectionInQuery;
            if (fromIsColl)
            {
                if (node.Parent is DeclarationNode dclNode)
                {
                    node.Type = collectionNameType.ToString();
                }
                else if (node.Parent is ExpressionNode expNode)
                {
                    node.Type = collectionNameType.ToString();
                    expNode.QueryName = node.Variable;
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
                node.Type = collection.ToString();
            }
            else
            {
                _createdSymbolTabe.FromVarIsNotCollError(node.Variable);
            }
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
                AllType? typeOfVar;

                foreach (var item in node.TypeOrVariable)
                {
                    item.Accept(this);

                    ExpressionNode expressionToAdd = (ExpressionNode)item;
                    typeOfVar = expressionToAdd.OverAllType;
                    bool targetIsGraph = TypeOfTargetCollection == AllType.GRAPH;

                    if (isCollectionTargetColl)
                    {//non-declarations are added to an extended collection on graph, or simply a collection.
                        if (TypeOfTargetCollection == typeOfVar)
                        {
                            //the expression type is correct corresponding to the type of the target collection.
                        }
                        else
                        {//mismatch of types if the target collection is not of same type of the expression
                            _createdSymbolTabe.WrongTypeError(item.Name, node.ToVariable);
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


            }
            else
            {
                Console.WriteLine("Is neither collection or Graph. This should not be possible");
            }

        }

        public override void Visit(WhereNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
            {
                item.Value.Parent = node;
                item.Value.Accept(this);
                AllType? typeOfKey = _createdSymbolTabe.GetAttributeType(item.Key, AllType.VERTEX);
                if(typeOfKey == node.Type_enum)
                {

                }
                else
                {
                    _createdSymbolTabe.TypeExpressionMismatch();
                }
            }
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? vertexFromType = _createdSymbolTabe.RetrieveSymbol(node.VertexNameFrom, false);
            AllType? vertexToType = _createdSymbolTabe.RetrieveSymbol(node.VertexNameTo, false);
            if(vertexFromType == AllType.VERTEX && vertexToType == AllType.VERTEX)
            {
                //both from and to targets are of type vertex, which they MUST be.
            }
            else
            {
                _createdSymbolTabe.TypeExpressionMismatch();
            }
            foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
            {
                item.Value.Parent = node;
                item.Value.Accept(this);
                AllType? typeOfKey = _createdSymbolTabe.GetAttributeType(item.Key, AllType.EDGE);
                if (typeOfKey == node.Type_enum)
                {

                }
                else
                {
                    _createdSymbolTabe.TypeExpressionMismatch();
                }
            }
        }

        public override void Visit(GraphNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
            foreach (AbstractNode item in node.Edges)
            {
                item.Accept(this);
            }
            foreach (AbstractNode item in node.Vertices)
            {
                item.Accept(this);
            }
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
        {
            _createdSymbolTabe.SetCurrentNode(node);
            string targetName = node.Attributes.Item1.Name.Trim('\'');
            AllType? targetType = _createdSymbolTabe.GetAttributeType(targetName, AllType.GRAPH);
            node.Attributes.Item3.Accept(this);
            AllType? assignedType = node.Attributes.Item3.OverAllType;

            if (targetType == assignedType)
            {
                //both the attribute type and the assigned value are of the same type.
            }
            else
            {
                _createdSymbolTabe.WrongTypeError(node.Attributes.Item1.Name, node.Attributes.Item3.Name);
            }
        }

        public override void Visit(DeclarationNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            if (node.Assignment != null)
            {
                node.Assignment.Parent = node;
                node.Assignment.Accept(this);
            }
            VisitChildren(node);
            AllType? typeOfVariable = _createdSymbolTabe.RetrieveSymbol(node.Name, out bool isCollection, false);
            if (node.Assignment is ExpressionNode exprNode)
            {
                if (typeOfVariable == exprNode.OverAllType)
                {
                    //the expression type and the variable is of same time.
                }
                else
                {
                    _createdSymbolTabe.TypeExpressionMismatch();
                }
            }
            else if (node.Assignment is SelectAllQueryNode selAll)
            {
                if (typeOfVariable == selAll.Type_enum && isCollection)
                {
                    //type correct, variable is a coll, and collections have the same time. inner collection is checked in selectallNode.
                }
                else
                {
                    if (!isCollection)
                    {
                        _createdSymbolTabe.TargetIsNotCollError(node.Name);
                    }
                    else
                    {
                        Console.WriteLine("select all, but something went wrong.");
                    }
                }
            }
            else
            {
                Console.WriteLine("this is something else than expression or selectall, in declaration node." + _createdSymbolTabe.CurrentLine);
            }
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

                if (node.HasChildren)
                {
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
                node.OverAllType = AllType.STRING;
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
                            if(node.Parent is GraphDeclVertexNode vDcl)
                            {
                                vDcl.Type = node.OverAllType.ToString();
                                
                            }
                            else if(node.Parent is GraphDeclEdgeNode eDcl)
                            {

                            }
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
            VisitChildren(node);
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? funcType = _createdSymbolTabe.RetrieveSymbol(node.FuncName, out bool FuncTypeCollection, false);
            AllType? returnType = null;
            bool ReturnTypeCollection = false;

            if(node.LeftmostChild is ExpressionNode expNode && expNode.ExpressionParts != null)
            {
                _createdSymbolTabe.RetrieveSymbol(expNode.QueryName, out bool returnTypeCollection, false);
                ReturnTypeCollection = returnTypeCollection;
                returnType = expNode.OverAllType;
            }
            else if (funcType == AllType.VOID)
            {
                _createdSymbolTabe.FunctionIsVoidError(node.FuncName);
            }
            else if (ReturnTypeCollection && FuncTypeCollection)
            {
                if (!(funcType == returnType))
                {
                    //ERROR, conflicting function and return type
                    _createdSymbolTabe.WrongTypeError(node.FuncName, node.Name);
                }
            }
            else
            {
                //ERROR, one is collection, other isn't
                _createdSymbolTabe.WrongTypeErrorCollection(node.FuncName, node.Name);
            }
        }

        public override void Visit(ForLoopNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.OpenScope(BlockType.ForLoop);
            node.Increment.Accept(this);
            node.VariableDeclaration.Accept(this);
            node.ToValueOperation.Accept(this);
            AllType? varDclNodeType;

            if (node.VariableDeclaration is VariableDclNode varDclNode && node.Increment is ExpressionNode incrementNode)
            {
                varDclNodeType = _createdSymbolTabe.RetrieveSymbol(varDclNode.Name);
                if (!(varDclNodeType == AllType.INT && incrementNode.OverAllType == AllType.INT))
                {
                    _createdSymbolTabe.WrongTypeConditionError();
                }
            }
            VisitChildren(node);
            _createdSymbolTabe.CloseScope();
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
            VisitChildren(node);
        }

        public override void Visit(VariableNode node)
        {
            AllType? variableType;

            if (node.Assignment != null)
            {
                AllType? variableExpressionType = _createdSymbolTabe.RetrieveSymbol(node.Assignment.Name);
            }

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
            AllType? variableType = _createdSymbolTabe.RetrieveSymbol(node.Name);
            AllType? ok = node.Type_enum;

            if (node.Children != null)
            {
                VisitChildren(node);
                foreach (AbstractNode child in node.Children)
                {
                    if (child is ExpressionNode expNode)
                    {
                        expNode.Accept(this);
                        if (expNode.OverAllType != variableType)
                        {
                            _createdSymbolTabe.WrongTypeError(child.Name, node.Name);
                        }
                    }
                }
            }
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
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);

        }

        public override void Visit(PredicateCall node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            _createdSymbolTabe.NotImplementedError(node);
        }
    }
}
