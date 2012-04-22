using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
	
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	private GameScreen _mainScreen;
	private SearchingView _searchingView;
	public PlayingView _playingView;
	private PauseMenu _pauseMenu;
	private GameEndMenu _gameEndMenu;
	private int currentLevel = -1;
	private int _weaponIndex = 0;
	private bool _searching;
	
	public AudioClip[] soundEffects = new AudioClip[2];
	//public AudioSource shootSound;
	//public AudioSource changeWeaponSound2;
	//public AudioClip changeWeaponSound;
	
	// Use this for initialization
	void Start () 
	{
		_searching = GameState.GetIsAugmented();
		
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
		
		_pauseMenu = (PauseMenu)gameObject.AddComponent("PauseMenu");
		_pauseMenu.Init();
		_pauseMenu.Size = _mainScreen.Size;
		_pauseMenu.Position = new Vector2(_mainScreen.Size.x / 2.0f, _mainScreen.Size.y / 2.0f);
		_pauseMenu.ResumeButton.ButtonPressed += new EventHandler(ResumePressed);
		_pauseMenu.MainMenuButton.ButtonPressed += new EventHandler(MenuPressed);
		_pauseMenu.ResetButton.ButtonPressed += new EventHandler(ResetPressed);
		_mainScreen.AddView(_pauseMenu);
		
		_gameEndMenu = (GameEndMenu)gameObject.AddComponent("GameEndMenu");
		_gameEndMenu.Init();
		_gameEndMenu.Size = _mainScreen.Size;
		_gameEndMenu.Position = new Vector2(_mainScreen.Size.x / 2.0f, _mainScreen.Size.y / 2.0f);
		_gameEndMenu.MainMenuButton.ButtonPressed += new EventHandler(MenuPressed);
		_gameEndMenu.ResetButton.ButtonPressed += new EventHandler(ResetPressed);
		_mainScreen.AddView(_gameEndMenu);
	}
	
	public void Update()
	{
		
		// If we are searching but the searching view is not showing...
		if(_searching && _searchingView.State != GameView.GameViewState.Showing)
		{
			// If the playing view is still showing, get rid of it
			if(_playingView.State == GameView.GameViewState.Showing)
			{
				_playingView.Hide(true);
			}
			// If the playing view is hidden, but the searching view is not showing, start showing it
			if(_playingView.State == GameView.GameViewState.Hidden && _searchingView.State != GameView.GameViewState.AnimatingIn)
			{
				_searchingView.Show(true);
			}
		}
		
		// If we are playing but the playing view is not showing...
		if(!_searching && _playingView.State != GameView.GameViewState.Showing)
		{
			// If the searching view is still showing, get rid of it
			if(_searchingView.State == GameView.GameViewState.Showing)
			{
				_searchingView.Hide(true);
			}
			// If the searching view is hidden, but the playing view is not showing, start showing it
			if(_searchingView.State == GameView.GameViewState.Hidden && _searchingView.State != GameView.GameViewState.AnimatingIn)
			{
				_playingView.Show(true);
			}
		}
		
		// End game stuff
		_playingView.WeaponBar.Energy -= Time.deltaTime;
		if(_playingView.WeaponBar.Energy == 0 && !(_gameEndMenu.State == GameView.GameViewState.Showing) && !(_gameEndMenu.State == GameView.GameViewState.AnimatingIn))
		{
			ShowEndGameMenu(false, 0);	
		}
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
		audio.clip = soundEffects[1];
		audio.Play();
		
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
		string scene = GameState.GetCurrentLevel();
		Debug.Log("Playing without AR in scene: " + scene);
		if (scene.Contains("Egypt"))
			GameState.SetCurrentLevel("SP-Egypt-NonAR", false);
		else if (scene.Contains("Castle"))
			GameState.SetCurrentLevel("SP-Castle-NonAR", false);
		Application.LoadLevel(GameState.GetCurrentLevel());
	}
	
	public void SwitchToPlayingView()
	{
		_searching = false;
	}
	public void SwitchToSearchingView()
	{
		_searching = true;
	}

	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
		_weaponIndex = newWeapon;
	}
	
	public void FireButtonPressed(object sender)
	{
		// Fire vector
		GameObject cam;
		cam = GameObject.Find("ARCamera");
		if (cam == null)
			cam = GameObject.Find("NonARCamera");
		if (cam == null)
		{
			Debug.Log("CAMERA NULL");
			return;
		}
		
		
		// Choose weapon
		bool primaryWeapon = (_weaponIndex == 0);
		GameObject weapon;
		if (primaryWeapon)
			weapon = (GameObject)Resources.Load(GameState.LoadPrimaryWeapon(),typeof(GameObject));
		else
			weapon = (GameObject)Resources.Load(GameState.LoadSecondaryWeapon(),typeof(GameObject));
		Debug.Log("Shooting: " + weapon.name + ". Primary: " + primaryWeapon);
		
		Shoot(cam.transform, weapon);
		
		// Shooting sounds
		audio.clip = soundEffects[0];
		audio.pitch = Random.Range(0.9F, 1.1F);
		audio.Play();
		
		// UI Changes
		_playingView.WeaponBar.Energy -= 1.0f;
	}
	
	
	public void Shoot(Transform fromObject, GameObject weapon)
	{
		float velocity = 500;
		
		// Instantiate weapon
		GameObject weaponSpawn = (GameObject)Instantiate(weapon, fromObject.position, fromObject.rotation);
		weaponSpawn.transform.rigidbody.AddForce(fromObject.forward * velocity);
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
	
	public void ShowEndGameMenu(bool winning, int score)
	{
		_searchingView.HasFocus = false;
		_playingView.HasFocus = false;
		_gameEndMenu.Victory = winning;
		_gameEndMenu.Score = score;
		_gameEndMenu.Show(true);
	}
}