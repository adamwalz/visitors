using UnityEngine;
using System.Collections;

public class GameControllerNetworking : MonoBehaviour 
{
	
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	private GameScreen _mainScreen;
	private SearchingView _searchingView;
	private PlayingView _playingView;
	private PauseMenu _pauseMenu;
	private bool hasStarted = false;
	private int currentLevel = -1;
	private int _weaponIndex = 0;
	
	// Use this for initialization
	void Start () 
	{
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_searchingView = (SearchingView)gameObject.AddComponent("SearchingView");
		_searchingView.Init();
		_searchingView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_searchingView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_searchingView.PauseButton.ButtonPressed += new EventHandler(PausePressed);
		_searchingView.PrintButton.ButtonPressed += new EventHandler(PrintButtonPressed);
		_searchingView.PlayWithoutButton.ButtonPressed += new EventHandler(PlayWithoutButtonPressed);
		_mainScreen.AddView(_searchingView);
		
		_playingView = (PlayingView)gameObject.AddComponent("PlayingView");
		_playingView.Init();
		_playingView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_playingView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_playingView.PauseButton.ButtonPressed += new EventHandler(PausePressed);
		_playingView.SwitchWeaponButton.ButtonPressed += new EventHandler(SwitchWeaponButtonPressed);
		_playingView.FireButton.ButtonPressed += new EventHandler(FireButtonPressed);
		_mainScreen.AddView(_playingView);
		_searchingView.Show(true);
		
		_pauseMenu = (PauseMenu)gameObject.AddComponent("PauseMenu");
		_pauseMenu.Init();
		_pauseMenu.Size = _mainScreen.Size;
		_pauseMenu.Position = new Vector2(_mainScreen.Size.x / 2.0f, _mainScreen.Size.y / 2.0f);
		_pauseMenu.ResumeButton.ButtonPressed += new EventHandler(ResumePressed);
		_pauseMenu.MainMenuButton.ButtonPressed += new EventHandler(MenuPressed);
		_pauseMenu.ResetButton.ButtonPressed += new EventHandler(ResetPressed);
		print(_pauseMenu);
		_mainScreen.AddView(_pauseMenu);
	}
			
	public void ResumePressed(object sender)	
	{
		DismissPauseMenu();
	}
	
	public void MenuPressed(object sender)
	{
		HUDGameViewMenuButtonPressed();
	}
	
	public void ResetPressed(object sender)
	{
		networkView.RPC("ResetLevel",RPCMode.All);
	}
	
	public void SwitchWeaponButtonPressed(object sender)
	{
		if(_weaponIndex == 0) _weaponIndex = 1;
		else _weaponIndex = 0;
		_playingView.Switcher.CurrentWeapon = _weaponIndex;
	}
	
	public void ShowPauseMenu()
	{
		_pauseMenu.Show(true);
		_playingView.HasFocus = false;
		_searchingView.HasFocus = false;
	}
	
	
	
	public void PausePressed(object sender)
	{
		ShowPauseMenu();
	}
	
	public void DismissPauseMenu()
	{
		_pauseMenu.Hide(true);
		_playingView.HasFocus = true;
		_searchingView.HasFocus = true;
	}
	
	public void PrintButtonPressed(object sender)
	{
		UtilityPlugin.PrintARCard();
	}
	
	public void PlayWithoutButtonPressed(object sender)
	{
		StartCoroutine(TransitionToPlayingView());
	}
	
	public void SwitchToPlayingView()
	{
		// if in search view, go to play view
		if (_searchingView.State == GameView.GameViewState.Showing)
			StartCoroutine(TransitionToPlayingView());
	}
	public void SwitchToSearchingView()
	{
		// if in play view, go to serach view
		if (_playingView.State == GameView.GameViewState.Showing)
			StartCoroutine(TransitionToSearchingView());
	}
	
	IEnumerator TransitionToPlayingView()
	{
		_searchingView.Hide(true);
		yield return new WaitForSeconds(_searchingView.AnimationDuration);
		_playingView.Show(true);
	}
	
	IEnumerator TransitionToSearchingView()
	{
		_playingView.Hide(true);
		yield return new WaitForSeconds(_searchingView.AnimationDuration);
		_searchingView.Show(true);
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
			Application.LoadLevel("Main Menu");
		}
	}
	
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		//This is called on the Client when the Connection to Server is severed. 
		//Usually when the server DC's
		
		Application.LoadLevel("Main Menu");
	}
	
	[RPC]
	public void GameIsFull()
	{
		hasStarted = true;
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
			Application.LoadLevel("Main Menu");
		}
	}
	
	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
		_weaponIndex = newWeapon;
	}
	
	public void FireButtonPressed(object sender)
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
			networkView.RPC("ShootWithoutNetworkInstantiate",RPCMode.All, cam.transform.position, cam.transform.rotation, fwd);
			_playingView.WeaponBar.Energy = _playingView.WeaponBar.Energy - 1.0f;
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
	public void ShootWithoutNetworkInstantiate(Vector3 position, Quaternion rotation, Vector3 fwd)
	{
		bool primaryWeapon = (_weaponIndex == 0);
		string weaponID = "";
		if (primaryWeapon)
			weaponID = GameState.LoadPrimaryWeapon();
		else
			weaponID = GameState.LoadSecondaryWeapon();
		Transform spawn = (Transform) Resources.Load(weaponID, typeof(Transform));
		
		Transform newWeapon = (Transform)Instantiate(spawn, position, rotation);
		newWeapon.rigidbody.AddForce(fwd);
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
			Application.LoadLevel("Main Menu");
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
		Application.LoadLevel("Main Menu");
	}
	
	[RPC]
	public void MainMenu()
	{
		networkView.RPC("Disconnect", RPCMode.Others);
		Network.Disconnect(200);
		Application.LoadLevel("Main Menu");
	}
}
