using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using SerializaahNS;

namespace MeshUtilities
{
	public enum SplitResult
	{
		Intersection,
		AllPositiv,
		AllNegativ
	}
	
	public struct SplitMeshResult
	{
		public bool Success;
		public VirtualMesh PositiveResult;
		public VirtualMesh NegativeResult;
	}
	
	[Flags]
	public enum VertexDeclaration
	{
		None = 0x0,
		Position = 0x1,
		Normal = 0x2,
		Tangent = 0x4,
		UV = 0x8,
		UV2 = 0x10,
		Color = 0x20
	}
		
	public class VertexData : IEquatable<VertexData>
	{
		public UnityEngine.Vector3 Position;
		public UnityEngine.Vector3 Normal;
		public UnityEngine.Vector4 Tangent;
		public UnityEngine.Vector2 UV;
		public UnityEngine.Vector2 UV2;
		public UnityEngine.Color Color;
		
		public bool IsSplitResult;
		
		public VertexData Clone()
		{
			return new VertexData()
			{
				Position = this.Position,
				Normal = this.Normal,
				Tangent = this.Tangent,
				UV = this.UV,
				UV2 = this.UV2,
				Color = this.Color,
				IsSplitResult = this.IsSplitResult
			};
		}
		
		public static VertexData Create(int a, VirtualMesh mesh, VertexDeclaration vertexDeclaration)
		{
			var result = new VertexData()
			{
				Position = (vertexDeclaration & MeshUtilities.VertexDeclaration.Position) != MeshUtilities.VertexDeclaration.None ? mesh.Vertices[a] : UnityEngine.Vector3.zero,
				Normal = (vertexDeclaration & MeshUtilities.VertexDeclaration.Normal) != MeshUtilities.VertexDeclaration.None ? mesh.Normals[a] : UnityEngine.Vector3.zero,
				Tangent = (vertexDeclaration & MeshUtilities.VertexDeclaration.Tangent) != MeshUtilities.VertexDeclaration.None ? mesh.Tangents[a] : UnityEngine.Vector4.zero,
				UV = (vertexDeclaration & MeshUtilities.VertexDeclaration.UV) != MeshUtilities.VertexDeclaration.None ? mesh.UV[a] : UnityEngine.Vector2.zero,
				UV2 = (vertexDeclaration & MeshUtilities.VertexDeclaration.UV2) != MeshUtilities.VertexDeclaration.None ? mesh.UV2[a] : UnityEngine.Vector2.zero,
				Color = (vertexDeclaration & MeshUtilities.VertexDeclaration.Color) != MeshUtilities.VertexDeclaration.None ? mesh.Colors[a] : UnityEngine.Color.black
			};
			return result;
		}
		
		public static VertexData Interpolate(int a, int b, float t1, float t2, VirtualMesh mesh, VertexDeclaration vertexDeclaration)
		{
			var result = new VertexData()
			{
				Position = (vertexDeclaration & MeshUtilities.VertexDeclaration.Position) != MeshUtilities.VertexDeclaration.None ? 
					(t1 * mesh.Vertices[a] 	- t2 * mesh.Vertices[b]) 	/ (t1 - t2) : UnityEngine.Vector3.zero,
				Normal = (vertexDeclaration & MeshUtilities.VertexDeclaration.Normal) != MeshUtilities.VertexDeclaration.None ? 
					UnityEngine.Vector3.Normalize((t1 * mesh.Normals[a] 	- t2 * mesh.Normals[b]) 	/ (t1 - t2)) : UnityEngine.Vector3.zero,
				Tangent = (vertexDeclaration & MeshUtilities.VertexDeclaration.Tangent) != MeshUtilities.VertexDeclaration.None ? 
					UnityEngine.Vector4.Normalize((t1 * mesh.Tangents[a] 	- t2 * mesh.Tangents[b]) 	/ (t1 - t2)) : UnityEngine.Vector4.zero,
				UV = (vertexDeclaration & MeshUtilities.VertexDeclaration.UV) != MeshUtilities.VertexDeclaration.None ? 
					(t1 * mesh.UV[a] 		- t2 * mesh.UV[b]) 			/ (t1 - t2) : UnityEngine.Vector2.zero,
				UV2 = (vertexDeclaration & MeshUtilities.VertexDeclaration.UV2) != MeshUtilities.VertexDeclaration.None ? 
					(t1 * mesh.UV2[a] 		- t2 * mesh.UV2[b]) 		/ (t1 - t2) : UnityEngine.Vector2.zero,
				Color = (vertexDeclaration & MeshUtilities.VertexDeclaration.Color) != MeshUtilities.VertexDeclaration.None ? 
					(t1 * mesh.Colors[a] 	- t2 * mesh.Colors[b]) 		/ (t1 - t2) : UnityEngine.Color.black,
				
				IsSplitResult = true
			};
			
			return result;
		}
		
		#region IEquatable<VertexData> Members
	
		public bool Equals(VertexData other)
		{
			return this.Position == other.Position;
		}
		
		public override int GetHashCode()
		{
		    return this.Position.GetHashCode();
		}
	
		#endregion
	}
	
	public class Triangle 
	{
		Edge[] edges;
		float[] edgeCollapseCost;
		
		public int i1;
		public int i2;
		public int i3;
		
		public Triangle(int a, int b, int c)
		{
			i1 = a;
			i2 = b;
			i3 = c;
		}
		
