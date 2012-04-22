using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitializingNetworkingScene : MonoBehaviour {
	public bool hasCreated = false;
	int peopleRemaining;
	string peopleMessage;
	
	int portNumber = 25000;
	
	void Start()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			Debug.Log("Disconnected");
			if (GameState.GetIsServer())
			{
				string gameNameWeaponsString = GameState.GetCurrentLevel() + "," + GameState.LoadPrimaryWeapon() + "," + GameState.LoadSecondaryWeapon();
				string gameName = SystemInfo.deviceName;
				int playerNumberInt = GameState.GetPlayerNumber();
				
				Network.InitializeServer(32, portNumber, false);
				
				Network.maxConnections = playerNumberInt - 1;
				
				
				MasterServer.updateRate = 3;
				MasterServer.RegisterHost("Visitors", gameName, gameNameWeaponsString);
				Debug.Log("the comment: " + gameNameWeaponsString);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the game hasn't been created, but game is full, We Set up Temple
		if ((hasCreated == false) && (Network.peerType == NetworkPeerType.Server))//(Network.peerType == NetworkPeerType.Server))
		{
			if (Network.maxConnections == Network.connections.Length)
			{
				networkView.RPC("SetUpTemple", RPCMode.All);
			}
		}
		
	}
	
	void OnGUI(){
		//If game hasn't Started yet
		if (hasCreated == false)
		{
			if (Network.peerType == NetworkPeerType.Server)
			{
				//Debug.Log("On Gui");
				//Server Updates the Remaining Players and Sends the Info to the Clients
				if ( peopleRemaining != Network.maxConnections - Network.connections.Length)
				{
					peopleRemaining = Network.maxConnections - Network.connections.Length;
					networkView.RPC("ChangeRemainingPlayers",RPCMode.Others, peopleRemaining);
				}
			}	
			
			//Having the correct Message with Correct Grammar depending on remaining People
			if (peopleRemaining > 1)
			{
				peopleMessage = "Waiting For " + peopleRemaining + " More Players To Join The Game";
				
			}
			
			else
			{
				peopleMessage = "Waiting For " + peopleRemaining + " More Player To Join The Game";	
			}
			
			GUI.Label(new Rect(10, 10, 150, 150), peopleMessage);
			
		}
	}
	
	//Called on All Clients to update the correct Remaining Players
	[RPC]
	void ChangeRemainingPlayers(int remainingPlayers)
	{
		peopleRemaining = remainingPlayers;
	}
	
	//Called on Server and Clients and Sets up the Temple
	[RPC]
	void SetUpTemple()
	{
		/*
		GameObject templeObject;
		
		Transform templeTransform;
		float templeX, templeY, templeZ;
		
		//Determines the Starting Location of the Temple
		templeX = transform.position.x;
		templeY = transform.position.y + 17.0f;
		templeZ = transform.position.z;
		
		
		//Network.Instantiate(temple, transform.position, transform.rotation,0);
		Instantiate(temple, new Vector3(templeX, templeY, templeZ), transform.rotation);
			
		
		//Find Object
		templeObject = GameObject.Find("Temple1(Clone)");
		
		//Grab transform to make it child of ImageTarget
		templeObject.transform.parent = transform;
		
		//Scale the Temple
		//templeObject.transform.localScale += new Vector3(2, 2, 2);
		*/
		
		//We say the game has not started so this function will not be called again.
		hasCreated = true;

	}
}
