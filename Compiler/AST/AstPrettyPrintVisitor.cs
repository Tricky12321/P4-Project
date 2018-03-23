using System;
using Compiler.AST;
using Compiler.AST.Nodes;


namespace Compiler.AST
{
    public class AstPrettyPrintVisitor : AstVisitorBase
    {
        public string ProgramCode;
        int _currentLineNumber;

        public override void VisitRoot(AbstractNode root)
        {
            _currentLineNumber = root.LineNumber;
            base.VisitRoot(root);
        }

        private void CheckNewLine(int lineNumber)
        {
            if (_currentLineNumber != lineNumber)
            {
                ProgramCode += "\n";
                _currentLineNumber = lineNumber;
            }
        }

        public override void Visit(FunctionNode node)
        {
            CheckNewLine(node.LineNumber);
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
            ProgramCode += ")\n{\n";
            VisitChildren(node);
            ProgramCode += "\n}\n";
        }

        public override void Visit(FunctionParameterNode node)
        {
            CheckNewLine(node.LineNumber);

            ProgramCode += $"{node.ParameterType} {node.ParameterName}";
        }

        public override void Visit(ProgramNode node)
        {
            CheckNewLine(node.LineNumber);

            Console.WriteLine("ProgramNode");
            VisitChildren(node);
        }

        public override void Visit(StartNode node)
        {
            CheckNewLine(node.LineNumber);

            Console.WriteLine("StartNode");
            VisitChildren(node);
        }

        public override void Visit(GraphNode node)
        {
            ProgramCode += $"GRAPH {node.Name}";
        }

        public override void Visit(AbstractNode node)
        {
            throw new NotImplementedException();
        }
    }
}
