using System;
using Compiler.AST;
using Compiler.AST.Nodes;


namespace Compiler.AST
{
    public class AstPrettyPrintVisitor : IAstVisitorBase
    {
        public string ProgramCode;

        public void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node.GetChildren())
            {
                child.Accept(this);
            }
        }

        public void VisitRoot(AbstractNode root)
        {
            root.Accept(this);
        }

        public void Visit(FunctionNode node)
        {
            Console.WriteLine("FunctionNode");
            ProgramCode += $"{node.FunctionName} -> {node.ReturnType}(";
            int i = 0;
            foreach (FunctionParameterNode Param in node.Parameters)
            {
                if (i > 0)
                {
                    ProgramCode += ", ";
                }
                Param.Accept(this);
                i++;
                
            }
            ProgramCode += $")";
            VisitChildren(node);

        }

        public void Visit(FunctionParameterNode node)
        {
            ProgramCode += $"{node.ParameterType} {node.ParameterName}";
        }

        public void Visit(ProgramNode node)
        {
            Console.WriteLine("ProgramNode");
            VisitChildren(node);
        }

        public void Visit(StartNode node)
        {
            Console.WriteLine("StartNode");
            VisitChildren(node);
        }

        public void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
