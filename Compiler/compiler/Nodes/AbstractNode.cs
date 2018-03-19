﻿using System;
namespace P4.Project
{
    public class AbstractNode
    {
        AbstractNode Parent;
        AbstractNode LeftmostSibling;
        AbstractNode LeftmostChild;
        AbstractNode RightSibling;

        public AbstractNode()
        {
        }

        public void MakeNode()
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
            if(this.LeftmostChild != null)
            {
                this.LeftmostChild.MakeSiblings(node);
            }
            else
            {
                AbstractNode nodeY = node.LeftmostSibling;
                this.LeftmostChild = nodeY;
                while(nodeY != null)
                {
                    nodeY.Parent = this;
                    nodeY = nodeY.RightSibling;
                }
            }
        }
    }
}
