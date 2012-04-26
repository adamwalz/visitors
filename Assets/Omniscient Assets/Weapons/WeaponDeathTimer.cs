using UnityEngine;
using System.Collections;

public class WeaponDeathTimer : MonoBehaviour
{
	
	int deathTime;
	// Use this for initialization
	void Start ()
	{
		deathTime = 500;
	}
	
	// Update is called once per frame
	void Update ()
	{
		deathTime--;
		
		if (deathTime <= 0)
			Destroy(this.gameObject);
	}
}
