using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleControllerNetworking : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
	
	private int _weaponIndex;
	
	private bool hasStarted = false;
	private int currentLevel = -1;
	
	// Use this for initialization
	void Start () 
	{
		_searchingView = GetComponent<HUDSearchingView>();
		_gameView = GetComponent<HUDGameView>();
		_searchingView.Show(false);
		_searchingView.setController(this);
		_weaponIndex = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((hasStarted == false) && (Network.peerType == NetworkPeerType.Server))
		{
			//If the game becomes full, this makes it so other devices
			//No Longer see this game session as an option
			if (Network.maxConnections == Network.connections.Length)
			{
				Network.maxConnections = -1;
				networkView.RPC("GameIsFull",RPCMode.All);
			}
		}
		
		//If we ever have the game started, and for some reason the game isn't full,
		//we disconnect and go back to the main Menu
		if ((hasStarted == true) && (Network.peerType == NetworkPeerType.Server))
		{
			if (Network.maxConnections != Network.connections.Length)
			{
				networkView.RPC("MainMenu", RPCMode.Server);
			}
		}
	}
	
	//This function is called on Server when a Client Disconnects
    void OnPlayerDisconnected(NetworkPlayer player) 
	{
		//if full, we do stuff, if not full, stay and wait for more
		if (hasStarted == true)
		{
			Network.Disconnect();
			Application.LoadLevel("VisitorsMainScene");
		}
	}
	
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		//This is called on the Client when the Connection to Server is severed. 
		//Usually when the server DC's
		
		Application.LoadLevel("VisitorsMainScene");
	}
	
	[RPC]
	public void GameIsFull()
	{
		hasStarted = true;
	}
	
	// IHUDSearchingViewController methods
	public void HUDSearchingViewPrintButtonPressed()
	{
		UtilityPlugin.PrintARCard();
	}
	
	public void HUDSearchingViewPlayWithoutButtonPressed()
	{
		_searchingView.Hide(false);
		_gameView.SetController(this);
		List<Texture2D> list = new List<Texture2D>();
		Texture2D tex = (Texture2D)Resources.Load("fireballWeapon", typeof(Texture2D));
		list.Add(tex);
		tex = (Texture2D)Resources.Load("blueWeaponOne", typeof(Texture2D));
		list.Add(tex);
		_gameView.Show(list, false);
	}
	
	public void HUDSearchingViewPauseButtonPressed()
	{
		networkView.RPC("ResetLevel",RPCMode.All);
	}
	
	public void HUDSearchingViewMenuButtonPressed()
	{
		if (hasStarted == true)
		{
			networkView.RPC("MainMenu", RPCMode.Server);
		}
		
		else
		{
			Network.Disconnect();
			Application.LoadLevel("VisitorsMainScene");
		}
	}
	
	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
		_weaponIndex = newWeapon;
	}
	
	public void HUDGameViewFireButtonPressed()
	{

//		_gameView.Energy = _gameView.Energy - 1.0f;
//		GameObject cam = GameObject.Find("ARCamera");
//		Debug.Log(cam.transform.position);
//		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
//		GameObject instance = (GameObject)Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
//		Vector3 fwd = cam.transform.forward * 50000;
//		instance.rigidbody.AddForce(fwd);
		

	
		
		if (hasStarted)
		{
			GameObject cam = GameObject.Find("ARCamera");
			Vector3 fwd = cam.transform.forward * 50000;
			networkView.RPC("ShootWithoutNetworkInstantiate",RPCMode.All, cam.transform.position, cam.transform.rotation, fwd, _weaponIndex);
			_gameView.Energy = _gameView.Energy - 1.0f;
		}

		
		//networkView.RPC("ShootWithoutNetworkInstantiate",RPCMode.All);
		
		/*
		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		GameObject instance = (GameObject)Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
		Vector3 fwd = cam.transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
		*/
	}
	
	[RPC]
	public void ShootWithoutNetworkInstantiate(Vector3 position, Quaternion rotation, Vector3 fwd, int weaponIndex)
	{
		//GameObject cam = GameObject.Find("ARCamera");
		Transform spawn;
		if (weaponIndex == 0)
			spawn = (Transform) Resources.Load("fireballPrefab 1", typeof(Transform));
		else
			spawn = (Transform) Resources.Load("fireballPrefab 5", typeof(Transform));

		
		//GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		//GameObject instance = (GameObject)Instantiate(thePrefab, cam.transform.position, cam.transform.rotation);
		
		Transform newWeapon = (Transform)Instantiate(spawn, position, rotation);
		newWeapon.rigidbody.AddForce(fwd);
		
		
		
		//GameObject instance = (GameObject)Instantiate(thePrefab, position, rotation);
		//Vector3 fwd = cam.transform.forward * 50000;
		//instance.rigidbody.AddForce(fwd);
	}
	
	public void HUDGameViewPauseButtonPressed()
	{
		networkView.RPC("ResetLevel",RPCMode.All);
	}
	
	public void HUDGameViewMenuButtonPressed()
	{
		if (hasStarted == true)
		{
			networkView.RPC("MainMenu", RPCMode.Server);
		}
		
		else
		{
			Network.Disconnect();
			Application.LoadLevel("VisitorsMainScene");
		}
		
	}
	
	[RPC]
	public void ResetLevel()
	{
		if (currentLevel != Application.loadedLevel)
		{
			currentLevel = Application.loadedLevel;
		}
		Application.LoadLevel(currentLevel);
		//Application.LoadLevel("sampleHUDnetworking");
	}
	
	[RPC]
	public void Disconnect()
	{
		Network.Disconnect();
		Application.LoadLevel("VisitorsMainScene");
	}
	
	[RPC]
	public void MainMenu()
	{
		networkView.RPC("Disconnect", RPCMode.Others);
		Network.Disconnect(200);
		Application.LoadLevel("VisitorsMainScene");
	}
	
}
