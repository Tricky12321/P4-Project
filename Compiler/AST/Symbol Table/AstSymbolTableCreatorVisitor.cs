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
                return true;
            } else {
                SymbolTable.AlreadyDeclaredError(name);
                return false;
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
        public override void Visit(AbstractNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public void VisitChildrenNewScope(AbstractNode node)
        {
            SymbolTable.OpenScope(node.Name);

            foreach (AbstractNode child in node.GetChildren())
            {
                Visit(child);
            }

            SymbolTable.CloseScope();
        }

        public override void VisitRoot(AbstractNode root)
        {
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
                foreach (FunctionParameterNode parameter in node.Parameters)
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
            if (!SymbolTable.DeclaredLocally(node.Name))
            {
				SymbolTable.SetCurrentNode(node);
				SymbolTable.EnterSymbol(node.Name, node.Type);
            }
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            if (CheckDeclared(node.Name)) {
				SymbolTable.SetCurrentNode(node);
				string graphName = node.Name;
				SymbolTable.EnterSymbol(graphName, AllType.GRAPH);
				VisitChildren(node);
            }
        }

        public override void Visit(VertexNode node)
        {
            SymbolTable.SetCurrentNode(node);
            /* Missing the values of the vertex*/
            string vertexName = node.Name;
            SymbolTable.EnterSymbol(vertexName, AllType.VERTEX);
        }

        public override void Visit(EdgeNode node)
        {
            SymbolTable.SetCurrentNode(node);
            /* Missing the values of the edge*/
            string edgeName = node.Name;
            SymbolTable.EnterSymbol(edgeName, AllType.EDGE);
        }

        public override void Visit(SetQueryNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(WhereNode node)
        {
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
                SymbolTable.ExtendClass(attributeType, longAttributeName, shortAttributeName);
            }
            else
            {
                // If only a long name is used, ignore the shortAttribute
                SymbolTable.ExtendClass(attributeType, longAttributeName);
            }
        }

        #region CollOPSvisits
        public override void Visit(DequeueQueryNode node)
        {
        }

        public override void Visit(EnqueueQueryNode node)
        {
        }

        public override void Visit(ExtractMaxQueryNode node)
        {
        }

        public override void Visit(ExtractMinQueryNode node)
        {
        }

        public override void Visit(PopQueryNode node)
        {
        }

        public override void Visit(PushQueryNode node)
        {
        }

        public override void Visit(SelectAllQueryNode node)
        {
        }

        public override void Visit(SelectQueryNode node)
        {
        }

        #endregion

        public override void Visit(PredicateNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string predicateName = node.Name;
            SymbolTable.EnterSymbol(predicateName, AllType.BOOL);
            SymbolTable.OpenScope(node.Name);
            foreach (PredicateParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            VisitChildren(node);
            SymbolTable.CloseScope();
        }

        public override void Visit(PredicateParameterNode node)
        {
            SymbolTable.SetCurrentNode(node);
            SymbolTable.EnterSymbol(node.Name, ResolveFuncType(node.Type));
        }

        public override void Visit(CollectionNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(IfElseIfElseNode node)
        {
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
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(DeclarationNode node)
        {
            SymbolTable.EnterSymbol(node.Name, Utilities.FindTypeFromString(node.Type));
        }
        
        public override void Visit(BoolComparisonNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ExpressionNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ForLoopNode node)
        {
            if (node.VariableDeclaration != null)
            {
                Visit(node.VariableDeclaration);
            }
            if (node.VariableDeclaration != null)
            {
                Visit(node.VariableDeclaration);
            }
            VisitChildrenNewScope(node);
        }

        public override void Visit(ForeachLoopNode node)
        {
            SymbolTable.NotImplementedError(node);
        }
        public override void Visit(CodeBlockNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(ReturnNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(WhileLoopNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(EdgeDclsNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(VariableAttributeNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(VariableNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(TerminalNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(AddQueryNode node)
        {
            SymbolTable.NotImplementedError(node);
        }

        public override void Visit(VariableDclNode node)
        {
            SymbolTable.EnterSymbol(node.Name, Utilities.FindTypeFromString(node.Type));
        }
    }
}
