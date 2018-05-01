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

        private bool IsNotEqual(string name1, string name2)
        {
            return !(name1 == name2);
        }


        //-----------------------------Visitor----------------------------------------------
        public override void Visit(ParameterNode node)
        {
            VisitChildren(node);
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
                    if (Attributes.Item1 is AttributeNode attNode)
                    {
                        //skal finde ud af hvad der er extended.
                        AllType? extentiontype = _createdSymbolTabe.RetrieveSymbol(Attributes.Item1.ClassVariableName);
                        if (extentiontype != null)
                        {
                            if (_createdSymbolTabe.IsExtended(variableName, extentiontype ?? default(AllType)))
                            {
                                AllType? attributeType = _createdSymbolTabe.GetAttributeType(variableName, extentiontype ?? default(AllType));

                                if (!(attributeType == extentiontype))
                                {
                                    //type wrong
                                    _createdSymbolTabe.WrongTypeError(Attributes.Item1.Name, Attributes.Item1.ClassVariableName);
                                }
                            }
                        }
                    }
                    else
                    {
                        variableType = _createdSymbolTabe.RetrieveSymbol(variableName);
                    }

                    if (Attributes.Item3 != null)
                    {
                        Attributes.Item3.Accept(this);
                    }

                    if (Attributes.Item3.OverAllType != variableType)
                    {
                        //type between varible and overalltyper
                        _createdSymbolTabe.WrongTypeError(variableName, Attributes.Item3.Name);
                    }

                    if (node.InVariable != null)
                    {
                        inVariableType = _createdSymbolTabe.RetrieveSymbol(node.InVariable.Name);
                        if (inVariableType != variableType)
                        {
                            //error  with invariable
                        }
                    }
                }


            }
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
            _createdSymbolTabe.OpenScope(node.Name);

            VisitChildren(node);

            _createdSymbolTabe.CloseScope();
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);

            AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            AllType? typeAttribute = AllType.UNKNOWNTYPE;
            if (isCollectionInQuery)
            {
                if (node.Attribute != null)
                {
                    if (collectionNameType != null && collectionNameType != AllType.INT && collectionNameType != AllType.DECIMAL)
                    {
                        if (_createdSymbolTabe.IsExtended(node.Attribute, collectionNameType ?? default(AllType)))
                        {
                            typeAttribute = _createdSymbolTabe.GetAttributeType(node.Attribute, collectionNameType ?? default(AllType));
                            if (typeAttribute == AllType.DECIMAL || typeAttribute == AllType.INT)
                            {
                                //variable is a collection, which are different from decimal and int collections, 
                                //an attribute is specified, and are of type int or decimal, which ir MUST be!
                                node.Type = collectionNameType.ToString();
                            }
                            else
                            {
                                //attribute is other than int or decimal, which it may not be.
                                _createdSymbolTabe.AttributeIllegal();
                            }
                        }
                        else
                        {
                            //the class is not extended with given attribute
                            _createdSymbolTabe.AttributeNotExtendedOnClass(node.Attribute, collectionNameType);
                        }
                    }
                    else
                    {
                        //the collection type is either int, decimal or null. which are not legal.
                        _createdSymbolTabe.ExtractCollNotIntOrDeciError();
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
                        _createdSymbolTabe.NoAttriProvidedCollNeedsToBeIntOrDecimalError();
                    }
                    //attribute not specified - coll needs to be int or deci
                }
            }
            else
            {
                //the from variable needs to be a collection. You cannot retrieve something from a variable - only retrieve from a collections.
                _createdSymbolTabe.FromVarIsNotCollError(node.Variable);
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
            _createdSymbolTabe.SetCurrentNode(node);

            AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            AllType? typeAttribute = AllType.UNKNOWNTYPE;
            if (isCollectionInQuery)
            {
                if (node.Attribute != null)
                {
                    if (collectionNameType != null && collectionNameType != AllType.INT && collectionNameType != AllType.DECIMAL)
                    {
                        if (_createdSymbolTabe.IsExtended(node.Attribute, collectionNameType ?? default(AllType)))
                        {
                            typeAttribute = _createdSymbolTabe.GetAttributeType(node.Attribute, collectionNameType ?? default(AllType));
                            if (typeAttribute == AllType.DECIMAL || typeAttribute == AllType.INT)
                            {
                                //variable is a collection, which are different from decimal and int collections, 
                                //an attribute is specified, and are of type int or decimal, which ir MUST be!
                                node.Type = collectionNameType.ToString();
                            }
                            else
                            {
                                //attribute is other than int or decimal, which it may not be.
                                _createdSymbolTabe.AttributeIllegal();
                            }
                        }
                        else
                        {
                            //the class is not extended with given attribute
                            _createdSymbolTabe.AttributeNotExtendedOnClass(node.Attribute, collectionNameType);
                        }
                    }
                    else
                    {
                        //the collection type is either int, decimal or null. which are not legal.
                        _createdSymbolTabe.ExtractCollNotIntOrDeciError();
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
                        _createdSymbolTabe.NoAttriProvidedCollNeedsToBeIntOrDecimalError();
                    }
                    //attribute not specified - coll needs to be int or deci
                }
            }
            else
            {
                //the from variable needs to be a collection. You cannot retrieve something from a variable - only retrieve from a collections.
                _createdSymbolTabe.FromVarIsNotCollError(node.Variable);
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
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? collectionNameType = _createdSymbolTabe.RetrieveSymbol(node.Variable, out bool isCollectionInQuery, false);
            bool fromIsColl = isCollectionInQuery;
            if (fromIsColl)
            {
                if (node.Parent is DeclarationNode dclNode)
                {
                    node.Type = collectionNameType.ToString();
                    node.Name = dclNode.Name;
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
                //TODO correct error message pls
            }
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
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
                        expNode.Name = node.Variable;
                    }
                    node.Type = collectionNameType.ToString();
                }
                else
                {
                    _createdSymbolTabe.FromVarIsNotCollError(node.Variable);
                }

                if (node.WhereCondition != null)
                {
                    node.WhereCondition.Accept(this);
                }
            }
        }

        public override void Visit(PushQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            AllType? varToAdd;
            AllType? collectionToAddTo = _createdSymbolTabe.RetrieveSymbol(node.VariableCollection, out bool isCollectionInQuery, false);
            node.Type = collectionToAddTo.ToString();
            if (node.VariableToAdd is ConstantNode constant)
            {
                varToAdd = constant.Type_enum;
                bool sameType = varToAdd == collectionToAddTo;
                bool typeCorrect = sameType && isCollectionInQuery;
                if (typeCorrect)
                {
                    //constant is fine, collection is fine
                }
                else
                {
                    _createdSymbolTabe.WrongTypeError(node.variableName, node.VariableCollection);
                }
                node.VariableToAdd.Accept(this);
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
                node.VariableToAdd.Accept(this);
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
                        expNode.Name = node.Variable;
                    }
                    node.Type = collectionNameType.ToString();
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
            node.Type = collectionToAddTo.ToString();
            if (node.VariableToAdd is ConstantNode constant)
            {
                varToAdd = constant.Type_enum;
                bool sameType = varToAdd == collectionToAddTo;
                bool typeCorrect = sameType && isCollectionInQuery;
                if (typeCorrect)
                {
                    //constant is fine, collection is fine
                }
                node.VariableToAdd.Accept(this);
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
                node.VariableToAdd.Accept(this);
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
                    expNode.Name = node.Variable;
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
                node.Type = TypeOfTargetCollection.ToString();
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
                node.Type = TypeOfTargetCollection.ToString();
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
                if (typeOfKey == node.Type_enum)
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
            if (vertexFromType == AllType.VERTEX && vertexToType == AllType.VERTEX)
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
            foreach (AbstractNode item in node.Parameters)
            {
                item.Accept(this);
            }
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
            string targetName = node.Attributes.Item1.Name;
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
            AllType? typeOfVariable;
            if (node.Assignment != null)
            {
                node.Assignment.Parent = node;
                node.Assignment.Accept(this);

                VisitChildren(node);
                typeOfVariable = _createdSymbolTabe.RetrieveSymbol(node.Name, out bool isCollection, false);
                if (node.Assignment is ExpressionNode exprNode)
                {
                    if (typeOfVariable == exprNode.OverAllType)
                    {
                        foreach(AbstractNode abnode in exprNode.ExpressionParts)
                        {
                            if (IsNotEqual(node.Name, abnode.Name))
                            {
                                //the expression type and the variable is of same type, and are not the same collection.
                            }
                            else
                            {
                                _createdSymbolTabe.DeclarationCantBeSameVariable(node.Name);
                            }
                        }
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
                        if (IsNotEqual(node.Name, selAll.Variable))
                        {
                            //type correct, variable is a coll, and collections have the same time. inner collection is checked in selectallNode.
                            //and is not the same collection.
                        }
                        else
                        {
                            _createdSymbolTabe.DeclarationCantBeSameVariable(node.Name);
                        }
                    }
                    else
                    {
                        if (!isCollection)
                        {
                            _createdSymbolTabe.TargetIsNotCollError(node.Name);
                        }
                        else
                        {
                            _createdSymbolTabe.TypeExpressionMismatch();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("something weird - spørg ezzi");
                }
            }
            else
            {
                VisitChildren(node);
                typeOfVariable = _createdSymbolTabe.RetrieveSymbol(node.Name, out bool isCollection, false);
                if (typeOfVariable == AllType.VOID)
                {
                    _createdSymbolTabe.DeclarationCantBeTypeVoid();
                }
                //The declaration assignment is just null, and therefore the collection is not set to something
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
                            if (node.Parent is GraphDeclVertexNode vDcl)
                            {
                                vDcl.Type = node.OverAllType.ToString();

                            }
                            else if (node.Parent is GraphDeclEdgeNode eDcl)
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

            if (node.LeftmostChild is ExpressionNode expNode && expNode.ExpressionParts != null)
            {
                if (expNode.QueryName != null)
                {
                    _createdSymbolTabe.RetrieveSymbol(expNode.QueryName, out bool returnTypeCollection, false);
                    ReturnTypeCollection = returnTypeCollection;
                }
                returnType = expNode.OverAllType;
            }
            if (funcType == AllType.VOID)
            {
                _createdSymbolTabe.FunctionIsVoidError(node.FuncName);
            }
            if (ReturnTypeCollection == FuncTypeCollection)
            // går ikke derind, function bool er false, wait for fix
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

            if (node.Increment != null)
            {
                node.Increment.Accept(this);
            }
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
            if (node.Parent is ExpressionNode expNode)
            {
                expNode.OverAllType = _createdSymbolTabe.RetrieveSymbol(node.Name);
            }
            node.Type = _createdSymbolTabe.RetrieveSymbol(node.Name).ToString();
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
            if (node.Type_enum == AllType.VOID)
            {
                _createdSymbolTabe.DeclarationCantBeTypeVoid();
            }

            if (node.Children != null)
            {
                foreach (AbstractNode child in node.Children)
                {
                    if (child is ExpressionNode expNode)
                    {
                        expNode.Accept(this);
                        if (expNode.OverAllType != variableType)
                        {
                            _createdSymbolTabe.WrongTypeError(child.Name, node.Name);
                        }
                        else
                        {
                            if(expNode.Name != null)
                            {
                                if (IsNotEqual(expNode.Name, node.Name))
                                {
                                    //the variables and the expression is of same type, and have not used the same variable for declaration and for expression.
                                }
                                else
                                {
                                    _createdSymbolTabe.DeclarationCantBeSameVariable(node.Name);
                                }
                            }
                            else
                            {
                                foreach(AbstractNode expPartNode in expNode.ExpressionParts)
                                {
                                    if (expPartNode.Name != null)
                                    {
                                        if (IsNotEqual(expPartNode.Name, node.Name))
                                        {
                                            //the variables and the expression is of same type, and have not used the same variable for declaration and for expression.
                                        }
                                        else
                                        {
                                            _createdSymbolTabe.DeclarationCantBeSameVariable(node.Name);
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
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
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
            foreach (ExpressionNode exp in node.Children)
            {
                exp.Accept(this);
                bool NonPrintableTypes = exp.OverAllType == AllType.GRAPH || exp.OverAllType == AllType.VERTEX || exp.OverAllType == AllType.EDGE ||
                                         exp.OverAllType == AllType.UNKNOWNTYPE || exp.OverAllType == AllType.VOID || exp.OverAllType == AllType.COLLECTION;
                if (NonPrintableTypes)
                {
                    _createdSymbolTabe.NonPrintableError();
                }
                else
                {
                    foreach (AbstractNode item in exp.ExpressionParts)
                    {
                        if (!(item is ConstantNode))
                        {
                            AllType? NonUsedVariable = _createdSymbolTabe.RetrieveSymbol(item.Name, out bool isCollection, false);
                            if (isCollection)
                            {
                                _createdSymbolTabe.NonPrintableError();
                            }
                        }
                    }
                }
            }
        }

        public override void Visit(RunQueryNode node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            List<FunctionParameterEntry> test = _createdSymbolTabe.GetParameterTypes(node.FunctionName);
            test.OrderBy(x => x.ID);
            int i = 0;
            AllType placeholderType = 0;
            AllType? varType = null;

            if (node.Children != null)
            {
                foreach (AbstractNode child in node.Children)
                {
                    if (child is VariableNode varNode)
                    {
                        varType = _createdSymbolTabe.RetrieveSymbol(child.Name);
                        placeholderType = varType ?? default(AllType);
                        if (placeholderType != test[i].Type)
                        {
                            //type error
                            _createdSymbolTabe.RunFunctionError(child.Name, test[i].Name);
                        }
                    }

                    else if (child is ConstantNode constNode)
                    {
                        if (child.Type_enum != test[i].Type)
                        {
                            _createdSymbolTabe.RunFunctionError(child.Name, test[i].Name);
                            //type error
                        }
                    }
                }
            }

        }

        public override void Visit(PredicateCall node)
        {
            _createdSymbolTabe.SetCurrentNode(node);
            VisitChildren(node);
            List<AllType> predParaTypes = _createdSymbolTabe.GetPredicateParameters(node.Name);
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
                    _createdSymbolTabe.PredicateTypeError(item.Name);
                }
                iterator++;
            }

        }
    }
}
