using UnityEditor;
using UnityEngine;

public class MeshUnify : MeshUnifyBase 
{	
	[MenuItem ("Window/Piecemaker/MeshUnify")]
	static void Init() 
	{
		var rect = new Rect();
		rect.width = 300;
		rect.height = 52;
		rect.x = 0;
		rect.y = 0;
		
		EditorWindow.GetWindowWithRect<MeshUnify>(rect, true, "MeshUnify");
	}

}
