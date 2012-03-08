using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using MeshUtilities;

public class MeshHullBase : EditorWindow 
{	
	private string finalMeshName = "Mesh Hull";
    private Mesh inputMesh;
    private int maxTriangles = 255;

	void OnGUI()
	{
        finalMeshName = EditorGUILayout.TextField("Prefab/Mesh filepath:", finalMeshName);
        inputMesh = (Mesh)EditorGUILayout.ObjectField("Mesh:", inputMesh, typeof(Mesh), false);
        maxTriangles = EditorGUILayout.IntSlider("Max triangles:", maxTriangles, 3, 255);
		if (GUILayout.Button("Create Convex Hull"))
		{
            if (inputMesh == null)
			{
				EditorUtility.DisplayDialog("Error", "No mesh selected, please select a mesh first.", "Ok");
				return;
			}

            CreateHull();
		}
	}

    void CreateHull()
	{
        var mesh = MeshUtilities.Tools.CreateHull(new VirtualMesh(inputMesh), maxTriangles);
        var newRealMesh = mesh.ToMesh();
		newRealMesh.Optimize();
		
		AssetDatabase.CreateAsset(newRealMesh, "Assets/" + finalMeshName + ".obj");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		var go = new GameObject();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<Rigidbody>();
        
		var filter = go.AddComponent<MeshFilter>();
		filter.sharedMesh = newRealMesh;

        var collider = go.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.sharedMesh = newRealMesh;

		var path = "Assets/" + finalMeshName + ".prefab";
		var prefab = PrefabUtility.CreateEmptyPrefab(path);
				
		EditorUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ReplaceNameBased);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Selection.activeGameObject = go;
	}
}
