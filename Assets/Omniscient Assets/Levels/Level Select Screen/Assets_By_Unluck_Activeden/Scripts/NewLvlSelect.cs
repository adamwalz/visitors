using UnityEngine;
using System.Collections;

public class NewLvlSelect : MonoBehaviour
{
	public string scene = "SP-Egypt-AR";
	
	public void Start()
	{
	}
	
	public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, 100.0f))
			{
				GameState.SetCurrentLevel((string)scene);
				Application.LoadLevel("WeaponSwitcher");
			}
		}
	}
}

