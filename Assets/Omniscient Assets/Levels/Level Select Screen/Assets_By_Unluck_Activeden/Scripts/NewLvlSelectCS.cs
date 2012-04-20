using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewLvlSelectCS : MonoBehaviour 
{
	
	public string scene;
	
	// Use this for initialization
<<<<<<< HEAD
	void Start () {
=======
	void Start () 
	{
<<<<<<< HEAD
		GameState.SetCurrentLevel(scene);
=======
>>>>>>> 6ab4df4d3c3705c63fbe3bcea4ab9a0aaf203b55
		GameState.setCurrentLevel(scene);
>>>>>>> 68c9c1feca8d3d8490cee1261f965261e06a06ce
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
