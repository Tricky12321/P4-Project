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

        public AbstractNode()
        {

        }

        public void MakeSiblings(AbstractNode node)
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

        public void AdoptChildren(AbstractNode node)
        {

            if (LeftmostChild != null)
            {
                LeftmostChild.MakeSiblings(node);
            }
            else
            {
                LeftmostChild = node;
                //node.Parent = this;

                //AbstractNode nodeY = node.LeftmostSibling;
                //LeftmostChild = nodeY;
                //while (nodeY != null)
                //{
                //    nodeY.Parent = this;
                //    nodeY = nodeY.RightSibling;
                //}
            }
        }
    }
}
