﻿using System;
namespace Giraph.Classes
{
    public class Graph
    {
        public GraphCollection<Vertex> _nameVertices = new GraphCollection<Vertex>();
		public GraphCollection<Edge> _nameEdges = new GraphCollection<Edge>();

        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Graph()
        {
            
        }
    }
}
