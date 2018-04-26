using System;
namespace Compiler.AST.SymbolTable
{
    public enum BlockType
    {
        ForLoop, WhileLoop, ForEachLoop, IfStatement, ElseifStatement, ElseStatement, WhereStatement, PredicateStatement
    }
}
