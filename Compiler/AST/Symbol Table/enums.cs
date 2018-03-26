using System;
namespace Compiler.AST.SymbolTable
{
    public enum PrimitiveType { BOOL, INT, DECIMAL, STRING };
    public enum ObjectType { GRAPH, EDGE, VERTEX };
    public enum AllType { BOOL, INT, DECIMAL, STRING, GRAPH, EDGE, VERTEX, VOID }
}
