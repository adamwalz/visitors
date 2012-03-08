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
	public class Face
	{
        public enum MarkType
        {
            Visible = 1,
            NonConvex = 2,
            Deleted = 3
        }

        public UnityEngine.Vector3 Centroid { get; private set; }
        public UnityEngine.Vector3 Normal { get; private set; }
        public HalfEdge FirstEdge { get; private set; }
        public double Area { get; private set; }
        public double PlaneOffset { get; private set; }
        public int Index { get; private set; }
        public int NumVertices { get; private set; }

        public Face NextFace;

        public MarkType MarkedAs = MarkType.Visible;

		public Vertex OutsideVertex;

		public void ComputeCentroid()
		{
			Centroid = new UnityEngine.Vector3();
			HalfEdge he = FirstEdge;
			do
			{
				Centroid += he.Head.Point;
				he = he.NextHalfEdge;
			}
			while (he != FirstEdge);
			Centroid *= (float)(1 / (double)NumVertices);
		}

		public void ComputeNormal(double minArea)
		{
			ComputeNormal();

			if (Area < minArea)
			{
				// make the normal more robust by removing
				// components parallel to the longest edge

				HalfEdge hedgeMax = null;
				double lenSqrMax = 0;
				HalfEdge hedge = FirstEdge;
				do
				{
					double lenSqr = hedge.LengthSq;
					if (lenSqr > lenSqrMax)
					{
						hedgeMax = hedge;
						lenSqrMax = lenSqr;
					}
					hedge = hedge.NextHalfEdge;
				}
				while (hedge != FirstEdge);

				UnityEngine.Vector3 p2 = hedgeMax.Head.Point;
				UnityEngine.Vector3 p1 = hedgeMax.Tail.Point;
				double lenMax = Math.Sqrt(lenSqrMax);
				double ux = (p2.x - p1.x) / lenMax;
				double uy = (p2.y - p1.y) / lenMax;
				double uz = (p2.z - p1.z) / lenMax;
				double dot = Normal.x * ux + Normal.y * uy + Normal.z * uz;
                Normal -= new UnityEngine.Vector3((float)(dot * ux), (float)(dot * uy), (float)(dot * uz));
				Normal.Normalize();
			}
		}

		public void ComputeNormal()
		{
			HalfEdge he1 = FirstEdge.NextHalfEdge;
			HalfEdge he2 = he1.NextHalfEdge;

			UnityEngine.Vector3 p0 = FirstEdge.Head.Point;
			UnityEngine.Vector3 p2 = he1.Head.Point;

			double d2x = p2.x - p0.x;
			double d2y = p2.y - p0.y;
			double d2z = p2.z - p0.z;

			Normal = new UnityEngine.Vector3();

			NumVertices = 2;

			while (he2 != FirstEdge)
			{
				double d1x = d2x;
				double d1y = d2y;
				double d1z = d2z;

				p2 = he2.Head.Point;
				d2x = p2.x - p0.x;
				d2y = p2.y - p0.y;
				d2z = p2.z - p0.z;

                Normal += new UnityEngine.Vector3((float)(d1y * d2z - d1z * d2y), (float)(d1z * d2x - d1x * d2z), (float)(d1x * d2y - d1y * d2x));

				he1 = he2;
				he2 = he2.NextHalfEdge;
				NumVertices++;
			}
			Area = Normal.magnitude;
			Normal *= (float)(1 / Area);
		}

		private void ComputeNormalAndCentroid()
		{
			ComputeNormal();
			ComputeCentroid();

            PlaneOffset = UnityEngine.Vector3.Dot(Normal, Centroid);
			int numv = 0;
			HalfEdge he = FirstEdge;
			do
			{
				numv++;
				he = he.NextHalfEdge;
			}
			while (he != FirstEdge);

			if (numv != NumVertices)
				throw new Exception("face " + ToString() + " numVerts=" + NumVertices + " should be " + numv);
		}

		private void ComputeNormalAndCentroid(double minArea)
		{
			ComputeNormal(minArea);
			ComputeCentroid();
            PlaneOffset = UnityEngine.Vector3.Dot(Normal, Centroid);
		}

		public static Face CreateTriangle(Vertex v0, Vertex v1, Vertex v2)
		{
			return CreateTriangle(v0, v1, v2, 0);
		}

		public static Face CreateTriangle(Vertex v0, Vertex v1, Vertex v2, double minArea)
		{
			Face face = new Face();
			HalfEdge he0 = new HalfEdge(v0, face);
			HalfEdge he1 = new HalfEdge(v1, face);
			HalfEdge he2 = new HalfEdge(v2, face);

			he0.PreviousHalfEdge = he2;
			he0.NextHalfEdge = he1;
			he1.PreviousHalfEdge = he0;
			he1.NextHalfEdge = he2;
			he2.PreviousHalfEdge = he1;
			he2.NextHalfEdge = he0;

			face.FirstEdge = he0;

			// compute the normal and offset
			face.ComputeNormalAndCentroid(minArea);
			return face;
		}

		public static Face Create(Vertex[] vtxArray, int[] indices)
		{
			Face face = new Face();
			HalfEdge hePrev = null;
			for (int i = 0; i < indices.Length; i++)
			{
				HalfEdge he = new HalfEdge(vtxArray[indices[i]], face);
				if (hePrev != null)
				{
					he.PreviousHalfEdge = hePrev;
					hePrev.NextHalfEdge = he;
				}
				else
					face.FirstEdge = he;
				hePrev = he;
			}
            face.FirstEdge.PreviousHalfEdge = hePrev;
            hePrev.NextHalfEdge = face.FirstEdge;

			// compute the normal and offset
			face.ComputeNormalAndCentroid();
			return face;
		}

		public Face()
		{
			Normal = new UnityEngine.Vector3();
			Centroid = new UnityEngine.Vector3();
            MarkedAs = Face.MarkType.Visible;
		}

		public HalfEdge GetEdge(int i)
		{
			HalfEdge he = FirstEdge;
			while (i > 0)
			{
				he = he.NextHalfEdge;
				i--;
			}
			while (i < 0)
			{
				he = he.PreviousHalfEdge;
				i++;
			}
			return he;
		}


		public HalfEdge FindEdge(Vertex vt, Vertex vh)
		{
			HalfEdge he = FirstEdge;
			do
			{
				if (he.Head == vh && he.Tail == vt)
					return he;
				he = he.NextHalfEdge;
			}
			while (he != FirstEdge);

			return null;
		}

		public double DistanceToPlane(UnityEngine.Vector3 p)
		{
			return Normal.x * p.x + Normal.y * p.y + Normal.z * p.z - PlaneOffset;
		}

        public override string ToString()
        {
            String s = null;
            HalfEdge he = FirstEdge;
            do
            {
                if (s == null)
                    s = "" + he.Head.Index;
                else
                    s += " " + he.Head.Index;
                he = he.NextHalfEdge;
            }
            while (he != FirstEdge);
            return s;
        }

		public void GetVertexIndices(int[] idxs)
		{
			HalfEdge he = FirstEdge;
			int i = 0;
			do
			{
				idxs[i++] = he.Head.Index;
				he = he.NextHalfEdge;
			}
			while (he != FirstEdge);
		}

		private Face ConnectHalfEdges(HalfEdge hedgePrev, HalfEdge hedge)
		{
			Face discardedFace = null;

			if (hedgePrev.OppositeFace == hedge.OppositeFace)
			{ // then there is a redundant edge that we can get rid off

				Face oppFace = hedge.OppositeFace;
				HalfEdge hedgeOpp;

				if (hedgePrev == FirstEdge)
					FirstEdge = hedge;
                if (oppFace.NumVertices == 3)
				{ // then we can get rid of the opposite face altogether
                    hedgeOpp = hedge.OppositeHalfEdge.PreviousHalfEdge.OppositeHalfEdge;

                    oppFace.MarkedAs = Face.MarkType.Deleted;
					discardedFace = oppFace;
				}
				else
				{
                    hedgeOpp = hedge.OppositeHalfEdge.NextHalfEdge;

					if (oppFace.FirstEdge == hedgeOpp.PreviousHalfEdge)
						oppFace.FirstEdge = hedgeOpp;
					hedgeOpp.PreviousHalfEdge = hedgeOpp.PreviousHalfEdge.PreviousHalfEdge;
					hedgeOpp.PreviousHalfEdge.NextHalfEdge = hedgeOpp;
				}
				hedge.PreviousHalfEdge = hedgePrev.PreviousHalfEdge;
				hedge.PreviousHalfEdge.NextHalfEdge = hedge;

				hedge.OppositeHalfEdge = hedgeOpp;
				hedgeOpp.OppositeHalfEdge = hedge;

				// oppFace was modified, so need to recompute
				oppFace.ComputeNormalAndCentroid();
			}
			else
			{
				hedgePrev.NextHalfEdge = hedge;
				hedge.PreviousHalfEdge = hedgePrev;
			}
			return discardedFace;
		}

		public void CheckConsistency()
		{
			// do a sanity check on the face
			HalfEdge hedge = FirstEdge;
			double maxd = 0;
			int numv = 0;

			if (NumVertices < 3)
				throw new Exception("degenerate face: " + ToString());
			do
			{
                HalfEdge hedgeOpp = hedge.OppositeHalfEdge;
				if (hedgeOpp == null)
				{
					throw new Exception(
                        "face " + ToString() + ": " +
						"unreflected half edge " + hedge.ToString());
				}
				else if (hedgeOpp.OppositeHalfEdge != hedge)
				{
					throw new Exception(
                        "face " + ToString() + ": " +
						"opposite half edge " + hedgeOpp.ToString() +
						" has opposite " +
						hedgeOpp.OppositeHalfEdge.ToString());
				}
				if (hedgeOpp.Head != hedge.Tail ||
				hedge.Head != hedgeOpp.Tail)
				{
					throw new Exception(
                        "face " + ToString() + ": " +
						"half edge " + hedge.ToString() +
						" reflected by " + hedgeOpp.ToString());
				}
				Face oppFace = hedgeOpp.Face;
				if (oppFace == null)
				{
					throw new Exception(
                        "face " + ToString() + ": " +
						"no face on half edge " + hedgeOpp.ToString());
				}
                else if (oppFace.MarkedAs == Face.MarkType.Deleted)
				{
					throw new Exception(
                        "face " + ToString() + ": " +
                        "opposite face " + oppFace.ToString() +
						" not on hull");
				}
				double d = Math.Abs(DistanceToPlane(hedge.Head.Point));
				if (d > maxd)
					maxd = d;
				numv++;
				hedge = hedge.NextHalfEdge;
			}
			while (hedge != FirstEdge);

			if (numv != NumVertices)
			{
				throw new Exception(
                    "face " + ToString() + " numVerts=" + NumVertices + " should be " + numv);
			}
		}

		public int MergeAdjacentFace(HalfEdge hedgeAdj, Face[] discarded)
		{
			Face oppFace = hedgeAdj.OppositeFace;
			int numDiscarded = 0;

			discarded[numDiscarded++] = oppFace;
            oppFace.MarkedAs = Face.MarkType.Deleted;

			HalfEdge hedgeOpp = hedgeAdj.OppositeHalfEdge;

			HalfEdge hedgeAdjPrev = hedgeAdj.PreviousHalfEdge;
			HalfEdge hedgeAdjNext = hedgeAdj.NextHalfEdge;
			HalfEdge hedgeOppPrev = hedgeOpp.PreviousHalfEdge;
			HalfEdge hedgeOppNext = hedgeOpp.NextHalfEdge;

			while (hedgeAdjPrev.OppositeFace == oppFace)
			{
				hedgeAdjPrev = hedgeAdjPrev.PreviousHalfEdge;
				hedgeOppNext = hedgeOppNext.NextHalfEdge;
			}

			while (hedgeAdjNext.OppositeFace == oppFace)
			{
				hedgeOppPrev = hedgeOppPrev.PreviousHalfEdge;
				hedgeAdjNext = hedgeAdjNext.NextHalfEdge;
			}

			HalfEdge hedge;

			for (hedge = hedgeOppNext; hedge != hedgeOppPrev.NextHalfEdge; hedge = hedge.NextHalfEdge)
				hedge.Face = this;

			if (hedgeAdj == FirstEdge)
				FirstEdge = hedgeAdjNext;

			// handle the half edges at the head
			Face discardedFace;

			discardedFace = ConnectHalfEdges(hedgeOppPrev, hedgeAdjNext);
			if (discardedFace != null)
				discarded[numDiscarded++] = discardedFace;

			// handle the half edges at the tail
			discardedFace = ConnectHalfEdges(hedgeAdjPrev, hedgeOppNext);
			if (discardedFace != null)
				discarded[numDiscarded++] = discardedFace;

			ComputeNormalAndCentroid();
			CheckConsistency();

			return numDiscarded;
		}

		private double CalcAreaSquared(HalfEdge hedge0, HalfEdge hedge1)
		{
			// return the squared area of the triangle defined
			// by the half edge hedge0 and the point at the
			// head of hedge1.

			UnityEngine.Vector3 p0 = hedge0.Tail.Point;
			UnityEngine.Vector3 p1 = hedge0.Head.Point;
			UnityEngine.Vector3 p2 = hedge1.Head.Point;

			double dx1 = p1.x - p0.x;
			double dy1 = p1.y - p0.y;
			double dz1 = p1.z - p0.z;

			double dx2 = p2.x - p0.x;
			double dy2 = p2.y - p0.y;
			double dz2 = p2.z - p0.z;

			double x = dy1 * dz2 - dz1 * dy2;
			double y = dz1 * dx2 - dx1 * dz2;
			double z = dx1 * dy2 - dy1 * dx2;

			return x * x + y * y + z * z;
		}

		public void Triangulate(FaceList newFaces, double minArea)
		{
			HalfEdge hedge;

            if (NumVertices < 4)
				return;

			Vertex v0 = FirstEdge.Head;

			hedge = FirstEdge.NextHalfEdge;
			HalfEdge oppPrev = hedge.OppositeHalfEdge;
			Face face0 = null;

			for (hedge = hedge.NextHalfEdge; hedge != FirstEdge.PreviousHalfEdge; hedge = hedge.NextHalfEdge)
			{
				Face face =
					CreateTriangle(v0, hedge.PreviousHalfEdge.Head, hedge.Head, minArea);
                face.FirstEdge.NextHalfEdge.SetOppositeHalfEdge(oppPrev);
                face.FirstEdge.PreviousHalfEdge.SetOppositeHalfEdge(hedge.OppositeHalfEdge);
				oppPrev = face.FirstEdge;
				newFaces.Add(face);
				if (face0 == null)
					face0 = face;
			}
			hedge = new HalfEdge(FirstEdge.PreviousHalfEdge.PreviousHalfEdge.Head, this);
            hedge.SetOppositeHalfEdge(oppPrev);

			hedge.PreviousHalfEdge = FirstEdge;
			hedge.PreviousHalfEdge.NextHalfEdge = hedge;

			hedge.NextHalfEdge = FirstEdge.PreviousHalfEdge;
			hedge.NextHalfEdge.PreviousHalfEdge = hedge;

			ComputeNormalAndCentroid(minArea);
			CheckConsistency();

			for (Face face = face0; face != null; face = face.NextFace)
				face.CheckConsistency();
		}
	}
}
