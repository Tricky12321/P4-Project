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


        public bool CheckDeclared(string name) {
            if (!SymbolTable.DeclaredLocally(name)) {
                SymbolTable.UndeclaredError(name);
                return false;
            } else {
                return true;
            }
        }

        public bool CheckAlreadyDeclared(string name) {
            if (SymbolTable.DeclaredLocally(name)) {
                SymbolTable.AlreadyDeclaredError(name);
                return false;
            } else {
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

        public void VisitChildrenNewScope(AbstractNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.OpenScope(node.Name);

            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }

            SymbolTable.CloseScope();
        }

        public override void VisitRoot(AbstractNode root)
        {
            SymbolTable.SetCurrentNode(root);
            root.Accept(this);
        }

        public override void Visit(FunctionNode node)
        {
            SymbolTable.SetCurrentNode(node);
            AllType type = Utilities.FindTypeFromString(node.ReturnType);
            string functionName = node.Name;
            if (!SymbolTable.DeclaredLocally(functionName))
            {
                SymbolTable.EnterSymbol(functionName, type);
                SymbolTable.OpenScope(node.Name);
                foreach (ParameterNode parameter in node.Parameters)
                {
                    parameter.Accept(this);
                }
                VisitChildren(node);
                SymbolTable.CloseScope();
            } else {
                
            }
        }

        public override void Visit(ParameterNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckAlreadyDeclared(node.Name))
            {
                SymbolTable.EnterSymbol(node.Name, node.Type_enum);
            }
        }

        public override void Visit(StartNode node)
        {
            SymbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckAlreadyDeclared(node.Name)) {
				SymbolTable.SetCurrentNode(node);
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
            if (CheckAlreadyDeclared(node.Name)) {
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
            string edgeName = node.Name;
            if (CheckAlreadyDeclared(edgeName)) {
				SymbolTable.EnterSymbol(edgeName, AllType.EDGE);
                CheckDeclared(node.VertexNameFrom);
                CheckDeclared(node.VertexNameTo);
                // TODO: ValueList i Graph skal laves om til at understøtte expressions
                foreach (var attribute in node.ValueList)
                {
                    SymbolTable.AttributeDefined(attribute.Key, AllType.EDGE);
                }
            }
        }

        public override void Visit(SetQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckDeclared(node.InVariable)) {
                
            }
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(WhereNode node)
        {
            SymbolTable.SetCurrentNode(node);
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string longAttributeName = node.ExtensionName;
            AllType attributeType = Utilities.FindTypeFromString(node.ExtendWithType);
            // If there is a shortname AND a long name, create 2 entries in the class table
            if (node.ExtensionShortName != null && node.ExtensionShortName.Length > 0)
            {
                string shortAttributeName = node.ExtensionShortName;
                SymbolTable.ExtendClass(attributeType, longAttributeName, shortAttributeName, node.ClassToExtend_enum);
            }
            else
            {
                // If only a long name is used, ignore the shortAttribute
                SymbolTable.ExtendClass(attributeType, longAttributeName, node.ClassToExtend_enum);
            }
        }

        #region CollOPSvisits
        public override void Visit(DequeueQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
        }

        public override void Visit(EnqueueQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
        }

        public override void Visit(ExtractMinQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
        }

        public override void Visit(PopQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
        }

        public override void Visit(PushQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckDeclared(node.VariableCollection)) {
                node.VariableToAdd.Accept(this);
            }
        }

        public override void Visit(SelectAllQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
        }

        public override void Visit(SelectQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Variable);
        }
        #endregion

        public override void Visit(PredicateNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string predicateName = node.Name;
            SymbolTable.EnterSymbol(predicateName, AllType.BOOL);
            SymbolTable.OpenScope(node.Name);
            foreach (ParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(IfElseIfElseNode node)
        {
            SymbolTable.SetCurrentNode(node);
            Visit(node.IfCondition);
            VisitChildrenNewScope(node.IfCodeBlock);

            int count = node.ElseIfList.Count();
            for (int i = 0; i < count; i++)
            {
                VisitChildren(node.ElseIfList[i].Item1);
                VisitChildrenNewScope(node.ElseIfList[i].Item2);
            }
            VisitChildren(node.ElseCodeBlock);
        }

        public override void Visit(GraphSetQuery node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.AttributeDefined(node.Name, AllType.GRAPH);
        }

        public override void Visit(DeclarationNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (CheckAlreadyDeclared(node.Name)) {
				SymbolTable.EnterSymbol(node.Name, node.Type_enum);
            }
        }
        
        public override void Visit(BoolComparisonNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (node.Left != null && node.Right != null) {
                node.Left.Accept(this);
                node.Right.Accept(this);
            } else {
                VisitChildren(node);
            }
        }

        public override void Visit(ExpressionNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ReturnNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ForLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);
            //TODO: VariableDcl in the forloop does not work...
            //Remake of the forloop node is needed (ASTBuilder)
            // Check if the InlineDCL isnt declared already, if it isnt, add it to the symbolTable
            SymbolTable.OpenScope(BlockType.ForLoop);

			if (node.VariableDeclaration != null) {
                node.VariableDeclaration.Accept(this);
			}
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(ForeachLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.OpenScope(BlockType.ForEachLoop);
            // Check the new declared variable
            if (SymbolTable.DeclaredLocally(node.VariableName)) {
                SymbolTable.AlreadyDeclaredError(node.VariableName);
            } else {
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
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            SymbolTable.SetCurrentNode(node);
            Visit(node.BoolCompare);
            SymbolTable.OpenScope(BlockType.WhileLoop);
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(VariableAttributeNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(VariableNode node)
        {
            SymbolTable.SetCurrentNode(node);
            CheckDeclared(node.Name);
        }

        public override void Visit(AddQueryNode node)
        {
            SymbolTable.SetCurrentNode(node);
            if (node.IsGraph) {
				foreach (var item in node.Dcls)
				{
					item.Accept(this);
				}
            }
            CheckDeclared(node.ToVariable);

            if (node.WhereCondition != null) {
                node.Accept(this);
            }
        }



    }
}
