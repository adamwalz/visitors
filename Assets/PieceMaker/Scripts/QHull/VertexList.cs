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
	/**
	 * Maintains a double-linked list of vertices for use by QuickHull3D
	 */
	public class VertexList
	{
		private Vertex head;
		private Vertex tail;

		public void Clear()
		{
			head = tail = null;
		}

		public void Add(Vertex vtx)
		{
			if (head == null)
				head = vtx;
			else
				tail.NextVertex = vtx;
			vtx.PreviousVertex = tail;
			vtx.NextVertex = null;
			tail = vtx;
		}

		public void AddChain(Vertex vtx)
		{
			if (head == null)
				head = vtx;
			else
				tail.NextVertex = vtx;

			vtx.PreviousVertex = tail;

			while (vtx.NextVertex != null)
				vtx = vtx.NextVertex;
			tail = vtx;
		}

		public void Delete(Vertex vtx)
		{
			if (vtx.PreviousVertex == null)
				head = vtx.NextVertex;
			else
				vtx.PreviousVertex.NextVertex = vtx.NextVertex;
			if (vtx.NextVertex == null)
				tail = vtx.PreviousVertex;
			else
				vtx.NextVertex.PreviousVertex = vtx.PreviousVertex;
		}

		public void DeleteChain(Vertex vtx1, Vertex vtx2)
		{
			if (vtx1.PreviousVertex == null)
				head = vtx2.NextVertex;
			else
				vtx1.PreviousVertex.NextVertex = vtx2.NextVertex;
			if (vtx2.NextVertex == null)
				tail = vtx1.PreviousVertex;
			else
				vtx2.NextVertex.PreviousVertex = vtx1.PreviousVertex;
		}

		public void InsertBefore(Vertex vtx, Vertex next)
		{
			vtx.PreviousVertex = next.PreviousVertex;
			if (next.PreviousVertex == null)
				head = vtx;
			else
				next.PreviousVertex.NextVertex = vtx;
			vtx.NextVertex = next;
			next.PreviousVertex = vtx;
		}

		public Vertex First
		{
            get
            {
			    return head;
            }
		}

		public bool IsEmpty
		{
            get
            {
                return head == null;
            }
		}
	}
}
