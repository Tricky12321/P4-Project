using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST.Nodes.QueryNodes
{
    public class AbstractExtractNode : AbstractNode
    {
        public string Attribute;
        public string Variable;
        public AbstractNode WhereCondition;
        public AbstractExtractNode(int LineNumber, int CharIndex) : base(LineNumber, CharIndex)
        {
        }

        public override void Accept(AstVisitorBase astVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
