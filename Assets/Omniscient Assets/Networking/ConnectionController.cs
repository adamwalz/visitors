using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ConnectionController : MonoBehaviour 
{
	private GameScreen _mainScreen;
	private ConnectionView _connectionView;
	private float lastRefreshTime = -1000.0F;
	private float refreshTimer = 0.5F;
	private ButtonView _joinGameOneButton;
	private ButtonView _joinGameTwoButton;
	private ArrayList _joinableGames;
	
	// Use this for initialization
	void Start () 
	{
		_joinableGames = new ArrayList();
		
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_connectionView = (ConnectionView)gameObject.AddComponent("ConnectionView");
		_connectionView.Init();
		_connectionView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_connectionView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_connectionView.CreateGameButton.ButtonPressed += new EventHandler(CreatePressed);
		_connectionView.RefreshButton.ButtonPressed += new EventHandler(RefreshPressed);
		_mainScreen.AddView(_connectionView);
		_connectionView.Show(false);
		
		_joinGameOneButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_joinGameOneButton.Init();
		_joinGameOneButton.ButtonImageName = "join";
		_joinGameOneButton.HighlightImageName = "joinHighlight";
		_joinGameOneButton.Size = new Vector2(100, 30);
		_joinGameOneButton.Position = new Vector2(_mainScreen.Size.x / 2.0f - 100, _mainScreen.Size.y / 2.0f);
		_joinGameOneButton.ButtonPressed += new EventHandler(JoinButtonPressed);
		_mainScreen.AddView(_joinGameOneButton);
		_joinGameOneButton.Show(false);
		
		_joinGameTwoButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_joinGameTwoButton.Init();
		_joinGameTwoButton.ButtonImageName = "join";
		_joinGameTwoButton.HighlightImageName = "joinHighlight";
		_joinGameTwoButton.Size = new Vector2(100, 30);
		_joinGameTwoButton.Position = new Vector2(_mainScreen.Size.x / 2.0f + 100, _mainScreen.Size.y / 2.0f);
		_joinGameTwoButton.ButtonPressed += new EventHandler(JoinButtonPressed);

		_mainScreen.AddView(_joinGameTwoButton);
		_joinGameTwoButton.Show(false);
	}
	
	public void JoinButtonPressed(object sender)
	{
		if(sender == _joinGameOneButton)ConnectToGame(0);
		if(sender == _joinGameTwoButton)ConnectToGame(1);
	}
	
	void Update()
	{
		if (Time.realtimeSinceStartup > lastRefreshTime + refreshTimer)
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Visitors");
		}
	
		HostData[] games = MasterServer.PollHostList();
		_joinableGames.Clear();
		foreach (HostData element in games)
		{
			if (element.playerLimit == element.connectedPlayers)
			{
				continue;
			}
			
			_joinableGames.Add(element);
			string gameInfo = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
		}
		
		_joinGameOneButton.Disabled = true;
		_joinGameTwoButton.Disabled = true;
		
		if(_joinableGames.Count > 0) _joinGameOneButton.Disabled = false;
		if(_joinableGames.Count > 1) _joinGameTwoButton.Disabled = false;
		
	}
	
	public void RefreshPressed(object sender)
	{
		lastRefreshTime = Time.realtimeSinceStartup;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("Visitors");
	}
	
		
	public void ConnectToGame(int gameIndex)
	{
		HostData element = (HostData)_joinableGames[gameIndex];
		Network.Connect(element);
					
		string[] lines = Regex.Split(element.comment, ",");
		GameState.SetCurrentLevel(lines[0], true);
		GameState.SavePrimaryWeapon(lines[1]);
		GameState.SaveSecondaryWeapon(lines[2]);
		GameState.SetIsServer(false);
				
		Application.LoadLevel(lines[0]);
	}
	
	
	public void CreatePressed(object sender)
	{
		GameState.SetIsServer(true);
		GameState.SetPlayerNumber(_connectionView.NumberOfPlayersSelector.Number);
		Application.LoadLevel("PlanetEarthScene");
	}
}