using System;
using Compiler.AST;
using Compiler.AST.Nodes;


namespace Compiler.AST
{
    public class AstPrettyPrintVisitor : AstVisitorBase
    {
        public string ProgramCode;

        public override void Visit(FunctionNode node)
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

        public override void Visit(FunctionParameterNode node)
        {
            ProgramCode += $"{node.ParameterType} {node.ParameterName}";
        }

        public override void Visit(ProgramNode node)
        {
            Console.WriteLine("ProgramNode");
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            Console.WriteLine("StartNode");
            VisitChildren(node);
        }

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
