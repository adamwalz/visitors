using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewLvlSelectCS : MonoBehaviour 
{
	
	public string scene;
	
	// Use this for initialization
	void Start () 
	{
		GameState.SetCurrentLevel(scene);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) // check for left-mouse
		{	
   			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			 
			
			if (Physics.Raycast(ray, out hit, (float)100.0))
			{
    			if (hit.collider.tag == "LevelLight")
    			{
    				Application.LoadLevel("WeaponSwitcher");
     	  		}
    		}
		}
	}
}
