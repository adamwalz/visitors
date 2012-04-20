using UnityEngine;
using System.Collections;

public class LvlChange : MonoBehaviour {
	public string SceneName = "SP-Egypt-AR";
	bool overLight = false;
	bool clickedOnLight = false;
	// Use this for initialization
	
	void OnMouseOver ()
	{	
		if (Input.GetMouseButtonDown(0))
		{
			GameState.setCurrentLevel(SceneName);
			Application.LoadLevel("WeaponSwitcher");
			Debug.Log("Should Of Shown Click");
		}
		
	}
		
	 
}
