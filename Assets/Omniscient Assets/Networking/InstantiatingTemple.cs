using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstantiatingTemple : MonoBehaviour {
	public GameObject temple;
	public bool hasCreated = false;


	// Use this for initialization
	void Start () {
<<<<<<< HEAD
		
		GameObject templeObject = null;
		if (Network.peerType == NetworkPeerType.Server)
=======
		//networkView.RPC("SetUpTemple", RPCMode.All);
		Debug.Log("Max Connections: " + Network.maxConnections);
		Debug.Log("Current Players: " + Network.connections.Length);
	/*	if (Network.peerType == NetworkPeerType.Server)
>>>>>>> 5127ccaa8f5d2298f20429eff8462cd6ac63cb39
		{
			Transform templeTransform;
			float templeX, templeY, templeZ;
		
			templeX = transform.position.x;
			templeY = transform.position.y;
			templeZ = transform.position.z;
			temple.transform.localScale += new Vector3(5.0f, 5.0f, 5.0f);
		
			NetworkViewID viewID = Network.AllocateViewID();
		
			//Network.Instantiate(temple, transform.position, transform.rotation,0);
			Network.Instantiate(temple, new Vector3(templeX, templeY, templeZ), transform.rotation, 0);
			//Instantiate(temple, transform.position, transform.rotation);
		
<<<<<<< HEAD
			//Find Object
			templeObject = GameObject.Find("Temple1(Clone)");
		
			//Give Network View
			templeObject.AddComponent("NetworkView");
		
			//Grab transform to make it child of ImageTarget
			templeTransform = templeObject.transform;
			
			templeTransform.parent = transform;
		}
=======
			networkView.RPC("SetUpTemple", RPCMode.All, viewID);

		} */
>>>>>>> 5127ccaa8f5d2298f20429eff8462cd6ac63cb39
	}
	
	// Update is called once per frame
	void Update () {
	
		if ((hasCreated == false) && (Network.peerType == NetworkPeerType.Server))
		{
			if (Network.maxConnections == Network.connections.Length + 1)
			{
				Debug.Log("Temple Being Created");
				networkView.RPC("SetUpTemple", RPCMode.All);
				
			}
			
		}
		
	}
	
	void OnGUI(){
		if (hasCreated == false)
		{
			GUI.Label(new Rect(10, 10, 150, 150), "Waiting for All Players to Join Game");
		}
	}
	
	[RPC]
	void SetUpTemple()
	{
		GameObject templeObject;
		
		Transform templeTransform;
		float templeX, templeY, templeZ;
		
		templeX = transform.position.x;
		templeY = transform.position.y + 17.0f;
		templeZ = transform.position.z;
		
		
		//Network.Instantiate(temple, transform.position, transform.rotation,0);
		Instantiate(temple, new Vector3(templeX, templeY, templeZ), transform.rotation);
			
		
		//Find Object
		//templeObject = GameObject.Find("Temple1(Clone)");
		
		//Give Network View
		//templeObject.AddComponent("NetworkView");
		
		//Grab transform to make it child of ImageTarget
		//templeObject.transform.parent = transform;
		
		//templeTransform = templeObject.transform;
		//templeTransform.parent = transform;
		//templeObject.transform.localScale += new Vector3(2, 2, 2);
		
		hasCreated = true;
				
		
	}
}
