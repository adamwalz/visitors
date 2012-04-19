using UnityEngine;
using System.Collections;

public class LvlChange : MonoBehaviour {
	
	bool overLight = false;
	bool clickedOnLight = false;
	// Use this for initialization
	
	void OnMouseOver ()
	{	
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("Should Of Shown Click");	
		}
		
	}
		
	 
}
