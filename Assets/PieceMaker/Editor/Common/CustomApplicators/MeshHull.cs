using UnityEditor;
using UnityEngine;

public class MeshHull : MeshHullBase 
{	
	[MenuItem ("Window/Piecemaker/MeshHull")]
	static void Init() 
	{
		var rect = new Rect();
		rect.width = 300;
		rect.height = 100;
		rect.x = 0;
		rect.y = 0;
		
		EditorWindow.GetWindowWithRect<MeshHull>(rect, true, "MeshHull");
	}

}
