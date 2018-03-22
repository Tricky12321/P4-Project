using System;
using Compiler.AST;
using Compiler.AST.Nodes;
using Compiler;

namespace Compiler.AST.Nodes
{
    enum PrimitiveType { BOOL, INT, DECIMAL, STRING };
    enum ObjectType { GRAPH, EDGE, VERTEX };

    public class AbstractNode
    {
        public AbstractNode Parent;
        public AbstractNode LeftmostSibling;
        public AbstractNode LeftmostChild;
        public AbstractNode RightSibling;
        public int LineNumber;

        public string Name;
        public int ChildCount;
        public AbstractNode()
        {

        }

        /*public virtual void Visit(AstVisitor astVisitor)
        {
            astVisitor.Visit(this);
        }*/

        public void MakeSiblings(AbstractNode node)
        {
            if (node != null)
            {
                AbstractNode nodeX = this;
                while (nodeX.RightSibling != null)
                {
                    nodeX = nodeX.RightSibling;
                }
                AbstractNode nodeY = node.LeftmostSibling;
                nodeX.RightSibling = nodeY;
                nodeY.LeftmostSibling = nodeX.LeftmostSibling;
                nodeY.Parent = nodeX.Parent;
                while (nodeY.RightSibling != null)
                {
                    nodeY = nodeY.RightSibling;
                    nodeY.LeftmostSibling = nodeX.LeftmostSibling;
                    nodeY.Parent = nodeX.Parent;
                }
            }
        }

        public void AdoptChildren(AbstractNode node)
        {
            if (this.LeftmostChild != null)
            {
                ChildCount++;
                LeftmostChild.MakeSiblings(node);
            }
            else
            {
                AbstractNode nodeY = node.LeftmostSibling;
                this.LeftmostChild = nodeY;
                while (nodeY != null)
                {
                    nodeY.Parent = this;
                    nodeY = nodeY.RightSibling;
                }
            }
        }
    }
}