		public static Triangle Create(VertexData v1, VertexData v2, VertexData v3, List<VertexData> vertices, List<Triangle> triangles, UnityEngine.Vector3 facing, bool correctFacing)
		{
			vertices.Add(v1);
			var i1 = vertices.Count - 1;
		
			vertices.Add(v2);
			var i2 = vertices.Count - 1;
		
			vertices.Add(v3);
			var i3 = vertices.Count - 1;
			
			if (correctFacing && !Helper.IsFacingTowards(v1.Position, v2.Position, v3.Position, facing))
			{
				var tmp = i3;
				i3 = i1;
				i1 = tmp;
			}
			
			var result = new Triangle(i1, i2, i3);
			triangles.Add(result);
			
			return result;
		}
		
		public void SetFacing(List<VertexData> vertices, UnityEngine.Vector3 facing)
		{
			if (!Helper.IsFacingTowards(vertices[i1].Position,
			                            vertices[i2].Position,
			                            vertices[i3].Position,
			                            facing))
			{
				var a = i1;
				var b = i2;
				var c = i3;
				
				i1 = a;
				i2 = c;
				i3 = b;
				
				RecalculateEdges();
			}
		}
		
		public bool HasEdge(Edge edge)
		{
			var cnt = 0;
			if (i1 == edge.i1 || i1 == edge.i2)
				cnt ++;
			if (i2 == edge.i1 || i2 == edge.i2)
				cnt ++;
			if (i3 == edge.i1 || i3 == edge.i2)
				cnt ++;
			return cnt > 1;
		}
		
		public bool HasIndex(int index)
		{
			return i1 == index || i2 == index || i3 == index;
		}
		
		public void ReplaceIndex(int oldIndex, int index)
		{
			if (i1 == oldIndex)
				i1 = index;
			if (i2 == oldIndex)
				i2 = index;
			if (i3 == oldIndex)
				i3 = index;
			RecalculateEdges();
		}
		
		public void RecalculateEdges()
		{
			edges = null;
		}
		
		public Edge[] Edges
		{
			get 
			{
				if (edges == null)
					edges = new Edge[] 
					{
						new Edge(i1, i2),
						new Edge(i2, i3),
						new Edge(i3, i1)	
					};
				return edges;
			}
		}
		
		public void CalculateEdgeCollapseCosts(List<UnityEngine.Vector3> vertices, List<Triangle> triangles)
		{
			EdgeCollapseCosts = Edges.Select(edge => new KeyValuePair<float, Edge>(Tools.ComputeEdgeCollapseCost(edge, vertices, triangles), edge)).ToArray();
		}
		
		public KeyValuePair<float, Edge>[] EdgeCollapseCosts {get; private set; }
	}
	
	public class TriangleComparer : IEqualityComparer<Triangle>
	{
		public bool Equals (Triangle x, Triangle y)
		{
			if (x == y)
				return true;
			if (x == null || y == null)
				return false;
			
			return GetHashCode(x) == GetHashCode(y);
		}
		
		public int GetHashCode (Triangle obj)
		{
			return obj.i1 ^ obj.i2 ^ obj.i3;
		}
	}
	
	public class TriangleEx : Triangle
	{
		public int p1;
		public int p2;
		public int p3;
		public int SubMeshId;
		
		public TriangleEx(int a, int b, int c, int subMeshId, VirtualMesh mesh)
			: base(a, b, c)
		{
			SubMeshId = subMeshId;
			Fill(mesh);
		}
		
		public void Fill(VirtualMesh mesh)
		{
			p1 = mesh.Vertices[i1].GetHashCode();
			p2 = mesh.Vertices[i2].GetHashCode();
			p3 = mesh.Vertices[i3].GetHashCode();
		}
		
		public bool HasSameVertex(IEnumerable<TriangleEx> otherTriangles)
		{
			return otherTriangles.Any(t =>
			{
				return 	t.p1 == p1 || t.p1 == p2 || t.p1 == p3 ||
						t.p2 == p1 || t.p2 == p2 || t.p2 == p3 ||
						t.p3 == p1 || t.p3 == p2 || t.p3 == p3;
			});
		}
	}
	
	public class Edge : IEquatable<Edge>
	{
		public int i1;
		public int i2;
		public Edge(int a, int b)
		{
			if (a > b)
			{
				i1 = b;
				i2 = a;
			}
			else
			{
				i1 = a;
				i2 = b;
			}
		}
		public Edge()
			: this(0, 0)
		{
		}
		
		#region IEquatable<Edge> Members
	
		public bool Equals(Edge other)
		{
			return
				((this.i1 == other.i2) && (this.i2 == other.i1)) ||
				((this.i1 == other.i1) && (this.i2 == other.i2));
		}
		
		public override int GetHashCode()
		{
		    return i1 ^ i2;
		}
		
		public bool HasSameVertex(List<VertexData> vertexData, IEnumerable<Edge> otherEdges)
		{
			return otherEdges.Any(t =>
			{
				return 	vertexData[t.i1].Position == vertexData[i1].Position || vertexData[t.i1].Position == vertexData[i2].Position ||
						vertexData[t.i2].Position == vertexData[i1].Position || vertexData[t.i2].Position == vertexData[i2].Position;
			});
		}
	
		#endregion
	}
	
	public class SliceAreaSettings
	{
		public int SubMeshId = 0;
		
		public UVMapper UVMapper = new UVMappers.PlaneMapper();
		
		public Triangulator Triangulator = new Triangulators.EarClippingTriangulator();
	}
	
	[System.Serializable]
	public class SliceSettings
	{
		[UnityEngine.SerializeField]
		public SliceAreaSettings SliceAreaSettings = new SliceAreaSettings();
	}
	
	[System.Serializable]
	public class MultiSliceSettings : SliceSettings
	{
		[UnityEngine.SerializeField]
		public IEnumerable<UnityEngine.Plane> SlicePlanes;
	}
}