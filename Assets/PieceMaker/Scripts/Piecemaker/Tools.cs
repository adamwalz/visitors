using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace MeshUtilities
{
	public static class Tools
	{		
		public static SplitMeshResult SplitMesh(UnityEngine.Plane cutPlane, VirtualMesh mesh, SliceSettings sliceSettings)
		{
			var vertexDeclaration = Helper.GetVertexDeclaration(mesh);
			
			var cutPlanePos = -cutPlane.normal * cutPlane.distance;
			var cutPlaneNormal = cutPlane.normal;
			
			var positiveVertexData = new List<VertexData>();
			var positiveCutEdgeData = new List<MeshUtilities.Edge>();
			var positiveTriangles = new List<Triangle>();
			
			var negativeVertexData = new List<VertexData>();
			var negativeCutEdgeData = new List<MeshUtilities.Edge>();
			var negativeTriangles = new List<Triangle>();
			
			var triangulator = sliceSettings.SliceAreaSettings.Triangulator;
			
			var positiveMeshToTriangle = new List<KeyValuePair<int, List<Triangle>>>();
			var negativeMeshToTriangle = new List<KeyValuePair<int, List<Triangle>>>();
			
			// main Mesh
			for (int i = 0; i < mesh.SubMeshCount; ++i)
			{
				SplitMesh(mesh, cutPlanePos, cutPlaneNormal, mesh.GetTriangles(i), 
				          positiveVertexData, positiveTriangles, positiveCutEdgeData, 
				          negativeVertexData, negativeTriangles, negativeCutEdgeData, 
				          vertexDeclaration);
				
				positiveMeshToTriangle.Add(new KeyValuePair<int, List<Triangle>>(i, new List<Triangle>(positiveTriangles)));
				negativeMeshToTriangle.Add(new KeyValuePair<int, List<Triangle>>(i, new List<Triangle>(negativeTriangles)));
			}
			
			positiveTriangles.Clear();
			negativeTriangles.Clear();
			
			var positiveEdgeIslands = Helper.GetEdgeIslands(positiveVertexData, positiveCutEdgeData);
			var negativeEdgeIslands = Helper.GetEdgeIslands(negativeVertexData, negativeCutEdgeData);
			
			if (positiveEdgeIslands.Count > 0)
			{
				foreach (var island in positiveEdgeIslands)
				{
					if (!triangulator.FillEdges(positiveVertexData, positiveTriangles, island, vertexDeclaration, cutPlaneNormal, cutPlanePos, sliceSettings.SliceAreaSettings))
					{
						if (triangulator.GetType() != typeof(Triangulators.DelaunayTriangulator))
						{
							var fallBackTriangulator = new Triangulators.DelaunayTriangulator();
							fallBackTriangulator.FillEdges(positiveVertexData, positiveTriangles, island, vertexDeclaration, cutPlaneNormal, cutPlanePos, sliceSettings.SliceAreaSettings);
						}
					}
				}
				
				var cutData = positiveMeshToTriangle.FirstOrDefault(p => p.Key == sliceSettings.SliceAreaSettings.SubMeshId);
				if (cutData.Value == null)
					positiveMeshToTriangle.Add(new KeyValuePair<int, List<Triangle>>(sliceSettings.SliceAreaSettings.SubMeshId, new List<Triangle>(positiveTriangles)));
				else
					cutData.Value.AddRange(positiveTriangles);
			}
			if (negativeEdgeIslands.Count > 0)
			{
				foreach (var island in negativeEdgeIslands)
				{
					if (!triangulator.FillEdges(negativeVertexData, negativeTriangles, island, vertexDeclaration, -cutPlaneNormal, cutPlanePos, sliceSettings.SliceAreaSettings))
					{
						if (triangulator.GetType() != typeof(Triangulators.DelaunayTriangulator))
						{
							var fallBackTriangulator = new Triangulators.DelaunayTriangulator();
							fallBackTriangulator.FillEdges(negativeVertexData, negativeTriangles, island, vertexDeclaration, -cutPlaneNormal, cutPlanePos, sliceSettings.SliceAreaSettings);
						}
					}
				}
				
				var cutData = negativeMeshToTriangle.FirstOrDefault(p => p.Key == sliceSettings.SliceAreaSettings.SubMeshId);
				if (cutData.Value == null)
					negativeMeshToTriangle.Add(new KeyValuePair<int, List<Triangle>>(sliceSettings.SliceAreaSettings.SubMeshId, new List<Triangle>(negativeTriangles)));
				else
					cutData.Value.AddRange(negativeTriangles);
			}
			
			var result = new SplitMeshResult();
			if (positiveVertexData.Count > 0 && negativeVertexData.Count > 0)
			{
				result.PositiveResult = PreFillMeshResult(positiveVertexData, vertexDeclaration);
				result.NegativeResult = PreFillMeshResult(negativeVertexData, vertexDeclaration);
				
				if (result.PositiveResult != null)
				{
					result.PositiveResult.SubMeshCount = positiveMeshToTriangle.Max(p => p.Key) + 1;
					foreach (var p in positiveMeshToTriangle)
						FillMeshResultTriangles(result.PositiveResult, p.Value, p.Key);
					result.PositiveResult.RecalculateBounds();
				}
				
				if (result.NegativeResult != null)
				{
					result.NegativeResult.SubMeshCount = negativeMeshToTriangle.Max(p => p.Key) + 1;
					foreach (var n in negativeMeshToTriangle)
						FillMeshResultTriangles(result.NegativeResult, n.Value, n.Key);
					result.NegativeResult.RecalculateBounds();
				}
			
				result.Success = true;
			}
			else
				result.Success = false;
			
			return result;
		}
		
		public static IEnumerable<VirtualMesh> SplitIsolatedMeshAreas(VirtualMesh mesh)
		{
			// create edges from target mesh
			var islands = new List<List<TriangleEx>>();
			for (int subMeshId = 0; subMeshId < mesh.SubMeshCount; ++subMeshId)
			{
				var triangles = mesh.GetTriangles(subMeshId);
				for (int i = 0; i < triangles.Length / 3; ++i)
				{
					var i1 = triangles[i * 3 + 0];
					var i2 = triangles[i * 3 + 1];
					var i3 = triangles[i * 3 + 2];
					var newTriangle = new TriangleEx(i1, i2, i3, subMeshId, mesh);
					
					var otherIslands = islands.Where(island => newTriangle.HasSameVertex(island)).ToArray();
					
					var newIsland = new List<TriangleEx>( new TriangleEx[] { newTriangle } );
					if (otherIslands.Any())
					{
						islands.RemoveAll(island => otherIslands.Contains(island));
						newIsland.AddRange(otherIslands.SelectMany(island => island));
					}
					islands.Add(newIsland);
				}
			}
			if (islands.Count <= 1)
				return new VirtualMesh[] { mesh };
			
			var result = new List<VirtualMesh>();
			foreach (var island in islands)
			{
				var newMesh = new VirtualMesh();
				newMesh.SubMeshCount = mesh.SubMeshCount;
				
				var indices = island.SelectMany(t => new int[] { t.i1, t.i2, t.i3 }).Distinct();
				
				newMesh.Vertices = indices.Select(i => mesh.Vertices[i]).ToArray();
				if (mesh.Normals != null && mesh.Normals.Length > 0)
					newMesh.Normals = indices.Select(i => mesh.Normals[i]).ToArray();
				if (mesh.Tangents != null && mesh.Tangents.Length > 0)
					newMesh.Tangents = indices.Select(i => mesh.Tangents[i]).ToArray();
				if (mesh.Colors != null && mesh.Colors.Length > 0)
					newMesh.Colors = indices.Select(i => mesh.Colors[i]).ToArray();
				if (mesh.UV != null && mesh.UV.Length > 0)
					newMesh.UV = indices.Select(i => mesh.UV[i]).ToArray();
				if (mesh.UV2 != null && mesh.UV2.Length > 0)
					newMesh.UV2 = indices.Select(i => mesh.UV2[i]).ToArray();
				
				var newIndexToIndexTable = indices.Select((i, index) => new KeyValuePair<int, int>(i, index)).ToDictionary(i => i.Key, i => i.Value);
				
				var islandsBySubMesh = island.GroupBy(i => i.SubMeshId).OrderBy(s => s.Key);
				foreach (var pair in islandsBySubMesh)
				{
					var newIndices = pair.SelectMany(t => new int[] { newIndexToIndexTable[t.i1], newIndexToIndexTable[t.i2], newIndexToIndexTable[t.i3] }).ToArray();
					if (newIndices.Any())
						newMesh.SetTriangles(newIndices.ToArray(), pair.Key);
				}
				
				result.Add(newMesh);
			}
			return result;
		}

        public static VirtualMesh CreateHull(VirtualMesh originalMesh, int maxTriangleCount)
        {
            var simplifiedMesh = SimplifyMesh(originalMesh, maxTriangleCount);
            var triangleCount = simplifiedMesh.GetTriangles(0).Length / 3;
            if (triangleCount <= 3 || simplifiedMesh.VertexCount <= 4)
            {
                return simplifiedMesh;
            }
            simplifiedMesh.Scale(1000.0f);
            var hull = new QHull.QuickHull3D();
            try
            {
                hull.Build(simplifiedMesh.Vertices);
                hull.Triangulate();
            }
            catch
            {
                return null;
            }
            var mesh = new VirtualMesh();
            mesh.Vertices = hull.Vertices;
            mesh.SetTriangles(hull.Faces.SelectMany(face => face.Take(3)).ToArray(), 0);
            mesh.Scale(1.0f / 1000.0f);
            return mesh;
        }
		
		public static VirtualMesh SimplifyMesh(VirtualMesh mesh, int targetTriangleCount)
		{
			var vertices = new List<Vector3>(mesh.Vertices);
			var triangles = new List<Triangle>();
			
			// collapse mesh (multiple submeshs -> one submesh, eg.)
			// 1. collapse submeshs
			for (int subMeshId = 0; subMeshId < mesh.SubMeshCount; ++subMeshId)
			{
				var subMeshIndicies = mesh.GetTriangles(subMeshId);
				for (int baseIndex = 0; baseIndex < subMeshIndicies.Length; baseIndex += 3)
					triangles.Add(new Triangle(subMeshIndicies[baseIndex + 0], subMeshIndicies[baseIndex + 1], subMeshIndicies[baseIndex + 2]));
			}
			//triangles = triangles.Distinct(new TriangleComparer()).ToList();
			
			// 2.1 find dublicate vertices
			var dublicates = new List<KeyValuePair<int, List<int>>>();
			for (int i = 0; i < vertices.Count; ++i)
			{
				var dublicateList = dublicates.FirstOrDefault(p => Helper.VectorEqual(vertices[p.Key], vertices[i], 0.01f)).Value;
				if (dublicateList != null)
					dublicateList.Add(i);
				else
					dublicates.Add(new KeyValuePair<int, List<int>>(i, new List<int>(new int[] { i })));
			}
			// 2.2 fix triangles
			foreach (var triangle in triangles)
			{
				triangle.i1 = dublicates.IndexOf(dublicates.First(p => p.Value.Contains(triangle.i1)));
				triangle.i2 = dublicates.IndexOf(dublicates.First(p => p.Value.Contains(triangle.i2)));
				triangle.i3 = dublicates.IndexOf(dublicates.First(p => p.Value.Contains(triangle.i3)));
			}
			//triangles = triangles.Distinct(new TriangleComparer()).ToList();
			// 2.3 update vertices
			vertices = dublicates.Select(p => vertices[p.Key]).ToList();
			
			foreach (var triangle in triangles)
				triangle.CalculateEdgeCollapseCosts(vertices, triangles);
			
			// 3 reduce triangles through edge collapse
			while (triangles.Count > targetTriangleCount)
			{
				Edge minCostEdge = null;
				float minCost = float.MaxValue;
				foreach (var triangle in triangles)
				{
					var min = triangle.EdgeCollapseCosts.OrderBy(p => p.Key).First();
					if (min.Key < minCost)
					{
						minCostEdge = min.Value;
						minCost = min.Key;
					}
				}
				     
				// collapse edge
				CollapseEdge(minCostEdge, vertices, triangles);
			}
			
			// build new result mesh
			var resultMesh = new VirtualMesh();
			resultMesh.Vertices = vertices.ToArray();
			resultMesh.SetTriangles(triangles.SelectMany(t => new int[] { t.i1, t.i2, t.i3 }).ToArray(), 0);
			resultMesh.RecalculateBounds();
			return resultMesh;
		}
		
		private static void CollapseEdge(Edge edge,
		                             List<Vector3> vertices,   
		                             List<Triangle> triangles)
		{
			var sides = new List<Triangle>();
			var faces = new List<Triangle>();
			foreach (var triangle in triangles)
			{
				if (triangle.HasEdge(edge))
					sides.Add(triangle);
				if (triangle.HasIndex(edge.i1))
					faces.Add(triangle);
			}
			foreach (var side in sides)
				triangles.Remove(side);
			foreach (var face in faces)
				face.ReplaceIndex(edge.i1, edge.i2);
			
			foreach (var face in faces)
				face.CalculateEdgeCollapseCosts(vertices, triangles);
		}
		
		internal static float ComputeEdgeCollapseCost(Edge edge, 
		                                             List<Vector3> vertices, 
		                                             List<Triangle> triangles) 
		{
			var edgelength = Vector3.Magnitude(vertices[edge.i1] - vertices[edge.i2]);
			var curvature = 0.0f;
			
			var sides = new List<Triangle>();
			var faces = new List<Triangle>();
			foreach (var triangle in triangles)
			{
				if (triangle.HasEdge(edge))
					sides.Add(triangle);
				if (triangle.HasIndex(edge.i1))
					faces.Add(triangle);
			}
			
			var faceNormals = faces.Select(face => Helper.GetFacing(vertices[face.i1], vertices[face.i2], vertices[face.i3]));
			var sideNormals = sides.Select(side => Helper.GetFacing(vertices[side.i1], vertices[side.i2], vertices[side.i3]));
			foreach (var face in faceNormals)
			{
				var minCurv = 1.0f;
				foreach (var side in sideNormals)
				{
					var dot = Vector3.Dot(face, side);
					minCurv = Mathf.Min(minCurv, (1 - dot) / 2.0f);
				}
				curvature = Mathf.Max(curvature, minCurv);
			}
			
			return edgelength * curvature;
		}
			
		private static void SplitMesh(VirtualMesh mesh, 
		                              Vector3 cutPlanePos, 
		                              Vector3 cutPlaneNormal, 
		                              int[] triangles, 
		                              List<VertexData> positiveVertexData, 
		                              List<Triangle> positiveTriangles, 
		                              List<Edge> positiveCutEdgeData, 
		                              List<VertexData> negativeVertexData, 
		                              List<Triangle> negativeTriangles, 
		                              List<Edge> negativeCutEdgeData,
		                              VertexDeclaration vertexDeclaration)
		{
			positiveTriangles.Clear();
			negativeTriangles.Clear();
			
			for (int faceId = 0; faceId < triangles.Length / 3; faceId++)
			{
				var i1 = triangles[faceId * 3 + 0];
				var i2 = triangles[faceId * 3 + 1];
				var i3 = triangles[faceId * 3 + 2];
				var v1 = mesh.Vertices[i1];
				var v2 = mesh.Vertices[i2];
				var v3 = mesh.Vertices[i3];
				var facing = Helper.GetFacing(v1, v2, v3);
					
				VertexData[] positive = null;
				VertexData[] negative = null;
				
				var cutResult = Helper.SplitTriangle(cutPlanePos, 
				                                     cutPlaneNormal, 
				                                     i1, i2, i3, 
				                                     mesh, 
				                                     vertexDeclaration, 
				                                     out positive, 
				                                     out negative);
				if (positive != null)
				{
					var newTriangle = Triangle.Create(positive[0], positive[1], positive[2], positiveVertexData, positiveTriangles, facing, false);
					if (positive.Length == 4)
					{
						newTriangle.SetFacing(positiveVertexData, facing);
						newTriangle = Triangle.Create(positive[1], positive[2], positive[3], positiveVertexData, positiveTriangles, facing, false);
					}
	
					if (cutResult == SplitResult.Intersection)
						positiveCutEdgeData.Add(new MeshUtilities.Edge(newTriangle.i2, newTriangle.i3));
					newTriangle.SetFacing(positiveVertexData, facing);
				}
				if (negative != null)
				{
					var newTriangle = Triangle.Create(negative[0], negative[1], negative[2], negativeVertexData, negativeTriangles, facing, false);
					if (negative.Length == 4)
					{
						newTriangle.SetFacing(negativeVertexData, facing);
						newTriangle = Triangle.Create(negative[1], negative[2], negative[3], negativeVertexData, negativeTriangles, facing, false);
					}
	
					if (cutResult == SplitResult.Intersection)
						negativeCutEdgeData.Add(new MeshUtilities.Edge(newTriangle.i2, newTriangle.i3));
					newTriangle.SetFacing(negativeVertexData, facing);
				}
			}
		}
		
		private static VirtualMesh PreFillMeshResult(List<VertexData> vertices, VertexDeclaration vertexDeclaration)
		{
			if (vertices.Count > 0)
			{
				var mesh = new VirtualMesh();
				mesh.Vertices = vertices.Select(v => v.Position).ToArray();
				if ((vertexDeclaration & MeshUtilities.VertexDeclaration.Normal) != MeshUtilities.VertexDeclaration.None)
					mesh.Normals = vertices.Select(v => v.Normal).ToArray();
				if ((vertexDeclaration & MeshUtilities.VertexDeclaration.Tangent) != MeshUtilities.VertexDeclaration.None)
					mesh.Tangents = vertices.Select(v => v.Tangent).ToArray();
				if ((vertexDeclaration & MeshUtilities.VertexDeclaration.UV) != MeshUtilities.VertexDeclaration.None)
					mesh.UV = vertices.Select(v => v.UV).ToArray();
				if ((vertexDeclaration & MeshUtilities.VertexDeclaration.UV2) != MeshUtilities.VertexDeclaration.None)
					mesh.UV2 = vertices.Select(v => v.UV2).ToArray();
				if ((vertexDeclaration & MeshUtilities.VertexDeclaration.Color) != MeshUtilities.VertexDeclaration.None)
					mesh.Colors = vertices.Select(v => v.Color).ToArray();
				return mesh;
			}
			return null;
		}
		
		private static void FillMeshResultTriangles(VirtualMesh mesh, List<Triangle> triangles, int subMeshId)
		{
			if (triangles.Count == 0)
				return;
			var triangleArray = triangles.SelectMany(t => new int[] { t.i1, t.i2, t.i3 }).ToArray();
			if (subMeshId < 0)
				subMeshId = 0;
			mesh.SetTriangles(triangleArray, subMeshId);
		}
	}
}
