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
        //int LineNumber;
        public string Name;
        public int ChildCount;
        public AbstractNode()
        {

        }

<<<<<<< HEAD
=======
        public virtual void Accept(AstVisitor astVisitor)
        {
            astVisitor.Visit(this);
        }

>>>>>>> 6ac854b... sdfdsfsdfdsfjdslfjsdlkjflksjflkjdlfkjdsljflksdjflkjdslfkjldskjf  Please enter the commit message for your changes. Lines starting
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
<<<<<<< HEAD
                node.Parent = this;
                ChildCount++;
                if (LeftmostChild == null)
                {
                    LeftmostChild = node;
                }
                else
                {
                    MakeSiblings(node);
=======
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
>>>>>>> 6ac854b... sdfdsfsdfdsfjdslfjsdlkjflksjflkjdlfkjdsljflksdjflkjdslfkjldskjf  Please enter the commit message for your changes. Lines starting
                }
            }
        }
    }
}
