using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleControllerNetworking : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
<<<<<<< HEAD
	
	private int _weaponIndex;
	
=======
	private bool hasStarted = false;
>>>>>>> 5127ccaa8f5d2298f20429eff8462cd6ac63cb39
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
		_weaponIndex = newWeapon;
	}
	
	public void HUDGameViewFireButtonPressed()
	{
<<<<<<< HEAD
//		_gameView.Energy = _gameView.Energy - 1.0f;
//		GameObject cam = GameObject.Find("ARCamera");
//		Debug.Log(cam.transform.position);
//		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
//		GameObject instance = (GameObject)Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
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
		Transform newWeapon = (Transform) Network.Instantiate(spawn, cam.transform.position, cam.transform.rotation, 0);
		newWeapon.rigidbody.AddForce(cam.transform.forward * initialVelocity);
=======
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
>>>>>>> 5127ccaa8f5d2298f20429eff8462cd6ac63cb39
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
