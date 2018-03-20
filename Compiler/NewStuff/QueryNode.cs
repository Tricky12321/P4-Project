using System;
namespace Compiler.NewStuff
{
    internal abstract class QueryNode : AbstractNode {
        public AbstractNode Left { get; set; }
        public AbstractNode Right { get; set; }
    }
}
