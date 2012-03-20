using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiatingTemple : MonoBehaviour {
	public GameObject temple;

	// Use this for initialization
	void Start () {
		
		if (Network.peerType == NetworkPeerType.Server)
		{
			GameObject templeObject;
			Transform templeTransform;
			float templeX, templeY, templeZ;
		
			templeX = transform.position.x;
			templeY = transform.position.y + 17.0f;
			templeZ = transform.position.z;
		
		
			//Network.Instantiate(temple, transform.position, transform.rotation,0);
			Network.Instantiate(temple, new Vector3(templeX, templeY, templeZ), transform.rotation, 0);
			//Instantiate(temple, transform.position, transform.rotation);
		
			//Find Object
			templeObject = GameObject.Find("Temple1(Clone)");
		
			//Give Network View
			templeObject.AddComponent("NetworkView");
		
			//Grab transform to make it child of ImageTarget
			//templeTransform = templeObject.transform;
			//templeTransform.parent = transform;
			templeObject.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
