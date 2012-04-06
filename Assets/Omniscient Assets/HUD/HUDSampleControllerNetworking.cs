using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleControllerNetworking : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
	private bool hasStarted = false;
	// Use this for initialization
	void Start () 
	{
		_searchingView = GetComponent<HUDSearchingView>();
		_gameView = GetComponent<HUDGameView>();
		_searchingView.Show(false);
		_searchingView.setController(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ((hasStarted == false) && (Network.peerType == NetworkPeerType.Server))
		{
			if (Network.maxConnections == Network.connections.Length + 1)
			{
				networkView.RPC("GameIsFull",RPCMode.All);
			}
		}
		
	}
	
	[RPC]
	public void GameIsFull()
	{
		hasStarted = true;
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
		Application.LoadLevel("sampleHUD");
	}
	
	public void HUDSearchingViewMenuButtonPressed()
	{
		Application.LoadLevel("VisitorsMainScene");	
	}
	
	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
	}
	
	public void HUDGameViewFireButtonPressed()
	{
		Debug.Log ("Max Connections = " + Network.maxConnections);
		Debug.Log ("Network Connections = " + Network.connections.Length);
		Debug.Log ("Network Upper Bound = " + Network.connections.GetUpperBound(0));
		
		if (hasStarted)
		{
			networkView.RPC("ShootWithoutNetworkInstantiate",RPCMode.All);
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
	public void ShootWithoutNetworkInstantiate()
	{
		GameObject cam = GameObject.Find("ARCamera");
		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		GameObject instance = (GameObject)Instantiate(thePrefab, cam.transform.position, cam.transform.rotation);
		Vector3 fwd = cam.transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
	}
	
	public void HUDGameViewPauseButtonPressed()
	{
		Application.LoadLevel("sampleHUD");
	}
	
	public void HUDGameViewMenuButtonPressed()
	{
		Application.LoadLevel("VisitorsMainScene");	
	}
}
