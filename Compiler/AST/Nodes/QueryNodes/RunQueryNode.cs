using System;
namespace Compiler.AST.Nodes.QueryNodes
{
    public class RunQueryNode : AbstractNode
    {
        public string FunctionName;
        public RunQueryNode(int Linenumber, int Charindex) : base(Linenumber, Charindex)
        {
            
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }
    }
}
