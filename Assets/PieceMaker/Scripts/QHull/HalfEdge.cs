/*
  * Copyright John E. Lloyd, 2003. All rights reserved. Permission
  * to use, copy, and modify, without fee, is granted for non-commercial 
  * and research purposes, provided that this copyright notice appears 
  * in all copies.
  *
  * This  software is distributed "as is", without any warranty, including 
  * any implied warranty of merchantability or fitness for a particular
  * use. The authors assume no responsibility for, and shall not be liable
  * for, any special, indirect, or consequential damages, or any damages
  * whatsoever, arising out of or in connection with the use of this
  * software.
  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QHull
{
	public class HalfEdge
	{
		public Vertex Vertex;
		public Face Face;
        public HalfEdge NextHalfEdge;
		public HalfEdge PreviousHalfEdge;
        public HalfEdge OppositeHalfEdge { get; internal set; }

		public HalfEdge(Vertex v, Face f)
		{
			Vertex = v;
			Face = f;
		}

		public HalfEdge()
		{
		}

		public Vertex Head
		{
            get
            {
			    return Vertex;
            }
		}

		public Vertex Tail
		{
            get
            {
                return PreviousHalfEdge != null ? PreviousHalfEdge.Vertex : null;
            }
		}

		public Face OppositeFace
		{
            get
            {
                return OppositeHalfEdge != null ? OppositeHalfEdge.Face : null;
            }
		}

		public override string ToString()
		{
			if (Tail != null)
			{
				return "" +
					Tail.Index + "-" +
					Head.Index;
			}
			else
				return "?-" + Head.Index;
		}

		public double Length
		{
            get
            {
                if (Tail != null)
                    return UnityEngine.Vector3.Distance(Head.Point, Tail.Point);
                else
                    return -1;
            }
		}

		public double LengthSq
		{
            get
            {
                if (Tail != null)
                {
                    var dist = UnityEngine.Vector3.Distance(Head.Point, Tail.Point);
                    return dist * dist;
                }
                else
                    return -1;
            }
		}

        public void SetOppositeHalfEdge(HalfEdge edge)
        {
            OppositeHalfEdge = edge;
            edge.OppositeHalfEdge = this;
        }
	}
}
