using System;
using System.Linq;
using System.Collections.Generic;

namespace MeshUtilities
{
	public class VirtualMesh
	{
		List<int[]> triangles = new List<int[]>();
			
		public VirtualMesh()
		{
			PreInit();
			this.SubMeshCount = 1;
			RecalculateBounds();
		}
		
		private void PreInit()
		{
			this.Vertices = new UnityEngine.Vector3[0];
			this.Normals = new UnityEngine.Vector3[0];
			this.Tangents = new UnityEngine.Vector4[0];
			this.UV = new UnityEngine.Vector2[0];
			this.UV2 = new UnityEngine.Vector2[0];
			this.Colors = new UnityEngine.Color[0];
		}

        public VirtualMesh(UnityEngine.Mesh mesh)
            : this(mesh, 1.0f)
        {
        }
		
		public VirtualMesh(UnityEngine.Mesh mesh, float scale)
		{
			PreInit();
			this.SubMeshCount = mesh.subMeshCount;

            if (mesh.vertices != null)
            {
                if (scale == 1.0f)
                    this.Vertices = mesh.vertices;
                else
                {
                    this.Vertices = mesh.vertices.Select(v => v * scale).ToArray();
                    this.RecalculateBounds();
                }
            }
			if (mesh.normals != null)
				this.Normals = mesh.normals;
			if (mesh.tangents != null)
				this.Tangents = mesh.tangents;
			if (mesh.uv != null)
				this.UV = mesh.uv;
			if (mesh.uv2 != null)
				this.UV2 = mesh.uv2;
			if (mesh.colors != null)
				this.Colors = mesh.colors;
			for (var i = 0; i < mesh.subMeshCount; ++i)
				this.SetTriangles(mesh.GetTriangles(i), i);
			this.Bounds = mesh.bounds;
		}

        public VirtualMesh(VirtualMesh mesh)
           : this(mesh, 1.0f)
        {
        }
		
		public VirtualMesh(VirtualMesh mesh, float scale)
		{
			PreInit();
			this.SubMeshCount = mesh.SubMeshCount;

            if (mesh.Vertices != null)
            {
                if (scale == 1.0f)
                    this.Vertices = mesh.Vertices;
                else
                {
                    this.Vertices = mesh.Vertices.Select(v => v * scale).ToArray();
                    this.RecalculateBounds();
                }
            }

			if (mesh.Normals != null)
				this.Normals = mesh.Normals;
			if (mesh.Tangents != null)
				this.Tangents = mesh.Tangents;
			if (mesh.UV != null)
				this.UV = mesh.UV;
			if (mesh.UV2 != null)
				this.UV2 = mesh.UV2;
			if (mesh.Colors != null)
				this.Colors = mesh.Colors;
			for (var i = 0; i < mesh.SubMeshCount; ++i)
				this.SetTriangles(mesh.GetTriangles(i), i);
			this.Bounds = mesh.Bounds;
		}

		#region VirtualMesh implementation
		public int[] GetTriangles (int subMeshId)
		{
			return triangles[subMeshId];
		}

		public void SetTriangles (int[] triangles, int subMeshId)
		{
			this.triangles[subMeshId] = triangles;
		}

		public void RecalculateBounds()
		{
			if (Vertices != null && Vertices.Length > 0)
			{
				UnityEngine.Vector3 min = Vertices[0];
				UnityEngine.Vector3 max = min;
				foreach (var vertex in Vertices)
				{
					min = UnityEngine.Vector3.Min(min, vertex);
					max = UnityEngine.Vector3.Max(max, vertex);
				}
				var size = max - min;
				var center = (min + max) / 2;
				this.Bounds = new UnityEngine.Bounds(center, size);
			}
			else
				Bounds = new UnityEngine.Bounds(UnityEngine.Vector3.zero, UnityEngine.Vector3.zero);
		}

		public UnityEngine.Vector3[] Vertices { get; set; }
		public UnityEngine.Vector3[] Normals { get; set; }
		public UnityEngine.Vector4[] Tangents { get; set; }
		public UnityEngine.Vector2[] UV { get; set; }
		public UnityEngine.Vector2[] UV2 { get; set; }
		public UnityEngine.Color[] Colors { get; set; }

		public int VertexCount 
		{
			get { return Vertices.Length; }
		}

		public int SubMeshCount 
		{
			get { return triangles.Count; }
			set 
			{
				while (value < SubMeshCount)
					triangles.RemoveAt(triangles.Count - 1);
				while (value > SubMeshCount)
					triangles.Add(new int[0]);
			}
		}

		public UnityEngine.Bounds Bounds { get; set; }

        public void Scale(float factor)
        {
            if (factor == 1.0f)
                return;

            this.Vertices = this.Vertices.Select(v => v * factor).ToArray();
            this.RecalculateBounds();
        }

        public UnityEngine.Mesh ToMesh()
        {
            return ToMesh(1.0f);
        }

		public UnityEngine.Mesh ToMesh(float scale)
		{
			var mesh = new UnityEngine.Mesh();
            if (this.Vertices != null)
            {
                if (scale == 1.0f)
                    mesh.vertices = this.Vertices;
                else
                    mesh.vertices = this.Vertices.Select(v => v * scale).ToArray();
            }
			if (this.Normals != null)
				mesh.normals = this.Normals;
			if (this.Tangents != null)
				mesh.tangents = this.Tangents;
			if (this.UV != null)
				mesh.uv = this.UV;
			if (this.UV2 != null)
				mesh.uv2 = this.UV2;
			if (this.Colors != null)
				mesh.colors = this.Colors;
			mesh.subMeshCount = this.SubMeshCount;
			for (var i = 0; i < triangles.Count; ++i)
				mesh.SetTriangles(triangles[i], i);
			return mesh;
		}
		#endregion
	}
}

