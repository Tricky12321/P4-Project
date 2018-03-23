using System;
namespace Compiler.AST.Nodes
{
    enum PrimitiveType { BOOL, INT, DECIMAL, STRING };
    enum ObjectType { GRAPH, EDGE, VERTEX };

    public class AbstractNode
    {
        AbstractNode Parent;
        AbstractNode LeftmostSibling;
        AbstractNode LeftmostChild;
        AbstractNode RightSibling;
        int LineNumber;
        public string Name;
        public int ChildCount;
        public AbstractNode(int LineNumber)
        {
            this.LineNumber = LineNumber;
        }

        public void MakeSiblings(AbstractNode node)
        {
            AbstractNode RightMostChild = LeftmostChild;
            AbstractNode NextChild = LeftmostChild.RightSibling;
            while (NextChild != null)
            {
				RightMostChild = NextChild;
                NextChild = NextChild.RightSibling;
            }

            RightMostChild.RightSibling = node;
            node.LeftmostSibling = LeftmostChild;
        }

        public void AdoptChildren(AbstractNode node)
        {
            if (node != null)
            {
                node.Parent = this;
                ChildCount++;
                if (LeftmostChild == null)
                {
                    LeftmostChild = node;
                }
                else
                {
                    MakeSiblings(node);
                }
            }
        }
    }
}
