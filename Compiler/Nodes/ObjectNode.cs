namespace Compiler.Nodes
{
    internal class ObjectTypeNode : AbstractNode
    {
        ObjectType Type;

        public ObjectTypeNode(ObjectType type)
        {
            Type = type;
        }
    }
}