using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    

    public class AbstractNode
    {
        public AbstractNode Parent;
        public AbstractNode LeftmostSibling;
        public AbstractNode LeftmostChild;
        public AbstractNode RightSibling;
        public int LineNumber;
        public string Name;
        public int ChildCount;

        public AbstractNode(int LineNumber)
        {
            this.LineNumber = LineNumber;
        }

        public virtual void Accept(AstVisitorBase astVisitor)
        {
            astVisitor.Visit(this);
        }

        public virtual IEnumerable<AbstractNode> GetChildren()
        {
            AbstractNode childNode = LeftmostChild;
            while(childNode != null)
            {
                yield return childNode;
                childNode = childNode.RightSibling;
            }
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
