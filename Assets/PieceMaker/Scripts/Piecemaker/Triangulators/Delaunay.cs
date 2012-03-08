using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace MeshUtilities.Triangulators
{
	[Triangulator("Delaunay")]
	public class DelaunayTriangulator : Triangulator
	{
		public override bool FillEdges(List<VertexData> inoutVertices, 
		                      List<Triangle> outTriangles, 
		                      List<MeshUtilities.Edge> inEdges, 
		                      VertexDeclaration vertexDeclaration, 
		                      Vector3 planeNormal, 
		                      Vector3 planePosition, 
		                      SliceAreaSettings sliceAreaSettings)
		{
			var vertices = inEdges.SelectMany(e => new VertexData[] { inoutVertices[e.i1], inoutVertices[e.i2] })
				.Distinct()
				.Select(v => 
					        {
								var clone = v.Clone();
								clone.Normal = -planeNormal;
								return clone;
							})
				.Distinct()
				.ToList();
			
			int nv = vertices.Count;
			if (nv < 3)
				return false;
			int trimax = 4 * nv;
			
			var planeOrthMatrix = Helper.Calc2DPointMatrix(planeNormal, planePosition);
			var vertices2D = vertices.Select(v => Helper.Calc2DPoint(v.Position, planeOrthMatrix)).ToList();
			
			// Find the maximum and minimum vertex bounds.
			// This is to allow calculation of the bounding supertriangle
			double xmin = vertices2D[0].x;
			double ymin = vertices2D[0].y;
			double xmax = xmin;
			double ymax = ymin;
			for (int i = 1; i < nv; i++)
			{
				if (vertices2D[i].x < xmin) xmin = vertices2D[i].x;
				if (vertices2D[i].x > xmax) xmax = vertices2D[i].x;
				if (vertices2D[i].y < ymin) ymin = vertices2D[i].y;
				if (vertices2D[i].y > ymax) ymax = vertices2D[i].y;
			}
	
			double dx = xmax - xmin;
			double dy = ymax - ymin;
			double dmax = (dx > dy) ? dx : dy;
	
			double xmid = (xmax + xmin) * 0.5;
			double ymid = (ymax + ymin) * 0.5;
			
			// Set up the supertriangle
			// This is a triangle which encompasses all the sample points.
			// The supertriangle coordinates are added to the end of the
			// vertex list. The supertriangle is the first triangle in
			// the triangle list.
			vertices2D.Add(new Vector2((float)(xmid - 2 * dmax), 	(float)(ymid - dmax)));
			vertices2D.Add(new Vector2((float)(xmid), 				(float)(ymid + 2 * dmax)));
			vertices2D.Add(new Vector2((float)(xmid + 2 * dmax), 	(float)(ymid - dmax)));
			
			var triangles = new  List<Triangle>();
			var superTriangle = new Triangle(nv, nv + 1, nv + 2);
			triangles.Add(superTriangle);
			
			// Include each point one at a time into the existing mesh
			for (int i = 0; i < nv; i++)
			{
				var edges = new List<Edge>(); //[trimax * 3];
				// Set up the edge buffer.
				// If the point (Vertex(i).x,Vertex(i).y) lies inside the circumcircle then the
				// three edges of that triangle are added to the edge buffer and the triangle is removed from list.
				for (int j = 0; j < triangles.Count; j++)
				{			
					if (Helper.InCircle(vertices2D[i], vertices2D[triangles[j].i1], vertices2D[triangles[j].i2], vertices2D[triangles[j].i3]))
					{
						edges.Add(new Edge(triangles[j].i1, triangles[j].i2));
						edges.Add(new Edge(triangles[j].i2, triangles[j].i3));
						edges.Add(new Edge(triangles[j].i3, triangles[j].i1));
						triangles.RemoveAt(j);
						j--;
					}
				}
				if (i >= nv) continue; //In case we the last duplicate point we removed was the last in the array
	
				// Remove duplicate edges
				// Note: if all triangles are specified anticlockwise then all
				// interior edges are opposite pointing in direction.
				for (int j = edges.Count - 2; j >= 0; j--)
				{
					for (int k = edges.Count - 1; k >= j + 1; k--)
					{
						if (edges[j].Equals(edges[k]))
						{
							edges.RemoveAt(k);
							edges.RemoveAt(j);
							k--;
							continue;
						}
					}
				}
				// Form new triangles for the current point
				// Skipping over any tagged edges.
				// All edges are arranged in clockwise order.
				for (int j = 0; j < edges.Count; j++)
				{
					if (triangles.Count >= trimax)
						throw new ApplicationException("Exceeded maximum edges");
					var newTriangle = new Triangle(edges[j].i1, edges[j].i2, i);
					//newTriangle.SetFacing(vertices, normal);
					triangles.Add(newTriangle);
				}
				edges.Clear();
				edges = null;
			}
			
			// Remove triangles with supertriangle vertices
			// These are triangles which have a vertex number greater than nv
			for (int i = triangles.Count - 1; i >= 0; i--)
			{
				if (triangles[i].i1 >= nv || triangles[i].i2 >= nv || triangles[i].i3 >= nv)
					triangles.RemoveAt(i);
			}
			triangles.TrimExcess();
			
			var baseIndex = inoutVertices.Count;
			inoutVertices.AddRange(vertices);
            var newTriangles = new List<Triangle>();
            foreach (var v in triangles)
			{
				var newTri = new Triangle(baseIndex + v.i1, baseIndex + v.i2, baseIndex + v.i3);
				newTri.SetFacing(inoutVertices, planeNormal);
				outTriangles.Add(newTri);
                newTriangles.Add(new Triangle(newTri.i1 - baseIndex, newTri.i2 - baseIndex, newTri.i3 - baseIndex));
			}
			
			if (sliceAreaSettings.UVMapper != null)
				sliceAreaSettings.UVMapper.Apply(vertices, planeOrthMatrix);

            Helper.RecalculateTangents(vertices, newTriangles);
			
			return true;
		}
	}
}
