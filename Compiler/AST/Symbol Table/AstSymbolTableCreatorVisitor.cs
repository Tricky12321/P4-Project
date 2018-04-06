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

        public AllType ResolveFuncType(string Type)
        {
            switch (Type)
            {
                case "VOID":
                    return AllType.VOID;
                case "STRING":
                    return AllType.STRING;
                case "BOOL":
                    return AllType.BOOL;
                case "DECIMAL":
                    return AllType.DECIMAL;
                case "INT":
                    return AllType.INT;
                case "GRAPH":
                    return AllType.GRAPH;
                case "EDGE":
                    return AllType.EDGE;
                case "VERTEX":
                    return AllType.VERTEX;
                case "COLLECTION":
                    return AllType.COLLECTION;
            }
            throw new Exception("Unknown type");
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
            Console.WriteLine("This node is visited, but its not implemented!");
            Console.WriteLine(node.ToString());
        }

		public void VisitChildrenNewScope(AbstractNode node)
		{
            SymbolTable.OpenScope();
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);

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
            AllType type = ResolveFuncType(node.ReturnType);
            string functionName = node.Name;
            if (!SymbolTable.DeclaredLocally(functionName)) {
				SymbolTable.EnterSymbol(functionName, type);
				SymbolTable.OpenScope();
				foreach (FunctionParameterNode parameter in node.Parameters)
				{
					parameter.Accept(this);
				}
				VisitChildren(node);
				SymbolTable.CloseScope();
            }
        }

        public override void Visit(FunctionParameterNode node)
        {
            if (!SymbolTable.DeclaredLocally(node.Name)) {
				SymbolTable.SetCurrentNode(node);
				AllType parameterType = ResolveFuncType(node.Type);
				SymbolTable.EnterSymbol(node.Name, parameterType);
            }
        }

        public override void Visit(StartNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ProgramNode node)
        {
            /* To be deleted */
            throw new NotImplementedException();
        }

        public override void Visit(GraphNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string graphName = node.Name;
            SymbolTable.EnterSymbol(graphName, AllType.GRAPH);
            VisitChildren(node);
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
            Console.WriteLine("This node is visited, but its not implemented!");
            Console.WriteLine(node.ToString());
        }

        public override void Visit(WhereNode node)
        {
            VisitChildren(node);
        }

        public override void Visit(ExtendNode node)
        {
            SymbolTable.SetCurrentNode(node);
            string longAttributeName = node.ExtensionName;
            AllType attributeType = ResolveFuncType(node.ExtendWithType);
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
            SymbolTable.OpenScope();
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
            throw new NotImplementedException();
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
                SymbolTable.CloseScope();
            }
            VisitChildren(node.ElseCodeBlock);
        }

        public override void Visit(GraphSetQuery node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(DeclarationNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(BoolComparisonNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ExpressionNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ForLoopNode node)
        {
            if (node.VariableDeclartion != null) {
                Visit(node.VariableDeclartion);
            }
            if (node.VariableDeclartion != null)
            {
                Visit(node.VariableDeclartion);
            }
            VisitChildrenNewScope(node);
        }

        public override void Visit(ForeachLoopNode node)
	    {
            throw new NotImplementedException();
        }
	    public override void Visit(CodeBlockNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(ReturnNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(WhileLoopNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(EdgeDclsNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VariableAttributeNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(VariableNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(TerminalNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(AddQueryNode node)
        {
            throw new NotImplementedException();
        }
    }
}
