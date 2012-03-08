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
	public class QuickHull3D
	{
		/**
		 * Specifies that (on output) vertex indices for a face should be
		 * listed in clockwise order.
		 */
		public static int CLOCKWISE = 0x1;

		/**
		 * Specifies that (on output) the vertex indices for a face should be
		 * numbered starting from 1.
		 */
		public static int INDEXED_FROM_ONE = 0x2;

		/**
		 * Specifies that (on output) the vertex indices for a face should be
		 * numbered starting from 0.
		 */
		public static int INDEXED_FROM_ZERO = 0x4;

		/**
		 * Specifies that (on output) the vertex indices for a face should be
		 * numbered with respect to the original input points.
		 */
		public static int POINT_RELATIVE = 0x8;

		/**
		 * Specifies that the distance DistanceTolerance should be
		 * computed automatically from the input point data.
		 */
		public static double AUTOMATIC_TOLERANCE = -1;

		protected int findIndex = -1;

		// estimated size of the point set
		protected double charLength;

		protected Vertex[] pointBuffer = new Vertex[0];
		protected int[] vertexPointIndices = new int[0];
		private Face[] discardedFaces = new Face[3];

		private Vertex[] maxVtxs = new Vertex[3];
		private Vertex[] minVtxs = new Vertex[3];

		protected List<Face> faces = new List<Face>(16);
		protected List<HalfEdge> horizon = new List<HalfEdge>(16);

		private FaceList newFaces = new FaceList();
		private VertexList unclaimed = new VertexList();
		private VertexList claimed = new VertexList();

        public int NumVertices { get; private set; }
		protected int numFaces;
		protected int numPoints;

        public double ExplicitDistanceTolerance = AUTOMATIC_TOLERANCE;
		private static double DOUBLE_PREC = 2.2204460492503131e-16;

        public double DistanceTolerance { get; private set; }

		private void AddPointToFace(Vertex vtx, Face face)
		{
			vtx.Face = face;

			if (face.OutsideVertex == null)
				claimed.Add(vtx);
			else
				claimed.InsertBefore(vtx, face.OutsideVertex);

			face.OutsideVertex = vtx;
		}

		private void RemovePointFromFace(Vertex vtx, Face face)
		{
			if (vtx == face.OutsideVertex)
			{
				if (vtx.NextVertex != null && vtx.NextVertex.Face == face)
					face.OutsideVertex = vtx.NextVertex;
				else
					face.OutsideVertex = null;
			}
			claimed.Delete(vtx);
		}

		private Vertex RemoveAllPointsFromFace(Face face)
		{
			if (face.OutsideVertex != null)
			{
				Vertex end = face.OutsideVertex;
				while (end.NextVertex != null && end.NextVertex.Face == face)
					end = end.NextVertex;

				claimed.DeleteChain(face.OutsideVertex, end);
				end.NextVertex = null;
				return face.OutsideVertex;
			}
			else
				return null;
		}

		public QuickHull3D()
		{
		}

		public QuickHull3D(UnityEngine.Vector3[] points)
		{
			Build(points, points.Length);
		}

		private HalfEdge FindHalfEdge(Vertex tail, Vertex head)
		{
			// brute force ... OK, since setHull is not used much
			foreach (var face in faces)
			{
				HalfEdge he = face.FindEdge(tail, head);
				if (he != null)
					return he;
			}
			
			return null;
		}

		protected void SetHull(double[] coords, int nump, int[][] faceIndices, int numf)
		{
			InitBuffers(nump);
			SetPoints(coords, nump);
			ComputeMaxAndMin();
			for (int i = 0; i < numf; i++)
			{
				Face face = Face.Create(pointBuffer, faceIndices[i]);
				HalfEdge he = face.FirstEdge;
				do
				{
					HalfEdge heOpp = FindHalfEdge(he.Head, he.Tail);
					if (heOpp != null)
                        he.SetOppositeHalfEdge(heOpp);
					he = he.NextHalfEdge;
				}
				while (he != face.FirstEdge);
				faces.Add(face);
			}
		}


		public void Build(double[] coords)
		{
			Build(coords, coords.Length / 3);
		}

		public void Build(double[] coords, int nump)
		{
			if (nump < 4)
				throw new ArgumentException("Less than four input points specified");
			if (coords.Length / 3 < nump)
				throw new ArgumentException("Coordinate array too small for specified number of points");

			InitBuffers(nump);
			SetPoints(coords, nump);
			BuildHull();
		}

		public void Build(UnityEngine.Vector3[] points)
		{
			Build(points, points.Length);
		}

		public void Build(UnityEngine.Vector3[] points, int nump)
		{
			if (nump < 4)
				throw new ArgumentException("Less than four input points specified");

			if (points.Length < nump)
				throw new ArgumentException("Point array too small for specified number of points");

			InitBuffers(nump);
			SetPoints(points, nump);
			BuildHull();
		}

		public void Triangulate()
		{
			double minArea = 1000 * charLength * DOUBLE_PREC;
			newFaces.Clear();
			foreach (var face in faces)
			{
                if (face.MarkedAs == Face.MarkType.Visible)
				{
					face.Triangulate(newFaces, minArea);
					// splitFace (face);
				}
			}

			for (Face face = newFaces.First; face != null; face = face.NextFace)
				faces.Add(face);
		}

		protected void InitBuffers(int nump)
		{
			if (pointBuffer.Length < nump)
			{
				Vertex[] newBuffer = new Vertex[nump];
				vertexPointIndices = new int[nump];

				for (int i = 0; i < pointBuffer.Length; i++)
					newBuffer[i] = pointBuffer[i];

				for (int i = pointBuffer.Length; i < nump; i++)
					newBuffer[i] = new Vertex();
				pointBuffer = newBuffer;
			}

			faces.Clear();
			claimed.Clear();
			numFaces = 0;
			numPoints = nump;
		}

		protected void SetPoints(double[] coords, int nump)
		{
			for (int i = 0; i < nump; i++)
			{
				Vertex vtx = pointBuffer[i];
				vtx.Point = new UnityEngine.Vector3((float)(coords[i * 3 + 0]), (float)(coords[i * 3 + 1]), (float)(coords[i * 3 + 2]));
				vtx.Index = i;
			}
		}

		protected void SetPoints(UnityEngine.Vector3[] pnts, int nump)
		{
			for (int i = 0; i < nump; i++)
			{
				Vertex vtx = pointBuffer[i];
				vtx.Point = pnts[i];
				vtx.Index = i;
			}
		}

		protected void ComputeMaxAndMin()
		{
			UnityEngine.Vector3 max = new UnityEngine.Vector3();
			UnityEngine.Vector3 min = new UnityEngine.Vector3();

			for (int i = 0; i < 3; i++)
				maxVtxs[i] = minVtxs[i] = pointBuffer[0];
			
            max = pointBuffer[0].Point;
			min = pointBuffer[0].Point;

			for (int i = 1; i < numPoints; i++)
			{
				UnityEngine.Vector3 pnt = pointBuffer[i].Point;
				if (pnt.x > max.x)
				{
					max.x = pnt.x;
					maxVtxs[0] = pointBuffer[i];
				}
				else if (pnt.x < min.x)
				{
					min.x = pnt.x;
					minVtxs[0] = pointBuffer[i];
				}
				if (pnt.y > max.y)
				{
					max.y = pnt.y;
					maxVtxs[1] = pointBuffer[i];
				}
				else if (pnt.y < min.y)
				{
					min.y = pnt.y;
					minVtxs[1] = pointBuffer[i];
				}
				if (pnt.z > max.z)
				{
					max.z = pnt.z;
					maxVtxs[2] = pointBuffer[i];
				}
				else if (pnt.z < min.z)
				{
					min.z = pnt.z;
					maxVtxs[2] = pointBuffer[i];
				}
			}

			// this epsilon formula comes from QuickHull, and I'm
			// not about to quibble.
			charLength = Math.Max(max.x - min.x, max.y - min.y);
			charLength = Math.Max(max.z - min.z, charLength);
			if (ExplicitDistanceTolerance == AUTOMATIC_TOLERANCE)
			{
				DistanceTolerance = 3 * DOUBLE_PREC * (Math.Max(Math.Abs(max.x), Math.Abs(min.x)) +
					 Math.Max(Math.Abs(max.y), Math.Abs(min.y)) +
					 Math.Max(Math.Abs(max.z), Math.Abs(min.z)));
			}
			else
				DistanceTolerance = ExplicitDistanceTolerance;
		}

		protected void CreateInitialSimplex()
		{
			double max = 0;
			int imax = 0;

			for (int i = 0; i < 3; i++)
			{
				double diff = maxVtxs[i].Point[i] - minVtxs[i].Point[i];
				if (diff > max)
				{
					max = diff;
					imax = i;
				}
			}

			if (max <= DistanceTolerance)
				throw new ArgumentException("Input points appear to be coincident");

			Vertex[] vtx = new Vertex[4];
            // set first two vertices to be those with the greatest
			// one dimensional separation
			vtx[0] = maxVtxs[imax];
			vtx[1] = minVtxs[imax];

			// set third vertex to be the vertex farthest from
			// the line between vtx0 and vtx1
			UnityEngine.Vector3 u01 = new UnityEngine.Vector3();
			UnityEngine.Vector3 diff02 = new UnityEngine.Vector3();
			UnityEngine.Vector3 nrml = new UnityEngine.Vector3();
			UnityEngine.Vector3 xprod = new UnityEngine.Vector3();
			double maxSqr = 0;
			u01 = vtx[1].Point -  vtx[0].Point;
			u01.Normalize();

			for (int i = 0; i < numPoints; i++)
			{
				diff02 = pointBuffer[i].Point - vtx[0].Point;
                xprod = UnityEngine.Vector3.Cross(u01, diff02);
				double lenSqr = xprod.sqrMagnitude;
				if (lenSqr > maxSqr && pointBuffer[i] != vtx[0] && pointBuffer[i] != vtx[1])
				{
					maxSqr = lenSqr;
					vtx[2] = pointBuffer[i];
					nrml = xprod;
				}
			}
			if (Math.Sqrt(maxSqr) <= 100 * DistanceTolerance)
				throw new ArgumentException("Input points appear to be colinear");

			nrml.Normalize();

			double maxDist = 0;
            double d0 = UnityEngine.Vector3.Dot(vtx[2].Point, nrml);
			for (int i = 0; i < numPoints; i++)
			{
                double dist = Math.Abs(UnityEngine.Vector3.Dot(pointBuffer[i].Point, nrml) - d0);
				if (dist > maxDist && pointBuffer[i] != vtx[0] && pointBuffer[i] != vtx[1] && pointBuffer[i] != vtx[2])
				{
					maxDist = dist;
					vtx[3] = pointBuffer[i];
				}
			}
			if (Math.Abs(maxDist) <= 100 * DistanceTolerance)
				throw new ArgumentException("Input points appear to be coplanar");

			Face[] tris = new Face[4];

            if (UnityEngine.Vector3.Dot(vtx[3].Point, nrml) - d0 < 0)
			{
				tris[0] = Face.CreateTriangle(vtx[0], vtx[1], vtx[2]);
				tris[1] = Face.CreateTriangle(vtx[3], vtx[1], vtx[0]);
				tris[2] = Face.CreateTriangle(vtx[3], vtx[2], vtx[1]);
				tris[3] = Face.CreateTriangle(vtx[3], vtx[0], vtx[2]);

				for (int i = 0; i < 3; i++)
				{
					int k = (i + 1) % 3;
                    tris[i + 1].GetEdge(1).SetOppositeHalfEdge(tris[k + 1].GetEdge(0));
                    tris[i + 1].GetEdge(2).SetOppositeHalfEdge(tris[0].GetEdge(k));
				}
			}
			else
			{
				tris[0] = Face.CreateTriangle(vtx[0], vtx[2], vtx[1]);
				tris[1] = Face.CreateTriangle(vtx[3], vtx[0], vtx[1]);
				tris[2] = Face.CreateTriangle(vtx[3], vtx[1], vtx[2]);
				tris[3] = Face.CreateTriangle(vtx[3], vtx[2], vtx[0]);

				for (int i = 0; i < 3; i++)
				{
					int k = (i + 1) % 3;
                    tris[i + 1].GetEdge(0).SetOppositeHalfEdge(tris[k + 1].GetEdge(1));
                    tris[i + 1].GetEdge(2).SetOppositeHalfEdge(tris[0].GetEdge((3 - i) % 3));
				}
			}


			for (int i = 0; i < 4; i++)
				faces.Add(tris[i]);

			for (int i = 0; i < numPoints; i++)
			{
				Vertex v = pointBuffer[i];

				if (v == vtx[0] || v == vtx[1] || v == vtx[2] || v == vtx[3])
					continue;

				maxDist = DistanceTolerance;
				Face maxFace = null;
				for (int k = 0; k < 4; k++)
				{
					double dist = tris[k].DistanceToPlane(v.Point);
					if (dist > maxDist)
					{
						maxFace = tris[k];
						maxDist = dist;
					}
				}
				if (maxFace != null)
					AddPointToFace(v, maxFace);
			}
		}

		public UnityEngine.Vector3[] Vertices
		{
            get
            {
                UnityEngine.Vector3[] vtxs = new UnityEngine.Vector3[NumVertices];
                for (int i = 0; i < NumVertices; i++)
                    vtxs[i] = pointBuffer[vertexPointIndices[i]].Point;
                return vtxs;
            }
		}


		/**
		 * Returns an array specifing the index of each hull vertex
		 * with respect to the original input points.
		 *
		 * @return vertex indices with respect to the original points
		 */
		public int[] VertexPointIndices
		{
            get
            {
                int[] indices = new int[NumVertices];
                for (int i = 0; i < NumVertices; i++)
                    indices[i] = vertexPointIndices[i];
                return indices;
            }
		}

		public int NumFaces
		{
            get
            {
                return faces.Count;
            }
		}

		public int[][] Faces
		{
            get
            {
                return GetFaces(0);
            }
		}

		public int[][] GetFaces(int indexFlags)
		{
			int[][] allFaces = new int[faces.Count][];
			int k = 0;

			foreach (var face in faces)
			{
                allFaces[k] = new int[face.NumVertices];
				GetFaceIndices(allFaces[k], face, indexFlags);
				k++;
			}

			return allFaces;
		}

		private void GetFaceIndices(int[] indices, Face face, int flags)
		{
			bool ccw = ((flags & CLOCKWISE) == 0);
			bool indexedFromOne = ((flags & INDEXED_FROM_ONE) != 0);
			bool pointRelative = ((flags & POINT_RELATIVE) != 0);

			HalfEdge hedge = face.FirstEdge;
			int k = 0;

			do
			{
				int idx = hedge.Head.Index;
				if (pointRelative)
					idx = vertexPointIndices[idx];
				if (indexedFromOne)
					idx++;
				indices[k++] = idx;
				hedge = (ccw ? hedge.NextHalfEdge : hedge.PreviousHalfEdge);
			}
			while (hedge != face.FirstEdge && k < 3);
		}

		protected void ResolveUnclaimedPoints(FaceList newFaces)
		{
			Vertex vtxNext = unclaimed.First;
			for (Vertex vtx = vtxNext; vtx != null; vtx = vtxNext)
			{
				vtxNext = vtx.NextVertex;

				double maxDist = DistanceTolerance;
				Face maxFace = null;
				for (Face newFace = newFaces.First; newFace != null; newFace = newFace.NextFace)
				{
                    if (newFace.MarkedAs == Face.MarkType.Visible)
					{
						double dist = newFace.DistanceToPlane(vtx.Point);
						if (dist > maxDist)
						{
							maxDist = dist;
							maxFace = newFace;
						}
						if (maxDist > 1000 * DistanceTolerance)
							break;
					}
				}
				if (maxFace != null)
					AddPointToFace(vtx, maxFace);
			}
		}

		protected void DeleteFacePoints(Face face, Face absorbingFace)
		{
			Vertex faceVtxs = RemoveAllPointsFromFace(face);
			if (faceVtxs != null)
			{
				if (absorbingFace == null)
					unclaimed.AddChain(faceVtxs);
				else
				{
					Vertex vtxNext = faceVtxs;
					for (Vertex vtx = vtxNext; vtx != null; vtx = vtxNext)
					{
						vtxNext = vtx.NextVertex;
						double dist = absorbingFace.DistanceToPlane(vtx.Point);

						if (dist > DistanceTolerance)
							AddPointToFace(vtx, absorbingFace);
						else
							unclaimed.Add(vtx);
					}
				}
			}
		}

		private static int NONCONVEX_WRT_LARGER_FACE = 1;
		private static int NONCONVEX = 2;

		protected double OppFaceDistance(HalfEdge he)
		{
			return he.Face.DistanceToPlane(he.OppositeHalfEdge.Face.Centroid);
		}

		private bool DoAdjacentMerge(Face face, int mergeType)
		{
			HalfEdge hedge = face.FirstEdge;
			bool convex = true;

			do
			{
				Face oppFace = hedge.OppositeFace;
				bool merge = false;
				//double dist1;

				if (mergeType == NONCONVEX)
				{ 
                    // then merge faces if they are definitively non-convex
					if (OppFaceDistance(hedge) > -DistanceTolerance || OppFaceDistance(hedge.OppositeHalfEdge) > -DistanceTolerance)
						merge = true;
				}
				else // mergeType == NONCONVEX_WRT_LARGER_FACE
				{ 
					// merge faces if they are parallel or non-convex
					// wrt to the larger face; otherwise, just mark
					// the face non-convex for the second pass.
					if (face.Area > oppFace.Area)
					{
                        //if ((dist1 = OppFaceDistance(hedge)) > -DistanceTolerance)
                        if (OppFaceDistance(hedge) > -DistanceTolerance)
							merge = true;
						else if (OppFaceDistance(hedge.OppositeHalfEdge) > -DistanceTolerance)
							convex = false;
					}
					else
					{
						if (OppFaceDistance(hedge.OppositeHalfEdge) > -DistanceTolerance)
							merge = true;
						else if (OppFaceDistance(hedge) > -DistanceTolerance)
							convex = false;
					}
				}

				if (merge)
				{
					int numd = face.MergeAdjacentFace(hedge, discardedFaces);
					for (int i = 0; i < numd; i++)
						DeleteFacePoints(discardedFaces[i], face);

					return true;
				}
				hedge = hedge.NextHalfEdge;
			}

			while (hedge != face.FirstEdge);
			if (!convex)
                face.MarkedAs = Face.MarkType.NonConvex;
			return false;
		}

		protected void CalculateHorizon(UnityEngine.Vector3 eyePnt, HalfEdge edge0, Face face, List<HalfEdge> horizon)
		{
			//	   oldFaces.add (face);
			DeleteFacePoints(face, null);
            face.MarkedAs = Face.MarkType.Deleted;
			HalfEdge edge;

			if (edge0 == null)
			{
				edge0 = face.GetEdge(0);
				edge = edge0;
			}
			else
				edge = edge0.NextHalfEdge;
			do
			{
				Face oppFace = edge.OppositeFace;
                if (oppFace.MarkedAs == Face.MarkType.Visible)
				{
					if (oppFace.DistanceToPlane(eyePnt) > DistanceTolerance)
						CalculateHorizon(eyePnt, edge.OppositeHalfEdge, oppFace, horizon);
					else
						horizon.Add(edge);
				}
				edge = edge.NextHalfEdge;
			}
			while (edge != edge0);
		}

		private HalfEdge AddAdjoiningFace(Vertex eyeVtx, HalfEdge he)
		{
			Face face = Face.CreateTriangle(eyeVtx, he.Tail, he.Head);
			faces.Add(face);
            face.GetEdge(-1).SetOppositeHalfEdge(he.OppositeHalfEdge);
			return face.GetEdge(0);
		}

		protected void AddNewFaces(FaceList newFaces, Vertex eyeVtx, List<HalfEdge> horizon)
		{
			newFaces.Clear();

			HalfEdge hedgeSidePrev = null;
			HalfEdge hedgeSideBegin = null;

			foreach (var horizonHe in horizon)
			{
				HalfEdge hedgeSide = AddAdjoiningFace(eyeVtx, horizonHe);
				if (hedgeSidePrev != null)
                    hedgeSide.NextHalfEdge.SetOppositeHalfEdge(hedgeSidePrev);
				else
					hedgeSideBegin = hedgeSide;
				newFaces.Add(hedgeSide.Face);
				hedgeSidePrev = hedgeSide;
			}

            hedgeSideBegin.NextHalfEdge.SetOppositeHalfEdge(hedgeSidePrev);
		}

		protected Vertex NextPointToAdd()
		{
			if (!claimed.IsEmpty)
			{
				Face eyeFace = claimed.First.Face;
				Vertex eyeVtx = null;
				double maxDist = 0;

				for (Vertex vtx = eyeFace.OutsideVertex; vtx != null && vtx.Face == eyeFace; vtx = vtx.NextVertex)
				{
					double dist = eyeFace.DistanceToPlane(vtx.Point);
					if (dist > maxDist)
					{
						maxDist = dist;
						eyeVtx = vtx;
					}
				}
				return eyeVtx;
			}
			else
				return null;
		}

		protected void AddPointToHull(Vertex eyeVtx)
		{
			horizon.Clear();
			unclaimed.Clear();

			RemovePointFromFace(eyeVtx, eyeVtx.Face);
			CalculateHorizon(eyeVtx.Point, null, eyeVtx.Face, horizon);
			newFaces.Clear();
			AddNewFaces(newFaces, eyeVtx, horizon);

			// first merge pass ... merge faces which are non-convex
			// as determined by the larger face
            for (Face face = newFaces.First; face != null; face = face.NextFace)
			{
                if (face.MarkedAs == Face.MarkType.Visible)
				{
					while (DoAdjacentMerge(face, NONCONVEX_WRT_LARGER_FACE));
				}
			}

			// second merge pass ... merge faces which are non-convex
			// wrt either face	     
            for (Face face = newFaces.First; face != null; face = face.NextFace)
			{
                if (face.MarkedAs == Face.MarkType.NonConvex)
				{
					face.MarkedAs = Face.MarkType.Visible;
					while (DoAdjacentMerge(face, NONCONVEX));
				}
			}	
			ResolveUnclaimedPoints(newFaces);
		}

		protected void BuildHull()
		{
			int cnt = 0;
			Vertex eyeVtx;

			ComputeMaxAndMin();
			CreateInitialSimplex();

			while ((eyeVtx = NextPointToAdd()) != null)
			{
				AddPointToHull(eyeVtx);
				cnt++;
			}
			ReindexFacesAndVertices();
		}

		private void MarkFaceVertices(Face face, int mark)
		{
			HalfEdge he0 = face.FirstEdge;
			HalfEdge he = he0;

			do
			{
				he.Head.Index = mark;
				he = he.NextHalfEdge;
			}
			while (he != he0);
		}

		protected void ReindexFacesAndVertices()
		{
			for (int i = 0; i < numPoints; i++)
				pointBuffer[i].Index = -1;

			// remove inactive faces and mark active vertices
			numFaces = 0;
			for (int i = 0; i < faces.Count;)
			{
				Face face = faces[i];
                if (face.MarkedAs != Face.MarkType.Visible)
					faces.RemoveAt(i);
				else
				{
					MarkFaceVertices(face, 0);
					numFaces++;
					i++;
				}
			}

			// reindex vertices
			NumVertices = 0;
			for (int i = 0; i < numPoints; i++)
			{
				Vertex vtx = pointBuffer[i];
				if (vtx.Index <= 0)
				{
					vertexPointIndices[NumVertices] = i;
					vtx.Index = NumVertices++;
				}
			}
		}

		protected bool CheckFaceConvexity(Face face, double tol)
		{
			double dist;
			HalfEdge he = face.FirstEdge;

			do
			{
				face.CheckConsistency();

				// make sure edge is convex
				dist = OppFaceDistance(he);
				if (dist > tol)
					return false;

				dist = OppFaceDistance(he.OppositeHalfEdge);
				if (dist > tol)
					return false;
				if (he.NextHalfEdge.OppositeFace == he.OppositeFace)
					return false;
				he = he.NextHalfEdge;
			}
			while (he != face.FirstEdge);
			return true;
		}

		protected bool CheckFaces(double tol)
		{
			// check edge convexity
			bool convex = true;
			foreach (var face in faces)
                if (face.MarkedAs == Face.MarkType.Visible && !CheckFaceConvexity(face, tol))
					convex = false;
			return convex;
		}

		public bool CheckCorrectness()
		{
			return CheckCorrectness(DistanceTolerance);
		}

		public bool CheckCorrectness(double tol)
		{
			// check to make sure all edges are fully connected
			// and that the edges are convex
			double dist;
			double pointTol = 10 * tol;

			if (!CheckFaces(DistanceTolerance))
				return false;

			// check point inclusion
			for (int i = 0; i < numPoints; i++)
			{
				UnityEngine.Vector3 pnt = pointBuffer[i].Point;
				foreach (var face in faces)
				{
                    if (face.MarkedAs == Face.MarkType.Visible)
					{
						dist = face.DistanceToPlane(pnt);
						if (dist > pointTol)
							return false;
					}
				}
			}
			return true;
		}
	}
}
