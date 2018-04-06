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
                case "VOID":
                    return AllType.VOID;
                case "STRING":
                    return AllType.STRING;
                case "BOOL":
                    return AllType.BOOL;
                case "DECIMAL":
                    return AllType.DECIMAL;
                case "INT":
                    return AllType.INT;
                case "GRAPH":
                    return AllType.GRAPH;
                case "EDGE":
                    return AllType.EDGE;
                case "VERTEX":
                    return AllType.VERTEX;
                case "COLLECTION":
                    return AllType.COLLECTION;
            }
            throw new Exception("Unknown type");
        }
    }
}
