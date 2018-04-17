﻿using System;
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
            if (Type != null)
            {
                Type = Type.ToLower();
            }
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

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 6) || (p == 128) || Environment.OSVersion.ToString().ToLower().Contains("linux");
            }
        }

        public static bool IsMacOS
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || Environment.OSVersion.ToString().ToLower().Contains("unix");
            }
        }

        public static bool IsWindows
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 2) || Environment.OSVersion.ToString().ToLower().Contains("windows");
            }
        }
    }
}
