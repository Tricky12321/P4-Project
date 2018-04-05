using System;
namespace Compiler
{
    public enum PrimitiveType { BOOL, INT, DECIMAL, STRING };
    public enum ObjectType { GRAPH, EDGE, VERTEX };
    public enum AllType { BOOL, INT, DECIMAL, STRING, GRAPH, EDGE, VERTEX, VOID, COLLECTION };
    public enum ExpressionPartType { BOOL, INT, DECIMAL, STRING, OPERATOR, ADVANCED_OPERATOR, VARIABLE, ATTRIBUTE, QUERYTYPE };
}
