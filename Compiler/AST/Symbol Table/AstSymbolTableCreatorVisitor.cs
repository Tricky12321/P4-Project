using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.AST.Nodes;
using Compiler.AST.Nodes.DatatypeNodes;
using Compiler.AST.Nodes.LoopNodes;
using Compiler.AST.Nodes.QueryNodes;

namespace Compiler.AST.SymbolTable
{
    internal class AstSymbolTableCreatorVisitor : AstVisitorBase
    {
        public SymTable SymbolTable = new SymTable();
        private bool _initialBuildDone = false;
        public bool MainDefined = false;
        public bool CheckDeclared(string name)
        {
            if (name != null)
            {

                // Means it is a function call, or a Attribute call on a class
                if (name.Contains("."))
                {
                    if (name.Contains("("))
                    {
                        name = name.Substring(0, name.IndexOf('('));
                    }
                    List<string> Names = name.Split('.').ToList();
                    if (CheckDeclared(Names[0]))
                    {
                        AllType type = SymbolTable.GetVariableType(Names[0]);
                        return SymbolTable.AttributeDefined(Names[1], type);
                    }
                    else
                    {
                        SymbolTable.UndeclaredError(name);
                        return false;
                    }
                }
                else
                {
                    if (!SymbolTable.DeclaredLocally(name))
                    {
                        SymbolTable.UndeclaredError(name);
                        return false;
                    }
                    return true;
                }
            }
            throw new Exception("Name is null in CheckDeclared!");
        }

        public bool CheckAlreadyDeclared(string name)
        {
            if (name == null || name == "")
            {
                return true;
            }
            if (SymbolTable.DeclaredLocally(name))
            {
                SymbolTable.AlreadyDeclaredError(name);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void BuildSymbolTable(AbstractNode root)
        {
            VisitRoot(root);
        }

        public bool IsClass(AllType Type)
        {

            switch (Type)
            {
                case AllType.EDGE:
                case AllType.GRAPH:
                case AllType.VERTEX:
                    return true;
                default:
                    return false; ;
            }
        }

        //All the visit stuff-----------------------------------------
        public override void Visit(VariableDclNode node)
        {
            SymbolTable.SetCurrentNode(node);
            var name = node.Name;
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, node.Type_enum);
            }
        }

        public override void Visit(AbstractNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.NotImplementedError(node);
        }

        public void VisitChildrenNewScope(AbstractNode node, string Name)
        {
            if (node != null)
            {
                SymbolTable.OpenScope(Name);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }

                SymbolTable.CloseScope();
            }
        }

