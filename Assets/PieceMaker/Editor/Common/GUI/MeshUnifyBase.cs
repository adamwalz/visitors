using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

public class MeshUnifyBase : EditorWindow 
{	
	struct Data
	{
        public MeshUtilities.VirtualMesh Mesh;
		public Material[] Materials;
		public Transform Transform;
	}
	
	string finalMeshName = "Unified Mesh";
	
	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Prefab/Mesh filepath:");
		finalMeshName = EditorGUILayout.TextField(finalMeshName);
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Collapse selection to single Mesh"))
		{
			if (Selection.gameObjects.Length == 0)
			{
				EditorUtility.DisplayDialog("Error", "No selection of GameObjects, please select one or more GameObjects.", "Ok");
				return;
			}
			var collectedData = new List<Data>();
			foreach (var gameObject in Selection.gameObjects)
				CollectData(gameObject, collectedData);
			
			if (collectedData.Count == 0)
			{
				EditorUtility.DisplayDialog("Info", "Selected GameObjects do not contain any meshes.", "Ok");
				return;
			}
			CreateMesh(collectedData);
		}
	}
	
	void CreateMesh(List<Data> collectedData)
	{
        var vertexDeclaration = MeshUtilities.VertexDeclaration.Position;
		foreach (var data in collectedData)
		{
            if (data.Mesh.Normals.Length > 0 && (vertexDeclaration & MeshUtilities.VertexDeclaration.Normal) == 0)
                vertexDeclaration |= MeshUtilities.VertexDeclaration.Normal;
            if (data.Mesh.Tangents.Length > 0 && (vertexDeclaration & MeshUtilities.VertexDeclaration.Tangent) == 0)
                vertexDeclaration |= MeshUtilities.VertexDeclaration.Tangent;
            if (data.Mesh.UV.Length > 0 && (vertexDeclaration & MeshUtilities.VertexDeclaration.UV) == 0)
                vertexDeclaration |= MeshUtilities.VertexDeclaration.UV;
            if (data.Mesh.UV2.Length > 0 && (vertexDeclaration & MeshUtilities.VertexDeclaration.UV2) == 0)
                vertexDeclaration |= MeshUtilities.VertexDeclaration.UV2;
            if (data.Mesh.Colors.Length > 0 && (vertexDeclaration & MeshUtilities.VertexDeclaration.Color) == 0)
                vertexDeclaration |= MeshUtilities.VertexDeclaration.Color;
		};
		
		var vertices = collectedData.SelectMany(data => 
		{
            var vertexData = new MeshUtilities.VertexData[data.Mesh.VertexCount];
			for (int i = 0; i < data.Mesh.VertexCount; ++i)
			{
                vertexData[i] = new MeshUtilities.VertexData();
				
				vertexData[i].Position = data.Transform.TransformPoint(data.Mesh.Vertices[i]);
				if (data.Mesh.Normals.Length > 0)
					vertexData[i].Normal = data.Transform.TransformDirection(data.Mesh.Normals[i]);
				if (data.Mesh.Tangents.Length > 0)
				{
					vertexData[i].Tangent = data.Transform.TransformDirection((Vector3)data.Mesh.Tangents[i]);
					vertexData[i].Tangent.w = data.Mesh.Tangents[i].w;
				}
				if (data.Mesh.UV.Length > 0)
					vertexData[i].UV = data.Mesh.UV[i];
				if (data.Mesh.UV2.Length > 0)
					vertexData[i].UV2 = data.Mesh.UV2[i];
				if (data.Mesh.Colors.Length > 0)
					vertexData[i].Color = data.Mesh.Colors[i];
			}
			return vertexData;
		});
		
		var trianglesByMaterial = new Dictionary<Material, List<int>>();
		var baseIndex = 0;
		foreach (var data in collectedData)
		{
			for (var i = 0; i < data.Materials.Length; ++i)
			{
				var material = data.Materials[i];
				
				var triangles = trianglesByMaterial.FirstOrDefault(pair => pair.Key.name == material.name).Value;
				if (triangles == null)
				{
					triangles = new List<int>();
					trianglesByMaterial.Add(material, triangles);
				}
				var tris = data.Mesh.GetTriangles(i);
				triangles.AddRange(tris.Select(triangle => baseIndex + triangle));
			}
			baseIndex += data.Mesh.VertexCount;
		}

        var mesh = new MeshUtilities.VirtualMesh();
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
		
		mesh.SubMeshCount = trianglesByMaterial.Count;
		for (var i = 0; i < trianglesByMaterial.Count; ++i)
			mesh.SetTriangles(trianglesByMaterial.ElementAt(i).Value.ToArray(), i);

		var newRealMesh = mesh.ToMesh();
		newRealMesh.Optimize();
		
		AssetDatabase.CreateAsset(newRealMesh, "Assets/" + finalMeshName + ".obj");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		var go = new GameObject();
		var renderer = go.AddComponent<MeshRenderer>();
		renderer.sharedMaterials = trianglesByMaterial.Select(pair => pair.Key).ToArray();
		var filter = go.AddComponent<MeshFilter>();
		filter.sharedMesh = newRealMesh;
		
		var path = "Assets/" + finalMeshName + ".prefab";
		var prefab = PrefabUtility.CreateEmptyPrefab(path);
				
		EditorUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ReplaceNameBased);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Selection.activeGameObject = go;
	}
	
	void CollectData(GameObject gameObject, List<Data> collectedData)
	{
		var meshFilter = gameObject.GetComponent<MeshFilter>();
		var meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshFilter != null && meshRenderer != null)
		{
			collectedData.Add(new Data() 
			{
				Materials = meshRenderer.sharedMaterials,
                Mesh = new MeshUtilities.VirtualMesh(meshFilter.sharedMesh),
				Transform = gameObject.transform
			});
		}
		for (int i = 0; i < gameObject.transform.childCount; ++i)
			CollectData(gameObject.transform.GetChild(i).gameObject, collectedData);
	}
}
