﻿using System;
using System.Collections.Generic;
namespace Compiler.AST.Nodes
{
    abstract public class AbstractNode
    {
        public AbstractNode Parent;
        public AbstractNode LeftmostSibling;
        public AbstractNode LeftmostChild;
        public AbstractNode RightSibling;
        public int LineNumber;
        public string Name;
        public int ChildCount;
        public int CharIndex;
        public bool HasChildren => LeftmostChild != null;
        public string Type;
        public AllType Type_enum => Utilities.FindTypeFromString(Type);
        private static int statId = 0;
        private int _id;

        public int ID
        {
            get { return _id; }
        }

        public List<AbstractNode> Children
        {
            get
            {
                AbstractNode Child = LeftmostChild;
                List<AbstractNode> ChildrenList = new List<AbstractNode>();
                while (Child != null)
                {
					ChildrenList.Add(Child);
                    Child = Child.RightSibling;
                }
				return ChildrenList;
            }
        }

        public AbstractNode(int LineNumber, int CharIndex)
        {
            this.LineNumber = LineNumber;
            this.CharIndex = CharIndex;
            _id = statId++;
        }

        public abstract void Accept(AstVisitorBase astVisitor);

        public virtual IEnumerable<AbstractNode> GetChildren()
        {
            AbstractNode childNode = LeftmostChild;
            while (childNode != null)
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
                if (!(HasChildren))
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
