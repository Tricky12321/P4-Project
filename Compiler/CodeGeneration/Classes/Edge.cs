﻿using System;
namespace Giraph.Classes
{
    public class Edge
    {
		// This ensures, that edges cannot be changed after they are created.
        // But makes it possible to retrive information about them.
		public Vertex _nameFrom => _from;
		public Vertex _nameTo => _to;

		private Vertex _from;
		private Vertex _to;

        //EXTENSIONS HERE
        //*****EXTEND*****
        //EXTENSIONS ENDED
        public Edge(Vertex vertexFrom, Vertex vertexTo)
        {
            _from = vertexFrom;
            _to = vertexTo; 
        }

		public bool disposed = false;

		private void Update() {
			if (_from.Get() == null || _to.Get() == null)
            {
				Console.WriteLine("Edge is now disposed!");
                disposed = true;
            }
		}

        public Edge Get()
        {
			Update();
            if (disposed)
            {
				    
                return null;
            }
            else
            {
                return this;
            }
        }

        // This is to dispose of the edge when it has been removed from a collection or graph.
        // Since edges cannot exist without a graph, this is an insureance that all references die.
        // However this could lead to a lot of null reference exceptions, if not handled correctly
        // Which is not done by the compiler, but expected of the user at this time.
        // this should properbly be changed in the future.... Ask Thue
    }
}
