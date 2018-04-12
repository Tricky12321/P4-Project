using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public static class Utilities
    {
        public static AllType FindTypeFromString(string Type)
        {
            switch (Type)
            {
                case "void":
                    return AllType.VOID;
                case "string":
                    return AllType.STRING;
                case "bool":
                    return AllType.BOOL;
                case "decimal":
                    return AllType.DECIMAL;
                case "int":
                    return AllType.INT;
                case "graph":
                    return AllType.GRAPH;
                case "edge":
                    return AllType.EDGE;
                case "vertex":
                    return AllType.VERTEX;
                case "collection":
                    return AllType.COLLECTION;
            }
            throw new Exception("Unknown type");
        }
    }
}
