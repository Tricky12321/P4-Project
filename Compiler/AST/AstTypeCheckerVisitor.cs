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
        SymTable _symbolTable;

        public AstTypeCheckerVisitor(SymTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        public void VisitChildrenNewScope(AbstractNode node, string Name)
        {
            if (node != null)
            {
                _symbolTable.OpenScope(Name);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }
                _symbolTable.CloseScope();
            }
        }

        public void VisitChildrenNewScope(AbstractNode node, BlockType Type)
        {
            if (node != null)
            {
                _symbolTable.OpenScope(Type);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }
                _symbolTable.CloseScope();
            }
        }

        private string DeclarationSetPrint(AddQueryNode node)
        {
            StringBuilder dclList = new StringBuilder();
            dclList.Append($"declaration set:(");
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
                child.Parent = node;
                if (child != null)
                {
                    child.Accept(this);
                }
            }
        }

        private void checkCollectionFollowsCollection(string varName)
        {
            if (varName.Contains('.'))
            {
                string[] names = varName.Split('.');
                string lastString = "";
                for (int i = 0; i < names.Length - 1; i++)
                {
                    string currentCheck = lastString + names[i];
                    lastString = currentCheck + ".";
                    _symbolTable.RetrieveSymbol(currentCheck, out bool isCollection, false);
                    {
                        if (isCollection)
                        {
                            _symbolTable.IllegalCollectionPath(varName);
                        }
                    }
                }
            }
        }


        //-----------------------------Visitor----------------------------------------------
        public override void Visit(ParameterNode node)
        {
            _symbolTable.SetCurrentNode(node);

            if (node.Type_enum == AllType.VOID)
            {
                _symbolTable.ParamerIsVoid(node.Parent.Name, node.Name);
            }
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(SetQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            AllType? variableType = null;
            AllType? inVariableType = null;
            bool isAttributeCollection = false;
            bool isTypeCollection = false;
            string variableName;

            if (node.Attributes != null)
            {
                //set query has attributes which is placed in a list
                foreach (Tuple<VariableAttributeNode, string, AbstractNode> Attributes in node.Attributes)
                {
                    variableName = Attributes.Item1.Name;

                    if (Attributes.Item1 is AttributeNode attNode)
                    {
                        //skal finde ud af hvad der er extended.

                        if (node.InVariable != null)
                        {
                            checkCollectionFollowsCollection(node.InVariable.Name);
                            inVariableType = _symbolTable.RetrieveSymbol(node.InVariable.Name);
                            variableType = _symbolTable.GetAttributeType(attNode.Name, inVariableType ?? default(AllType), out isAttributeCollection);
                        }
                    }
                    else
                    {
                        variableType = _symbolTable.RetrieveSymbol(variableName, out isTypeCollection);
                    }

                    if (Attributes.Item3 != null)
                    {
                        Attributes.Item3.Accept(this);
                    }
                    //skriv en fejlbesked
                    if (variableType != null)
                    {
                        if (isTypeCollection == isAttributeCollection)
                        {
                            if (Attributes.Item3 is BoolComparisonNode && (Attributes.Item3 as BoolComparisonNode).Children[0] is ExpressionNode)
                                if ((Attributes.Item3.Children[0] as ExpressionNode).OverAllType != variableType)
                                {
                                    //type between varible and overalltyper
                                    _symbolTable.TypeExpressionMismatch();
                                }
                        }
                        else
                        {
                            _symbolTable.WrongTypeConditionError();
                        }
                    }
                }
            }
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(PredicateNode node)
        {
            _symbolTable.SetCurrentNode(node);
            _symbolTable.OpenScope(node.Name);

            VisitChildren(node);

            _symbolTable.CloseScope();
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            AllType? typeAttribute = AllType.UNKNOWNTYPE;
            if (isCollectionInQuery)
            {
                if (node.Attribute != null)
                {
                    if (collectionNameType != null && collectionNameType != AllType.INT && collectionNameType != AllType.DECIMAL)
                    {
                        if (_symbolTable.IsExtended(node.Attribute, collectionNameType ?? default(AllType)))
                        {
                            typeAttribute = _symbolTable.GetAttributeType(node.Attribute, collectionNameType ?? default(AllType));
                            if (typeAttribute == AllType.DECIMAL || typeAttribute == AllType.INT)
                            {
                                //variable is a collection, which are different from decimal and int collections, 
                                //an attribute is specified, and are of type int or decimal, which ir MUST be!
                                node.Type = collectionNameType.ToString();
                                node.Name = node.Variable;
                            }
                            else
                            {
                                //attribute is other than int or decimal, which it may not be.
                                _symbolTable.AttributeIllegal();
                            }
                        }
                        else
                        {
                            //the class is not extended with given attribute
                            _symbolTable.AttributeNotExtendedOnClass(node.Attribute, collectionNameType);
                        }
                    }
                    else
                    {
                        //the collection type is either int, decimal or null. which are not legal.
                        _symbolTable.ExtractCollNotIntOrDeciError();
                        //SKAL FINDE UD AF OM DEN ERROR I SYMTABLE ER KORREKT AT BRUGE ET ELLER ANDET STED HER
                    }
                }
                else
                {
                    if (collectionNameType == AllType.DECIMAL || collectionNameType == AllType.INT)
                    {
                        //the attribute is proveded, there the collection must be of type decimal or interger.
                        //Because it will sort on the values of the items in the decimal or integer collections,
                        //and not on some attribute extended on the classes.
                        node.Type = collectionNameType.ToString();
                    }
                    else
                    {
                        _symbolTable.NoAttriProvidedCollNeedsToBeIntOrDecimalError();
                    }
                    //attribute not specified - coll needs to be int or deci
                }
            }
            else
            {
                //the from variable needs to be a collection. You cannot retrieve something from a variable - only retrieve from a collections.
                _symbolTable.FromVarIsNotCollError(node.Variable);
            }

            if (isCollectionInQuery)
            {
                if (node.Parent is ExpressionNode expNode)
                {
                    expNode.OverAllType = collectionNameType;
                }
                node.Type = collectionNameType.ToString();
            }

            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            AllType? typeAttribute = AllType.UNKNOWNTYPE;
            if (isCollectionInQuery)
            {
                if (node.Attribute != null)
                {
                    if (collectionNameType != null && collectionNameType != AllType.INT && collectionNameType != AllType.DECIMAL)
                    {
                        if (_symbolTable.IsExtended(node.Attribute, collectionNameType ?? default(AllType)))
                        {
                            typeAttribute = _symbolTable.GetAttributeType(node.Attribute, collectionNameType ?? default(AllType));
                            if (typeAttribute == AllType.DECIMAL || typeAttribute == AllType.INT)
                            {
                                //variable is a collection, which are different from decimal and int collections, 
                                //an attribute is specified, and are of type int or decimal, which ir MUST be!
                                node.Type = collectionNameType.ToString();
                                node.Name = node.Variable;
                            }
                            else
                            {
                                //attribute is other than int or decimal, which it may not be.
                                _symbolTable.AttributeIllegal();
                            }
                        }
                        else
                        {
                            //the class is not extended with given attribute
                            _symbolTable.AttributeNotExtendedOnClass(node.Attribute, collectionNameType);
                        }
                    }
                    else
                    {
                        //the collection type is either int, decimal or null. which are not legal.
                        _symbolTable.ExtractCollNotIntOrDeciError();
                        //SKAL FINDE UD AF OM DEN ERROR I SYMTABLE ER KORREKT AT BRUGE ET ELLER ANDET STED HER
                    }
                }
                else
                {
                    if (collectionNameType == AllType.DECIMAL || collectionNameType == AllType.INT)
                    {
                        //the attribute is proveded, there the collection must be of type decimal or interger.
                        //Because it will sort on the values of the items in the decimal or integer collections,
                        //and not on some attribute extended on the classes.
                        node.Type = collectionNameType.ToString();
                    }
                    else
                    {
                        _symbolTable.NoAttriProvidedCollNeedsToBeIntOrDecimalError();
                    }
                    //attribute not specified - coll needs to be int or deci
                }
            }
            else
            {
                //the from variable needs to be a collection. You cannot retrieve something from a variable - only retrieve from a collections.
                _symbolTable.FromVarIsNotCollError(node.Variable);
            }

            if (isCollectionInQuery)
            {
                if (node.Parent is ExpressionNode expNode)
                {
                    expNode.OverAllType = collectionNameType;
                }
                node.Type = collectionNameType.ToString();
            }

            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            bool fromIsColl = isCollectionInQuery;
            if (fromIsColl)
            {
                if (node.Parent is DeclarationNode dclNode)
                {
                    node.Type = collectionNameType.ToString();
                    node.Name = node.Variable;
                }
                else if (node.Parent is ExpressionNode expNode)
                {
                    node.Type = collectionNameType.ToString();
                    expNode.QueryName = node.Variable;
                    expNode.OverAllType = collectionNameType;
                    expNode.Name = node.Variable;
                }
            }
            else
            {
                _symbolTable.FromVarIsNotCollError(node.Variable);
            }
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(SelectQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            if (node.Parent != null)
            {
                AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                        expNode.Name = node.Variable;
                    }
                    node.Type = collectionNameType.ToString();
                    node.Name = node.Variable;
                }
                else
                {
                    _symbolTable.FromVarIsNotCollError(node.Variable);
                }

                if (node.WhereCondition != null)
                {
                    node.WhereCondition.Accept(this);
                }
            }
        }

        public override void Visit(PushQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.VariableCollection);
            AllType? varToAdd;
            AllType? collectionToAddTo = _symbolTable.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);
            node.Type = collectionToAddTo.ToString();
            ExpressionNode expNode = (ExpressionNode)node.VariableToAdd.Children[0];
            foreach (AbstractNode abnode in expNode.ExpressionParts)
            {
                if (!(abnode is OperatorNode))
                {
                    if (abnode is ConstantNode constant)
                    {
                        varToAdd = constant.Type_enum;
                        constant.Parent = expNode;
                        bool sameType = varToAdd == collectionToAddTo;
                        bool typeCorrect = sameType && isCollectionInQuery;
                        if (typeCorrect)
                        {
                            //constant is fine, collection is fine
                        }
                        else
                        {
                            _symbolTable.WrongTypeError(node.variableName, node.VariableCollection);
                        }
                        abnode.Accept(this);
                    }
                    else
                    {
                        varToAdd = _symbolTable.RetrieveSymbol(abnode.Name, out bool isCollectionVarToAdd, false);
                        bool sameType = varToAdd == collectionToAddTo;
                        bool VarIsNotCollAndToIsColl = !isCollectionVarToAdd && isCollectionInQuery;
                        bool typeCorrect = sameType && VarIsNotCollAndToIsColl;
                        if (typeCorrect)
                        {
                            //varie is fine, collection is fine
                        }
                        else
                        {
                            _symbolTable.WrongTypeError(node.variableName, node.VariableCollection);
                        }
                        abnode.Accept(this);
                    }
                }
            }
        }

        public override void Visit(PopQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            if (node.Parent != null)
            {
                AllType? collectionNameType = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
                bool FromIsColl = isCollectionInQuery;
                if (FromIsColl)
                {
                    if (node.Parent is ExpressionNode expNode)
                    {
                        expNode.OverAllType = collectionNameType;
                        expNode.Name = node.Variable;
                    }
                    node.Type = collectionNameType.ToString();
                    node.Name = node.Variable;
                }
                else
                {
                    _symbolTable.FromVarIsNotCollError(node.Variable);
                }
            }
        }

        public override void Visit(EnqueueQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.VariableCollection);
            AllType? varToAdd;
            AllType? collectionToAddTo = _symbolTable.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);
            node.Type = collectionToAddTo.ToString();
            ExpressionNode expNode = (ExpressionNode)node.VariableToAdd.Children[0];
            foreach (AbstractNode abnode in expNode.ExpressionParts)
            {
                if (!(abnode is OperatorNode))
                {
                    if (abnode is ConstantNode constant)
                    {
                        varToAdd = constant.Type_enum;
                        constant.Parent = expNode;
                        bool sameType = varToAdd == collectionToAddTo;
                        bool typeCorrect = sameType && isCollectionInQuery;
                        if (typeCorrect)
                        {
                            //constant is fine, collection is fine
                        }
                        else
                        {
                            _symbolTable.WrongTypeError(node.variableName, node.VariableCollection);
                        }
                        abnode.Accept(this);
                    }
                    else
                    {
                        varToAdd = _symbolTable.RetrieveSymbol(abnode.Name, out bool isCollectionVarToAdd, false);
                        bool sameType = varToAdd == collectionToAddTo;
                        bool VarIsNotCollAndToIsColl = !isCollectionVarToAdd && isCollectionInQuery;
                        bool typeCorrect = sameType && VarIsNotCollAndToIsColl;
                        if (typeCorrect)
                        {
                            //varie is fine, collection is fine
                        }
                        else
                        {
                            _symbolTable.WrongTypeError(node.variableName, node.VariableCollection);
                        }
                        abnode.Accept(this);
                    }
                }
            }
        }

        public override void Visit(DequeueQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.Variable);
            AllType? collection = _symbolTable.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            bool FromIsColl = isCollectionInQuery;
            checkCollectionFollowsCollection(node.Variable);
            if (FromIsColl)
            {
                if (node.Parent is ExpressionNode expNode)
                {
                    expNode.OverAllType = collection;
                    expNode.Name = node.Variable;
                }
                node.Type = collection.ToString();
                node.Name = node.Variable;
            }
            else
            {
                _symbolTable.FromVarIsNotCollError(node.Variable);
            }
        }

        public override void Visit(AddQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            checkCollectionFollowsCollection(node.ToVariable);
            if (node.IsGraph)
            {//control statement for input to graphs
                AllType? TypeOfTargetCollection = _symbolTable.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
                node.Type = TypeOfTargetCollection.ToString();
                bool IsGraphVertexCollection = TypeOfTargetCollection == AllType.VERTEX && isCollectionTargetColl;
                bool isGraphEdgeCollection = TypeOfTargetCollection == AllType.EDGE && isCollectionTargetColl;
                bool isPreDefVerOrEdgeCollInGraph = TypeOfTargetCollection == AllType.GRAPH;
                if (isPreDefVerOrEdgeCollInGraph)
                {//if declarations are added to the graph.
                    foreach (AbstractNode edgeOrVertexdcl in node.Dcls)
                    {
                        edgeOrVertexdcl.Accept(this);
                    }
                }
                else if (IsGraphVertexCollection || isGraphEdgeCollection)
                {//if declarations is added to an extended collection on graph - NOT LEGAL
                    foreach (AbstractNode vertexOrEdgedcl in node.Dcls)
                    {
                        if (vertexOrEdgedcl is GraphDeclVertexNode || vertexOrEdgedcl is GraphDeclEdgeNode)
                        {
                            _symbolTable.DeclarationsCantBeAdded(DeclarationSetPrint(node), node.ToVariable);
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("should not be possible to reach this. IsGraph bool can only be true if its declarations for a graph"
                        + "or vertices/edges added to the graphs original attributes:   .Vertices and .Edges");
                }
            }
            //if the ToVariable is a collection:
            else if (node.IsColl)
            {
                AllType? TypeOfTargetCollection = _symbolTable.RetrieveSymbol(node.ToVariable, out bool isCollectionTargetColl, false);
                node.Type = TypeOfTargetCollection.ToString();
                AllType? typeOfVar = null;

                foreach (var item in node.TypeOrVariable)
                {
                    item.Accept(this);

                    AbstractNode expressionToAdd = item;

                    if (expressionToAdd.Children[0] is ExpressionNode)
                    {
                        typeOfVar = (expressionToAdd.Children[0] as ExpressionNode).OverAllType;
                    }
                    else
                    {
                        typeOfVar = expressionToAdd.Type_enum;
                    }
                    bool targetIsGraph = TypeOfTargetCollection == AllType.GRAPH;

                    if (isCollectionTargetColl)
                    {//non-declarations are added to an extended collection on graph, or simply a collection.
                        if (TypeOfTargetCollection == typeOfVar)
                        {
                            //the expression type is correct corresponding to the type of the target collection.
                        }
                        else
                        {//mismatch of types if the target collection is not of same type of the expression
                            _symbolTable.WrongTypeError(item.Name, node.ToVariable);
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
                            _symbolTable.WrongTypeError(node.TypeOrVariable.ToString(), node.ToVariable);
                        }
                    }
                    else
                    {
                        _symbolTable.TargetIsNotCollError(node.ToVariable);
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
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            _symbolTable.SetCurrentNode(node);
            foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
            {
                item.Value.Parent = node;
                item.Value.Accept(this);
                AllType? typeOfKey = _symbolTable.GetAttributeType(item.Key, AllType.VERTEX);
                ExpressionNode expNode = (ExpressionNode)item.Value.Children[0];
                if (typeOfKey == expNode.OverAllType)
                {

                }
                else
                {
                    _symbolTable.TypeExpressionMismatch();
                }
            }
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            _symbolTable.SetCurrentNode(node);
            AllType? vertexFromType = _symbolTable.RetrieveSymbol(node.VertexNameFrom, false);
            AllType? vertexToType = _symbolTable.RetrieveSymbol(node.VertexNameTo, false);
            if (!(vertexFromType == AllType.VERTEX && vertexToType == AllType.VERTEX))
            {
                //type error
                _symbolTable.TypeExpressionMismatch();
            }
            foreach (KeyValuePair<string, AbstractNode> item in node.ValueList)
            {
                item.Value.Parent = node;
                item.Value.Accept(this);
                AllType? typeOfKey = _symbolTable.GetAttributeType(item.Key, AllType.EDGE);
                if (item.Value.Children[0] is ExpressionNode expNode)
                {
                    if (typeOfKey != expNode.OverAllType)
                    {
                        _symbolTable.TypeExpressionMismatch();
                    }
                }
                else
                {
                    //throw new Exception("haha gg");
                }

            }
        }

        public override void Visit(GraphNode node)
        {
            _symbolTable.SetCurrentNode(node);
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
            _symbolTable.SetCurrentNode(node);
            foreach (AbstractNode item in node.Parameters)
            {
                item.Accept(this);
            }
            VisitChildrenNewScope(node, node.Name);
        }

        public override void Visit(AbstractNode node)
        {
            _symbolTable.SetCurrentNode(node);
            _symbolTable.NotImplementedError(node);
        }

        public override void Visit(IfElseIfElseNode node)
        {
            _symbolTable.SetCurrentNode(node);
            // If conditions
            node.IfCondition.Accept(this);
            // If codeblock
            _symbolTable.OpenScope(BlockType.IfStatement);
            foreach (var item in node.IfCodeBlock.Children)
            {
                item.Accept(this);
            }
            _symbolTable.CloseScope();
            // Elseif statements
            foreach (var item in node.ElseIfList)
            {
                item.Item1.Accept(this);
                _symbolTable.OpenScope(BlockType.ElseifStatement);
                foreach (var child in item.Item2.Children)
                {
                    child.Accept(this);
                }
                _symbolTable.CloseScope();
            }
            // Else statement
            _symbolTable.OpenScope(BlockType.ElseStatement);
            if (node.ElseCodeBlock != null)
            {
                foreach (var child in node.ElseCodeBlock.Children)
                {
                    child.Accept(this);
                }
            }
            _symbolTable.CloseScope();
        }

        public override void Visit(GraphSetQuery node)
        {
            _symbolTable.SetCurrentNode(node);
            string targetName = node.Attributes.Item1.Name;
            AllType? targetType = _symbolTable.GetAttributeType(targetName, AllType.GRAPH);
            node.Attributes.Item3.Accept(this);
            AllType? assignedType = node.Attributes.Item3.OverAllType;

            if (targetType == assignedType)
            {
                //both the attribute type and the assigned value are of the same type.
            }
            else
            {
                _symbolTable.WrongTypeError(node.Attributes.Item1.Name, node.Attributes.Item3.Name);
            }
        }

        public override void Visit(DeclarationNode node)
        {
            VisitChildren(node);
            _symbolTable.SetCurrentNode(node);
            AllType? typeOfVariable;
            if (node.Assignment != null)
            {
                node.Assignment.Parent = node;
                node.Assignment.Accept(this);

                VisitChildren(node);
                typeOfVariable = _symbolTable.RetrieveSymbol(node.Name, out bool isCollection, false);
                if (node.Assignment is BoolComparisonNode)
                {
                    if (node.Assignment.Children[0] is ExpressionNode exprNode)
                    {//TODO typecheckunittests, se om der kan ramme de to fejl herunder
                        if (typeOfVariable == exprNode.OverAllType)
                        {
                            foreach (AbstractNode abnode in exprNode.ExpressionParts)
                            {
                                if (node.Name == abnode.Name)
                                {
                                    _symbolTable.DeclarationCantBeSameVariable(node.Name);
                                }
                                else
                                {
                                    //the expression type and the variable is of same type, and are not the same collection.
                                }
                            }
                        }
                        else
                        {
                            _symbolTable.TypeExpressionMismatch();
                        }
                    }
                    
                    else
                    {
                        throw new Exception("Spørg ezzi - declaration");
                    }
                }
                else if (node.Assignment is VariableNode varNode)
                {
                    if (typeOfVariable == varNode.Type_enum)
                    {
                        if (node.Name == varNode.Name)
                        {
                            _symbolTable.DeclarationCantBeSameVariable(node.Name);
                        }
                    }
                    else
                    {
                        _symbolTable.TypeExpressionMismatch();
                    }
                }
                else
                {
                    AbstractNode abNode = node.Assignment;
                    AllType? typeOfRetreiveVariable = _symbolTable.RetrieveSymbol(abNode.Name, out bool isCollectionRetrieve, false);
                    if (typeOfVariable == abNode.Type_enum && isCollectionRetrieve)
                    {
                        if (node.Name == abNode.Name)
                        {
                            _symbolTable.DeclarationCantBeSameVariable(node.Name);
                        }
                        else
                        {
                            //type correct, variable is a coll, and collections have the same time. inner collection is checked in selectallNode.
                            //and is not the same collection.
                        }
                    }
                    else
                    {
                        if (!isCollectionRetrieve)
                        {
                            _symbolTable.TargetIsNotCollError(node.Name);
                        }
                        else
                        {
                            _symbolTable.TypeExpressionMismatch();
                        }
                    }
                }
            }
            else
            {
                typeOfVariable = _symbolTable.RetrieveSymbol(node.Name, out bool isCollection, false);
                if (typeOfVariable == AllType.VOID)
                {
                    _symbolTable.DeclarationCantBeTypeVoid();
                }
                //The declaration assignment is just null, and therefore the collection is not set to something
            }
        }

        public override void Visit(BoolComparisonNode node)
        {
            _symbolTable.SetCurrentNode(node);
            AllType type_check;
            bool compare = false;
            if (node.Left != null && node.Right != null)
            {
                // Check if the nodes are boolcomparisons
                if (node.Left is BoolComparisonNode && node.Right is BoolComparisonNode)
                {
                    node.Left.Accept(this);
                    node.LeftType = node.Left.Type_enum;
                    node.Right.Accept(this);
                    node.RightType = node.Right.Type_enum;
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
                        node.LeftType = _symbolTable.RetrieveSymbol(node.Left.Children[0].Name) ?? default(AllType);
                    }

                    if (node.Right.Children[0] is ExpressionNode)
                    {
                        node.RightType = ((ExpressionNode)node.Right.Children[0]).OverAllType ?? default(AllType);
                    }
                    else if (node.Right.Children[0] is PredicateNode)
                    {
                        node.RightType = _symbolTable.RetrieveSymbol(node.Right.Children[0].Name) ?? default(AllType);
                    }
                }
                if (node.RightType != AllType.UNKNOWNTYPE && node.LeftType != AllType.UNKNOWNTYPE)
                {
                    if (node.RightType != node.LeftType)
                    {
                        if (!((node.RightType == AllType.INT && node.LeftType == AllType.DECIMAL) || (node.RightType == AllType.DECIMAL && node.LeftType == AllType.INT)))
                        {
                            _symbolTable.WrongTypeConditionError();
                        }
                    }
                }
            }
            else
            {
                VisitChildren(node);
                if (node.HasChildren)
                {
                    if(node.Children[0] is ExpressionNode expNode)
                    {
                        ExpressionNode exnode = (ExpressionNode)node.Children[0];
                    }
                }
            }
        }

        public override void Visit(ExpressionNode node)
        {
            VisitChildren(node);
            _symbolTable.SetCurrentNode(node);
            AllType? previousType = null;
            bool firstWasCollection = false;
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
                            if(node.Parent is BoolComparisonNode boolcNode)
                            {
                                if (boolcNode.Parent is GraphDeclVertexNode vDcl1)
                                {
                                    vDcl1.Type = node.OverAllType.ToString();
                                }
                                else if (boolcNode.Parent is GraphDeclEdgeNode eDcl1)
                                {

                                }
                            }
                            if (node.Parent is GraphDeclVertexNode vDcl2)
                            {
                                vDcl2.Type = node.OverAllType.ToString();
                            }
                            else if (node.Parent is GraphDeclEdgeNode eDcl2)
                            {

                            }
                            else if (item is VariableNode)
                            {
                                previousType = _symbolTable.RetrieveSymbol(item.Name, out firstWasCollection);
                                node.OverAllType = previousType;
                            }
                            else
                            {
                                previousType = node.OverAllType;
                            }
                        }
                        else if (previousType != item.Type_enum)
                        {//types are different from eachother
                            if ((previousType == AllType.INT && node.OverAllType == AllType.DECIMAL) || (previousType == AllType.DECIMAL && node.OverAllType == AllType.INT))
                            {//types are accepted if one is int and one is decimal
                                node.OverAllType = AllType.DECIMAL;
                                //do nothing, but set overalltype to decimal.
                            }
                            else
                            {//types are different from eachother, and do not allow operates between them
                                _symbolTable.TypeExpressionMismatch();
                            }
                        }
                        else
                        {//times are of the same time
                         //bools to control which types are not allowed to be operated upon, even if same time.
                            bool currentWasCollection = false;
                            if (item is VariableNode)
                            {
                                _symbolTable.RetrieveSymbol(item.Name, out currentWasCollection);
                            }
                            bool bothIsBool = previousType == AllType.BOOL && item.Type_enum == AllType.BOOL;
                            bool bothIsGraph = previousType == AllType.GRAPH && item.Type_enum == AllType.GRAPH;
                            bool bothIsVertex = previousType == AllType.VERTEX && item.Type_enum == AllType.VERTEX;
                            bool bothIsEdge = previousType == AllType.EDGE && item.Type_enum == AllType.EDGE;
                            bool bothIsVoid = previousType == AllType.VOID && item.Type_enum == AllType.VOID;
                            bool OneWasCollection = firstWasCollection || currentWasCollection;

                            if (bothIsBool || bothIsGraph || bothIsVertex || bothIsEdge || bothIsVoid || OneWasCollection)
                            {
                                _symbolTable.TypeExpressionMismatch();
                            }
                            else
                            {
                                //do nothing, both is the same type and are allowed, so everything is fine.
                            }
                        }
                    }
                }
            }
            if (node.OverAllType == AllType.UNKNOWNTYPE)
            {//TODO: cant have unit tests, can force an error in giraph to get unknowntype.
                _symbolTable.TypeExpressionMismatch();
            }
        }

        public override void Visit(ReturnNode node)
        {
            VisitChildren(node);
            _symbolTable.SetCurrentNode(node);
            AllType? funcType = _symbolTable.RetrieveSymbol(node.FuncName, out bool FuncTypeCollection, false);
            AllType? returnType = null;
            bool ReturnTypeCollection = false;  

            if (node.LeftmostChild.Children[0] is ExpressionNode expNode && expNode.ExpressionParts != null)
            {
                if (expNode.QueryName != null)
                {
                    _symbolTable.RetrieveSymbol(expNode.QueryName, out bool returnTypeCollection, false);
                    ReturnTypeCollection = returnTypeCollection;
                }
                returnType = expNode.OverAllType;
            }
            if (funcType == AllType.VOID)
            {
                _symbolTable.FunctionIsVoidError(node.FuncName);
            }
            else if (ReturnTypeCollection == FuncTypeCollection)
            // går ikke derind, function bool er false, wait for fix
            {
                if (!(funcType == returnType))
                {
                    //ERROR, conflicting function and return type
                    _symbolTable.WrongTypeError(node.FuncName, node.Name);
                }
            }
            else
            {
                //ERROR, one is collection, other isn't
                _symbolTable.WrongTypeErrorCollection(node.FuncName, node.Name);
            }
        }

        public override void Visit(ForLoopNode node)
        {
            AllType? varDclNodeType;
            _symbolTable.SetCurrentNode(node);
            _symbolTable.OpenScope(BlockType.ForLoop);
            VisitChildren(node);

            if (node.Increment != null && (node.VariableDeclaration != null || node.FromValueNode != null) && node.ToValueOperation != null)
            {
                node.Increment.Accept(this);
                if (node.VariableDeclaration != null)
                {
                    node.VariableDeclaration.Accept(this);
                }
                if (node.FromValueNode != null)
                {
                    node.FromValueNode.Accept(this);
                }
                node.ToValueOperation.Accept(this);
            }

            if (node.Increment is ExpressionNode incrementNode)
            {
                if (node.VariableDeclaration is VariableDclNode varDclNode)
                {
                    varDclNodeType = _symbolTable.RetrieveSymbol(varDclNode.Name);
                    if (varDclNodeType != AllType.INT || incrementNode.OverAllType != AllType.INT)
                    {
                        _symbolTable.WrongTypeConditionError();
                    }
                }
                else
                {
                    if (node.FromValueNode.Type_enum != AllType.INT || incrementNode.OverAllType != AllType.INT)
                    {
                        _symbolTable.WrongTypeConditionError();
                    }
                }
            }
            _symbolTable.CloseScope();
        }

        public override void Visit(ForeachLoopNode node)
        {
            _symbolTable.SetCurrentNode(node);
            AllType? collectionType = _symbolTable.RetrieveSymbol(node.InVariableName, out bool isCollectionInForeach, false);

            if (node.VariableType_enum == collectionType)
            {
                //collection type and variable type is the same.
            }
            else
            {
                _symbolTable.WrongTypeError(node.VariableName, node.InVariableName);
            }
			_symbolTable.OpenScope(BlockType.ForEachLoop);
            VisitChildren(node);
			_symbolTable.CloseScope();
        }

        public override void Visit(WhileLoopNode node)
        {
            _symbolTable.SetCurrentNode(node);
            node.BoolCompare.Accept(this);
            VisitChildrenNewScope(node, BlockType.WhileLoop);
        }

        public override void Visit(VariableAttributeNode node)
        {
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(VariableNode node)
        {
            VisitChildren(node);
            _symbolTable.SetCurrentNode(node);

            if (node.Assignment != null)
            {
                AllType? variableExpressionType = _symbolTable.RetrieveSymbol(node.Assignment.Name);
            }

            _symbolTable.SetCurrentNode(node);
            if (node.Parent is ExpressionNode expNode)
            {
                expNode.OverAllType = _symbolTable.RetrieveSymbol(node.Name);
            }
            node.Type = _symbolTable.RetrieveSymbol(node.Name).ToString();
        }

        public override void Visit(CodeBlockNode node)
        {
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(VariableDclNode node)
        {
            _symbolTable.SetCurrentNode(node);
            AllType? variableType = _symbolTable.RetrieveSymbol(node.Name);
            if (node.Type_enum == AllType.VOID)
            {
                _symbolTable.DeclarationCantBeTypeVoid();
            }
            VisitChildren(node);

            if (node.Children != null)
            {
                foreach (AbstractNode child in node.Children)
                {
                    if (child.Children.Count != 0)
                    {
                        if (child.Children[0] is ExpressionNode expNode)
                        {
                            expNode.Accept(this);
                            if (expNode.OverAllType != variableType)
                            {
                                if (variableType == AllType.DECIMAL && expNode.OverAllType == AllType.INT)
                                {
                                    //TODO det bliver mærkeligt med select query...
                                    //ints can be added to decimal variables, but not vice versa
                                }
                                else
                                {
                                    _symbolTable.WrongTypeError(child.Name, node.Name);
                                }
                            }
                            else
                            {
                                foreach (AbstractNode expPartNode in expNode.ExpressionParts)
                                {
                                    if (expPartNode.Name != null)
                                    {
                                        if (expPartNode.Name == node.Name)
                                        {
                                            _symbolTable.DeclarationCantBeSameVariable(node.Name);
                                        }
                                        else
                                        {
                                            //the variables and the expression is of same type, and have not used the same variable for declaration and for expression.
                                        }
                                    }
                                }
                                //the variables and the expression is of same type, and have not used the same variable for declaration and for expression.
                            }
                        }
                    }
                }
            }
        }

        public override void Visit(OperatorNode node)
        {
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(ConstantNode node)
        {
            _symbolTable.SetCurrentNode(node);
            if (node.Parent is ExpressionNode)
            {
                ExpressionNode parentNode = (ExpressionNode)node.Parent;
                parentNode.OverAllType = node.Type_enum;
            }

        }

        public override void Visit(PrintQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            foreach (BoolComparisonNode abnode in node.Children)
            {
                ExpressionNode exp = (ExpressionNode)abnode.Children[0];
                bool iscollection = false;
                foreach (AbstractNode item in exp.ExpressionParts)
                {
                    if (!iscollection)
                    {//skal bare finde en der er true for collection, så snart den finder en, behøver den ikke gøre mere.
                        if(item is VariableNode)
                        {
                            _symbolTable.RetrieveSymbol(item.Name, out iscollection);
                        }
                    }
                }
                exp.Accept(this);
                bool NonPrintableTypes = exp.OverAllType == AllType.GRAPH || exp.OverAllType == AllType.VERTEX || exp.OverAllType == AllType.EDGE ||
                                         exp.OverAllType == AllType.UNKNOWNTYPE || exp.OverAllType == AllType.VOID || iscollection;
                if (NonPrintableTypes)
                {
                    _symbolTable.NonPrintableError();
                }
            }
        }

        public override void Visit(RunQueryNode node)
        {
            _symbolTable.SetCurrentNode(node);
            bool isCollection = false;
            VisitChildren(node);
            List<FunctionParameterEntry> test = _symbolTable.GetParameterTypes(node.FunctionName);
            test.OrderBy(x => x.ID);
            int i = 0;
            AllType placeholderType = 0;
            AllType? varType = null;

            if (node.Children.Count > 0)
            {
                foreach (AbstractNode child in node.Children)
                {
                    if (child is VariableNode varNode)
                    {
                        varType = _symbolTable.RetrieveSymbol(child.Name, out isCollection);
                        placeholderType = varType ?? default(AllType);

                        if (test.Count > 0)
                        {
                            if (placeholderType != test[i].Type && test[i].Collection == isCollection)
                            {
                                //type error
                                _symbolTable.RunFunctionTypeError(child.Name, test[i].Name);
                            }
                            else
                            {
                                if(node.Parent is ExpressionNode expNode)
                                {
                                    expNode.OverAllType = _symbolTable.RetrieveSymbol(node.FunctionName); ;
                                }
                            }
                        }
                        else
                        {
                            //calling function with no formal parameters
                            _symbolTable.RunFunctionWithNoFormalParameters(node.FunctionName);
                        }
                    }
                    else if (child is ConstantNode constNode)
                    {//TODO hvis man kalder en func, der ikke har parameter, med en constant, indexoutofrange.
                        if(test.Count == 0)
                        {
                            _symbolTable.RunFunctionWithNoFormalParameters(child.Name);
                        }
                        else
                        {
                            if (child.Type_enum != test[i].Type)
                            {
                                _symbolTable.RunFunctionTypeError(child.Name, test[i].Name);
                                //type error
                            }
                            else
                            {
                                if (node.Parent is ExpressionNode expNode)
                                {
                                    expNode.OverAllType = _symbolTable.RetrieveSymbol(node.FunctionName);
                                }
                            }
                        }
                    }
                    ++i;
                }
            }
            else
            {
                //running function without actual parameters, when function has formal parameters
                _symbolTable.RunFunctionWithNoActualParameter(node.FunctionName);
            }

        }

        public override void Visit(PredicateCall node)
        {
            _symbolTable.SetCurrentNode(node);
            VisitChildren(node);
            List<AllType> predParaTypes = _symbolTable.GetPredicateParameters(node.Name);
            int iterator = 0;
            foreach (AbstractNode item in node.Children)
            {
                AllType formalParameterType = predParaTypes[iterator];
                AllType actualParameterType = item.Type_enum;
                if (formalParameterType == actualParameterType)
                {
                    //typecorrect
                }
                else
                {
                    _symbolTable.PredicateTypeError(item.Name);
                }
                iterator++;
            }

        }

        public override void Visit(RemoveQueryNode node)
        {
            var type = _symbolTable.RetrieveSymbol(node.Variable, out bool IsCollection);
            if (IsCollection != true)
            {
                _symbolTable.NotCollection(node.Variable);
            }
            if (node.WhereCondition != null)
            {
                (node.WhereCondition as WhereNode).Type = type.ToString().ToLower();
            }
        }

        public override void Visit(RemoveAllQueryNode node)
        {
            var type = _symbolTable.RetrieveSymbol(node.Variable, out bool IsCollection);
            if (IsCollection != true)
            {
                _symbolTable.NotCollection(node.Variable);
            }
            if (node.WhereCondition != null)
            {
                (node.WhereCondition as WhereNode).Type = type.ToString().ToLower();
            }
        }
    }
}
