using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewLvlSelectCS : MonoBehaviour 
{
	
	public string scene;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) // check for left-mouse
		{	
   			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			 
			
			if (Physics.Raycast(ray, out hit, (float)100.0))
			{
    			if (hit.collider.tag == "Egypt")
    			{
					GameState.SetCurrentLevel("SP-Egypt-AR");
    				
     	  		}
    		
				if (hit.collider.tag == "Player")
				{
					GameState.SetCurrentLevel("SP-Castle-AR");
				}
				
				Application.LoadLevel("WeaponSwitcher");
			
			}
		}
	}
}
