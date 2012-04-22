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
				Debug.Log("Hit Object Named: " + hit.collider.gameObject.name + " and a Tag with: " + hit.collider.tag);
				//hit.collider.gameObject.name
    			if (hit.collider.tag == "Egypt")
    			{
					Debug.Log("Hit Egypt");
					if (GameState.GetPlayingMultiplayer())
					{
						GameState.SetCurrentLevel("MP-Egypt-AR");
						Application.LoadLevel("ConnectionScene");
					}
					else
					{
						GameState.SetCurrentLevel("SP-Egypt-AR");
						Application.LoadLevel("WeaponSwitcher");
					}
					
     	  		}
    		
				else if (hit.collider.tag == "Castle")
				{
					Debug.Log("Hit Castle");
					if (GameState.GetPlayingMultiplayer())
					{
						GameState.SetCurrentLevel("MP-Castle-AR");
						Application.LoadLevel("ConnectionScene");
					}
					else
					{
						GameState.SetCurrentLevel("SP-Castle-AR");
						Application.LoadLevel("WeaponSwitcher");
					}
				}
			}
		}
	}
}
