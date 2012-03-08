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
	public class FaceList
	{
		private Face head;
		private Face tail;

		public void Clear()
		{
			head = tail = null;
		}

		public void Add(Face vtx)
		{
			if (head == null)
				head = vtx;
			else
				tail.NextFace = vtx;
			vtx.NextFace = null;
			tail = vtx;
		}

		public Face First
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
