using UnityEngine;
using System.Linq;
using System.Collections.Generic;
   
namespace MeshUtilities.Triangulators
{
	[Triangulator("EarClipping")]
	public class EarClippingTriangulator : Triangulator
	{
		public override bool FillEdges(List<VertexData> inoutVertices, 
		                      List<Triangle> outTriangles, 
		                      List<MeshUtilities.Edge> inEdges, 
		                      VertexDeclaration vertexDeclaration, 
		                      Vector3 planeNormal, 
		                      Vector3 planePosition, 
		                      SliceAreaSettings sliceAreaSettings)
		{
			List<int> tmpIndices;
			if (!Helper.GenerateOrderedEdges(inoutVertices, inEdges, out tmpIndices))
				return false;
			
			var vertices = tmpIndices.Select(i => inoutVertices[i])
				.Distinct()
				.Select(v => 
					        {
								var clone = v.Clone();
								clone.Normal = -planeNormal;
								return clone;
							})
				.Distinct()
				.ToList();
			
			var planeOrthMatrix = Helper.Calc2DPointMatrix(planeNormal, planePosition);
			var vertices2D = vertices.Select(v => Helper.Calc2DPoint(v.Position, planeOrthMatrix)).ToList();
			
			int n = vertices2D.Count;
			if (n < 3)
				return false;
			
	        var indices = new List<int>();
			
	        int[] V = new int[n];
	        if (Area(vertices2D) > 0) {
	            for (int v = 0; v < n; v++)
	                V[v] = v;
	        }
	        else {
	            for (int v = 0; v < n; v++)
	                V[v] = (n - 1) - v;
	        }
	       
			var finished = false;
	        int nv = n;
	        int count = 2 * nv;
	        for (int m = 0, v = nv - 1; nv > 2; ) {
	            if ((count--) <= 0)
				{
					finished = true;
					break;
				}
	           
	            int u = v;
	            if (nv <= u)
	                u = 0;
	            v = u + 1;
	            if (nv <= v)
	                v = 0;
				int w = v + 1;
	            if (nv <= w)
	                w = 0;
	           
	            if (Snip(vertices2D, u, v, w, nv, V)) {
	                int a, b, c, s, t;
	                a = V[u];
	                b = V[v];
	                c = V[w];
	                indices.Add(a);
	                indices.Add(b);
	                indices.Add(c);
	                m++;
	                for (s = v, t = v + 1; t < nv; s++, t++)
	                    V[s] = V[t];
	                nv--;
	                count = 2 * nv;
	            }
	        }
			if (!finished)
	       		indices.Reverse();
			
            var newTriangles = new List<Triangle>();
			var baseIndex = inoutVertices.Count;
			inoutVertices.AddRange(vertices);
			for (int i = 0; i < indices.Count / 3; ++i)
			{
				var newTri = new Triangle(baseIndex + indices[i * 3 + 0], 
				                          baseIndex + indices[i * 3 + 1], 
				                          baseIndex + indices[i * 3 + 2]);
				newTri.SetFacing(inoutVertices, planeNormal);
				outTriangles.Add(newTri);

                newTriangles.Add(new Triangle(newTri.i1 - baseIndex, newTri.i2 - baseIndex, newTri.i3 - baseIndex));
			}
			
			if (sliceAreaSettings.UVMapper != null)
				sliceAreaSettings.UVMapper.Apply(vertices, planeOrthMatrix);

            Helper.RecalculateTangents(vertices, newTriangles);
			
			return true;
	    }
	   
	    private float Area (List<Vector2> vertices)
		{
	        int n = vertices.Count;
	        float A = 0.0f;
	        for (int p = n - 1, q = 0; q < n; p = q++) {
	            Vector2 pval = vertices[p];
	            Vector2 qval = vertices[q];
	            A += pval.x * qval.y - qval.x * pval.y;
	        }
	        return (A * 0.5f);
	    }
	   
	    private bool Snip(List<Vector2> vertices, int u, int v, int w, int n, int[] V)
		{
	        int p;
	        var A = vertices[V[u]];
	        var B = vertices[V[v]];
	        var C = vertices[V[w]];
	        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
	            return false;
	        for (p = 0; p < n; p++)
			{
	            if ((p == u) || (p == v) || (p == w))
	                continue;
	            var P = vertices[V[p]];
	            if (InsideTriangle(A, B, C, P))
	                return false;
	        }
	        return true;
	    }
	   
	    private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
	        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
	        float cCROSSap, bCROSScp, aCROSSbp;
	       
	        ax = C.x - B.x; ay = C.y - B.y;
	        bx = A.x - C.x; by = A.y - C.y;
	        cx = B.x - A.x; cy = B.y - A.y;
	        apx = P.x - A.x; apy = P.y - A.y;
	        bpx = P.x - B.x; bpy = P.y - B.y;
	        cpx = P.x - C.x; cpy = P.y - C.y;
	       
	        aCROSSbp = ax * bpy - ay * bpx;
	        cCROSSap = cx * apy - cy * apx;
	        bCROSScp = bx * cpy - by * cpx;
	       
	        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	    }
	}
}