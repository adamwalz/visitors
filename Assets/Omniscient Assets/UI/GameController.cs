using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	private GameScreen _mainScreen;
	private SearchingView _searchingView;
	private PlayingView _playingView;
	private PauseMenu _pauseMenu;
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
		ResetLevel();
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
	
	public void SwitchGameView()
	{
		bool searchingViewShowing = (_searchingView.State == GameView.GameViewState.Showing);
		bool searchingViewAnimating = (_searchingView.State == GameView.GameViewState.AnimatingIn);
		
		if (searchingViewShowing || searchingViewAnimating)
			StartCoroutine(TransitionToPlayingView());
		else
			StartCoroutine(TransitionToSearchingView());	
	}
	
	IEnumerator TransitionToPlayingView()
	{
		
		Debug.Log("Got here from trackable");
		_searchingView.Hide(true);
		yield return new WaitForSeconds(_searchingView.AnimationDuration);
		_playingView.Show(true);
	}
	
	IEnumerator TransitionToSearchingView()
	{
		Debug.Log("Got here from trackable");
		_playingView.Hide(true);
		yield return new WaitForSeconds(_searchingView.AnimationDuration);
		_searchingView.Show(true);
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
		

	
		
		GameObject cam = GameObject.Find("ARCamera");
		Vector3 fwd = cam.transform.forward * 50000;
		ShootWithoutNetworkInstantiate(cam.transform.position, cam.transform.rotation, fwd);
		_playingView.WeaponBar.Energy = _playingView.WeaponBar.Energy - 1.0f;

		
		//networkView.RPC("ShootWithoutNetworkInstantiate",RPCMode.All);
		
		/*
		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		GameObject instance = (GameObject)Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
		Vector3 fwd = cam.transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
		*/
	}
	
	
	public void ShootWithoutNetworkInstantiate(Vector3 position, Quaternion rotation, Vector3 fwd)
	{
		bool primaryWeapon = (_weaponIndex == 0);
		string weaponID = "";
		if (primaryWeapon)
			weaponID = GameState.LoadPrimaryWeapon();
		else
			weaponID = GameState.LoadSecondaryWeapon();
		Debug.Log("Shooting: " + weaponID + ". Primary: " + primaryWeapon);
		Transform spawn = (Transform) Resources.Load(weaponID, typeof(Transform));
		
		Transform newWeapon = (Transform)Instantiate(spawn, position, rotation);
		newWeapon.rigidbody.AddForce(fwd);
	}
	
	public void HUDGameViewMenuButtonPressed()
	{
		Application.LoadLevel("Main Menu");
	}
	
	public void ResetLevel()
	{
		if (currentLevel != Application.loadedLevel)
		{
			currentLevel = Application.loadedLevel;
		}
		Application.LoadLevel(currentLevel);
	}
}