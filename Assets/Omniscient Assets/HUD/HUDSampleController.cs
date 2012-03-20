using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleController : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	
	// Use this for initialization
	void Start () 
	{
		_searchingView = GetComponent<HUDSearchingView>();
		_gameView = GetComponent<HUDGameView>();
		_searchingView.Show(false);
		_searchingView.setController(this);
		
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
		
	}
	
	public void HUDGameViewFireButtonPressed()
	{
		_gameView.Energy = _gameView.Energy - 1.0f;
		GameObject cam = GameObject.Find("ARCamera");
		Debug.Log(cam.transform.position);
		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		GameObject instance = (GameObject)Instantiate(thePrefab, cam.transform.position, cam.transform.rotation);
		Vector3 fwd = cam.transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
		
		
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
