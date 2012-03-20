using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleController : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	
	private int _weaponIndex;
	
	// Use this for initialization
	void Start () 
	{
		_searchingView = GetComponent<HUDSearchingView>();
		_gameView = GetComponent<HUDGameView>();
		_searchingView.Show(false);
		_searchingView.setController(this);
		_weaponIndex = 0;
		
		//reset level messages
		//clientMessage.gameObject.active = false;
		//serverMessage.gameObject.active = false;
		

		
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	// IHUDSearchingViewController methods
	public void HUDSearchingViewPrintButtonPressed()
	{
		AirprintPlugin.PrintARCard();
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
		//Application.LoadLevel("sampleHUD");
		
	}
	
	public void HUDSearchingViewMenuButtonPressed()
	{
		//Application.LoadLevel("VisitorsMainScene");
	}
	
	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
<<<<<<< HEAD
		_weaponIndex = newWeapon;
=======
		
>>>>>>> 19e11bfe97a1cafc5bb5b47d8c64b2f173455fb3
	}
	
	public void HUDGameViewFireButtonPressed()
	{
//		_gameView.Energy = _gameView.Energy - 1.0f;
//		GameObject cam = GameObject.Find("ARCamera");
//		Debug.Log(cam.transform.position);
//		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
//		GameObject instance = (GameObject)Instantiate(thePrefab, cam.transform.position, cam.transform.rotation);
//		Vector3 fwd = cam.transform.forward * 50000;
//		instance.rigidbody.AddForce(fwd);
		
		_gameView.Energy = _gameView.Energy - 1.0f;
		GameObject cam = GameObject.Find("ARCamera");
		
		Transform spawn;
		if (_weaponIndex == 0)
			spawn = (Transform) Resources.Load("fireballPrefab 1", typeof(Transform));
		else
			spawn = (Transform) Resources.Load("fireballPrefab 5", typeof(Transform));
		
		float initialVelocity = 5000; 
		Transform newWeapon = (Transform) Instantiate(spawn, cam.transform.position, cam.transform.rotation);
		newWeapon.rigidbody.AddForce(cam.transform.forward * initialVelocity);
	}
	
	public void HUDGameViewPauseButtonPressed()
	{
		//Multiplayer Server
		if(Network.peerType == NetworkPeerType.Server)
		{
			Application.LoadLevel("sampleHUDnetworking");
		}
		
		//Multiplayer Client
		if(Network.peerType == NetworkPeerType.Client)
		{

			
			//clientMessage.gameObject.active = true;
			
			Instantiate(clientMessage);
			clientMessage.text = "A Message has been sent to Player 1 Requesting a Level Reset";
		
			StartCoroutine(ClientMessageTimer());
			
			//Send Message to Server
			networkView.RPC("PrintSeverMessage", RPCMode.Server);
		}
		
		//singlePlayer
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Application.LoadLevel("sampleHUD");
		}
		
	}
	
		
	//Only called on Server, Prints request to reset
	public void PrintServerMessage()
	{
		Instantiate(serverMessage);
		serverMessage.text = "A Player has requested you to reset the level";
	
		StartCoroutine(ServerMessageTimer());
			
	}
	
	//Timer Functions to delete the message/// 
	IEnumerator ClientMessageTimer()
	{
		yield return new WaitForSeconds(2);
		
		//Deactivate Message
		//clientMessage.gameObject.active = false;
		Destroy(clientMessage);
	}

	IEnumerator ServerMessageTimer()
	{
		yield return new WaitForSeconds(2);
		
		//Deactivate Message
		//clientMessage.gameObject.active = false;
		Destroy(serverMessage);
	}

	
	
	
	public void HUDGameViewMenuButtonPressed()
	{
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Application.LoadLevel("VisitorsMainScene");	
		}
			
		if(Network.peerType == NetworkPeerType.Client)
		{
			Network.Disconnect(200);
			Application.LoadLevel("VisitorsMainScene");
		}
		
		if(Network.peerType == NetworkPeerType.Server)
		{
			Network.Disconnect(200);
			Application.LoadLevel("VisitorsMainScene");	
		}
	}
}
