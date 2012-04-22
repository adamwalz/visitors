using UnityEngine;
using System.Collections;

public class MainMenuImageGui : MonoBehaviour {
	
	private GUITexture _mainMenuGUITexture;
	private GUITexture _singlePlayerGUITexture;
	private GUITexture _multiPlayerGUITexture;
	private GUITexture _optionsGUITexture;
	private GUITexture _activeButton;
	
	// Use this for initialization
	void Start () 
	{
		// Setup background
		_mainMenuGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_mainMenuGUITexture.transform.position = new Vector3(0, 0, -100000.0f);
		_mainMenuGUITexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
		Texture tex = (Texture2D)Resources.Load("menu_screen_no_text", typeof(Texture2D));
		_mainMenuGUITexture.texture = tex;
		
		// Setup single player button
		_singlePlayerGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_singlePlayerGUITexture.transform.position = new Vector3(0.5f, 0.2f, 0.0f);
		_singlePlayerGUITexture.pixelInset = new Rect(0, 0, 210, 210);
		tex = (Texture2D)Resources.Load("single_player", typeof(Texture2D));
		_singlePlayerGUITexture.texture = tex;
		
		// Setup multi player button
		_multiPlayerGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_multiPlayerGUITexture.transform.position = new Vector3(0.23f, 0.225f, 0.0f);
		_multiPlayerGUITexture.pixelInset = new Rect(0, 0, 140, 140);
		tex = (Texture2D)Resources.Load("multi_player", typeof(Texture2D));
		_multiPlayerGUITexture.texture = tex;
		
		// Setup options button
		_optionsGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_optionsGUITexture.transform.position = new Vector3(0.38f, -0.1f, 0.0f);
		_optionsGUITexture.pixelInset = new Rect(0, 0, 180, 180);
		tex = (Texture2D)Resources.Load("options", typeof(Texture2D));
		_optionsGUITexture.texture = tex;
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleInput();
		_mainMenuGUITexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
	}
	
	// Self-explanatory, called from Update()
	private void HandleInput()
	{
		// If mouse is down, highlight any button that it hits and set that as the "active" button
		if(Input.GetMouseButtonDown(0))
		{
			if(_singlePlayerGUITexture.HitTest(Input.mousePosition))
			{
				_singlePlayerGUITexture.texture = (Texture2D)Resources.Load("single_player_hover");
				_activeButton = _singlePlayerGUITexture;
			}
			if(_multiPlayerGUITexture.HitTest(Input.mousePosition))
			{
				_multiPlayerGUITexture.texture = (Texture2D)Resources.Load("multi_player_hover");
				_activeButton = _multiPlayerGUITexture;
			}
			if(_optionsGUITexture.HitTest(Input.mousePosition))
			{
				_optionsGUITexture.texture = (Texture2D)Resources.Load("options_hover");
				_activeButton = _optionsGUITexture;		
			}
		}
		
		// If mouse is released, set all buttons to their non-highlighted state. If the active button
		// (the one that the user initially pressed) has been released, call the appropriate pressed
		// method on our controller.
		if(Input.GetMouseButtonUp(0))
		{
			_singlePlayerGUITexture.texture = (Texture2D)Resources.Load("single_player");
			_multiPlayerGUITexture.texture = (Texture2D)Resources.Load("multi_player");
			_optionsGUITexture.texture = (Texture2D)Resources.Load("options");
			
			if(_singlePlayerGUITexture.HitTest(Input.mousePosition))
			{
				 if(_activeButton == _singlePlayerGUITexture)
				{
					GameState.SetIsServer(false);
					Application.LoadLevel("PlanetEarthScene");
				}
			}
			if(_multiPlayerGUITexture.HitTest(Input.mousePosition))
			{
				if(_activeButton == _multiPlayerGUITexture) 
				{
					GameState.SetIsServer(true);
					GameState.SetPlayerNumber(2);
					Application.LoadLevel("PlanetEarthScene");
				}
			}
			if(_optionsGUITexture.HitTest(Input.mousePosition))
			{
				if(_activeButton == _optionsGUITexture)
					print("No options for you!");
			}
		}	
	}
	
}