        public void VisitChildrenNewScope(AbstractNode node, BlockType Type)
        {
            if (node != null)
            {
                SymbolTable.OpenScope(Type);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }

                SymbolTable.CloseScope();
            }
        }

        public void VisitChildrenNewScope(AbstractNode node)
        {
            if (node != null)
            {
                SymbolTable.OpenScope(node.Name);
                foreach (AbstractNode child in node.GetChildren())
                {
                    child.Accept(this);
                }

                SymbolTable.CloseScope();
            }
        }

        public override void VisitRoot(AbstractNode root)
        {
            SymbolTable.SetCurrentNode(root);
            root.Accept(this);
        }

        public override void Visit(FunctionNode node)
        {
            if (node.Name == "Main")
            {
                MainDefined = true;
                if (node.Parameters.Count > 0)
                {
                    SymbolTable.MainHasParameters();
                }
                if (Utilities.FindTypeFromString(node.ReturnType) != AllType.VOID)
                {
                    SymbolTable.MainHasWrongReturnType();
                }
            }
            SymbolTable.SetCurrentNode(node);
            AllType type = Utilities.FindTypeFromString(node.ReturnType);
            string functionName = node.Name;
            if (!SymbolTable.DeclaredLocally(functionName))
            {
                SymbolTable.EnterSymbol(functionName, type, node.IsCollection);
                SymbolTable.OpenScope(node.Name);

                foreach (ParameterNode parameter in node.Parameters)
                {
                    SymbolTable.EnterFunctionParameter(node.Name, parameter.Name, parameter.Type_enum, parameter.IsCollection);
                    parameter.Accept(this);
                }
                if (_initialBuildDone)
                {
                    VisitChildren(node);
                }
                SymbolTable.CloseScope();
            }
        }

        public override void Visit(ParameterNode node)
        {
            SymbolTable.SetCurrentNode(node);

            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, node.Type_enum);
                if (node.Parent != null && (node.Parent is FunctionNode))
                {

                }
            }
        }

        public override void Visit(StartNode node)
        {
            SymbolTable.SetCurrentNode(node);
            List<AbstractNode> PredicateNodes;
            List<AbstractNode> FunctionNodes;
            // Visit all extend nodes as the first task
            node.Children.Where(x => x is ExtendNode).ToList().ForEach(x => x.Accept(this));
            // Index all function nodes
            FunctionNodes = node.Children.Where(x => x is FunctionNode).ToList();
            // Accept all the function nodes, so they can enter themselfes in the symboltable
            // Also to enter all the parameters into the symbol table
            FunctionNodes.ForEach(x => x.Accept(this));
            // grab all predicate nodes, that are not in a function, and add them to a list
            PredicateNodes = node.Children.Where(x => x is PredicateNode).ToList();
            // Accept them all, but dont acces their body
            PredicateNodes.ForEach(x => x.Accept(this));
            // Now that everything is declared for both functions and predicates, Visit their body(children)
            FunctionNodes.ForEach(x => VisitChildrenNewScope(x));
            PredicateNodes.ForEach(x => VisitChildrenNewScope(x));

            // Set initialBuildDone so predicates now will visit their children when visited inside functions
            _initialBuildDone = true;

            //VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, AllType.GRAPH);
                foreach (var Vertex in node.Vertices)
                {
                    Vertex.Accept(this);
                }
                foreach (var Edge in node.Edges)
                {
                    Edge.Accept(this);
                }
                VisitChildren(node);
            }
        }

        public override void Visit(GraphDeclVertexNode node)
        {
            SymbolTable.SetCurrentNode(node);
            /* Missing the values of the vertex*/
            string vertexName = node.Name;
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(vertexName, AllType.VERTEX);
                foreach (var attribute in node.ValueList)
                {
                    SymbolTable.AttributeDefined(attribute.Key, AllType.VERTEX);
                }
            }
        }

        public override void Visit(GraphDeclEdgeNode node)
        {
            SymbolTable.SetCurrentNode(node);
            /* Missing the values of the edge*/
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, AllType.EDGE);
                CheckDeclared(node.VertexNameFrom);
                CheckDeclared(node.VertexNameTo);
                foreach (var attribute in node.ValueList)
                {
                    SymbolTable.AttributeDefined(attribute.Key, AllType.EDGE);
                }
            }
        }

        public override void Visit(SetQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            foreach (var Exp in node.Attributes)
            {
                Exp.Item1.Accept(this);
                Exp.Item3.Accept(this);
            }
            if (node.HasChildren)
            {
                VisitChildren(node);
            }
            if (node.InVariable != null)
            {
                CheckDeclared(node.InVariable.Name);
            }
            if (node.WhereCondition != null)
            {
				(node.WhereCondition as WhereNode).AttributeClass = node.Type_enum;
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(WhereNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.OpenScope(BlockType.WhereStatement);
            SymbolTable.AddClassVariablesToScope(node.AttributeClass);
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(ExtendNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string longAttributeName = node.ExtensionName;
            // If there is a shortname AND a long name, create 2 entries in the class table
            if (node.ExtensionShortName != null && node.ExtensionShortName.Length > 0)
            {
                string shortAttributeName = node.ExtensionShortName;
                SymbolTable.ExtendClass(node.ExtendWithType_enum, longAttributeName, shortAttributeName, node.ClassToExtend_enum, node.IsCollection);
            }
            else
            {
                // If only a long name is used, ignore the shortAttribute
                SymbolTable.ExtendClass(node.ExtendWithType_enum, longAttributeName, node.ClassToExtend_enum, node.IsCollection);
            }
        }

        public override void Visit(DequeueQueryNode node)
        {
            // TODO: Check if a its a variable that is being added or a constant
            SymbolTable.SetCurrentNode(node);
            if (node.Variable != null)
            {
                CheckDeclared(node.Variable);
            }
        }

        public override void Visit(EnqueueQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.VariableCollection);

        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(PopQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
        }

        public override void Visit(PushQueryNode node)
        {
            // TODO: Check if a its a variable that is being added or a constant
            SymbolTable.SetCurrentNode(node);
            if (CheckDeclared(node.VariableCollection))
            {
                node.VariableToAdd.Accept(this);
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
            if (node.WhereCondition != null)
            {
				var type = SymbolTable.RetrieveSymbol(node.Variable);
				(node.WhereCondition as WhereNode).AttributeClass = type ?? default(AllType);
                node.WhereCondition.Accept(this);
            }

        }

        public override void Visit(SelectQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
            if (node.WhereCondition != null)
            {
                (node.WhereCondition as WhereNode).AttributeClass = SymbolTable.RetrieveSymbol(node.Variable) ?? default(AllType);
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(PredicateNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string predicateName = node.Name;
            SymbolTable.EnterSymbol(predicateName, AllType.BOOL);
            SymbolTable.AddPredicateToList(predicateName);
            SymbolTable.OpenScope(node.Name);
            foreach (ParameterNode parameter in node.Parameters)
            {
                SymbolTable.EnterPredicateParameter(node.Name, parameter.Type_enum);
                parameter.Accept(this);
            }

            if (_initialBuildDone)
            {
                VisitChildren(node);
            }
            SymbolTable.CloseScope();
        }

        public override void Visit(IfElseIfElseNode node)
        {
            SymbolTable.SetCurrentNode(node);
            // If statements
            Visit(node.IfCondition);
            VisitChildrenNewScope(node.IfCodeBlock, BlockType.IfStatement);

            foreach (var ElseIf in node.ElseIfList)
            {
                VisitChildren(ElseIf.Item1);
                VisitChildrenNewScope(ElseIf.Item2, BlockType.ElseifStatement);
            }

            VisitChildrenNewScope(node.ElseCodeBlock, BlockType.ElseStatement);
        }

        public override void Visit(GraphSetQuery node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.AttributeDefined(node.Name, AllType.GRAPH);
        }

        public override void Visit(DeclarationNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, node.Type_enum, node.CollectionDcl);
            }
            if (node.Assignment != null)
            {
                node.Assignment.Accept(this);
            }
        }

        public override void Visit(BoolComparisonNode node)
        {
            SymbolTable.SetCurrentNode(node);

            if (node.Left != null && node.Right != null)
            {
                node.Left.Accept(this);
                node.Right.Accept(this);
            }
            else
            {
                VisitChildren(node);
            }
        }

        public override void Visit(ExpressionNode node)
        {
            SymbolTable.SetCurrentNode(node);
            foreach (var Exp in node.ExpressionParts)
            {
                Exp.Accept(this);
            }
            HandleExpressionTypes(node);
        }

        public void HandleExpressionTypes(ExpressionNode node)
        {
            foreach (var item in node.ExpressionParts)
            {
                if (item is VariableNode)
                {
                    AllType Type = SymbolTable.GetVariableType(item.Name);
                    node.ExpressionTypes.Add(Type);
                }
                else if (item is ConstantNode)
                {
                    node.ExpressionTypes.Add(((ConstantNode)item).Type_enum);
                }
            }
            node.FindOverAllType();
        }

        public override void Visit(ReturnNode node)
        {
            SymbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(ForLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);

            SymbolTable.OpenScope(BlockType.ForLoop);
            if (node.VariableDeclaration != null)
            {
                node.VariableDeclaration.Accept(this);
            }
            node.ToValueOperation.Accept(this);

            if (node.Increment != null)
            {
                node.Increment.Accept(this);
            }
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(ForeachLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.OpenScope(BlockType.ForEachLoop);
            // Check the new declared variable
            if (SymbolTable.DeclaredLocally(node.VariableName))
            {
                SymbolTable.AlreadyDeclaredError(node.VariableName);
            }
            else
            {
                SymbolTable.EnterSymbol(node.VariableName, node.VariableType_enum);
            }
            // CHeck if the variable (collection) to loop though, is defined!
            SymbolTable.CheckIfDefined(node.InVariableName);
            // Visit all children (codeBlock items)
            VisitChildren(node);
            // Close the scope again
            SymbolTable.CloseScope();
        }

        public override void Visit(CodeBlockNode node)
        {
            SymbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);
            node.BoolCompare.Accept(this);
            SymbolTable.OpenScope(BlockType.WhileLoop);
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(VariableAttributeNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (node.IsAttribute && CheckDeclared(node.ClassVariableName))
            {
                bool IsCollection = false;

                node.ClassType = SymbolTable.GetVariableType(node.ClassVariableName);
                SymbolTable.RetrieveSymbol(node.ClassVariableName, out IsCollection);
                node.IsCollection = IsCollection;

                if (SymbolTable.AttributeDefined(node.Name, node.ClassType)) {
                    var AttriType = SymbolTable.GetAttributeType(node.Name, node.ClassType, out IsCollection);
                }
            }
            else
            {
                CheckDeclared(node.Name);
            }
        }

        public override void Visit(VariableNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Name);
            bool IsCollection = false;
            var name = node.Name;
            node.Type = (SymbolTable.RetrieveSymbol(node.Name, out IsCollection) ?? default(AllType)).ToString().ToLower();
            node.IsCollection = IsCollection;
        }

        public override void Visit(AddQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (node.IsGraph)
            {
                foreach (var item in node.Dcls)
                {
                    item.Accept(this);
                }
            }
            CheckDeclared(node.ToVariable);

            if (node.WhereCondition != null)
            {
                node.WhereCondition.Accept(this);
            }
        }

        public override void Visit(OperatorNode node)
        {
            // Operator nodes are not entered into the symbol table, so these can be ignored
            // SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ConstantNode node)
        {

            // Constants are note entered into the symbol table, so these can be ignored
            // SymbolTable.NotImplementedError(node);
        }

        public override void Visit(PrintQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(RunQueryNode node)
        {
            var Type = SymbolTable.FunctionReturnType(node.FunctionName);
            if (Type == null)
            {
                SymbolTable.UndeclaredFunction(node.FunctionName);
            }
            VisitChildren(node);
        }

        public override void Visit(PredicateCall node)
        {
            CheckDeclared(node.Name);
        }

        public override void Visit(RemoveQueryNode node)
        {
            if (CheckDeclared(node.Variable))
            {
                if (node.WhereCondition != null)
                {
                    node.WhereCondition.Accept(this);
                }
            }
        }

        public override void Visit(RemoveAllQueryNode node)
        {
            if (CheckDeclared(node.Variable))
            {
                if (node.WhereCondition != null)
                {
                    node.WhereCondition.Accept(this);
                }
            }
        }
    }
}
