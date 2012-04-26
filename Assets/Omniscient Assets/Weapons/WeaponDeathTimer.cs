using UnityEngine;
using System.Collections;

public class WeaponDeathTimer : MonoBehaviour
{
	
	private int deathTime;
	// Use this for initialization
	void Start ()
	{
		deathTime = 500;
		if (this.gameObject.name.Contains("Fire"))
			deathTime = 100;
	}
	
	// Update is called once per frame
	void Update ()
	{
		deathTime--;
		
		if (deathTime <= 0)
			Destroy(this.gameObject);
	}
}
