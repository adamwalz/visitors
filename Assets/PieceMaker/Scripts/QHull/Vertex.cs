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
	public class Vertex
	{
		public UnityEngine.Vector3 Point;
        public int Index;
		public Vertex PreviousVertex;
		public Vertex NextVertex;
		public Face Face;

		public Vertex()
		{
			Point = new UnityEngine.Vector3();
		}

		public Vertex(double x, double y, double z, int idx)
		{
			Point = new UnityEngine.Vector3((float)x, (float)y, (float)z);
			Index = idx;
		}
	}
}
