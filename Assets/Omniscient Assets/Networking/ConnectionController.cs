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
	
	// Use this for initialization
	void Start () 
	{
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_connectionView = (ConnectionView)gameObject.AddComponent("ConnectionView");
		_connectionView.Init();
		_connectionView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_connectionView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_connectionView.CreateGameButton.ButtonPressed += new EventHandler(CreatePressed);
		_connectionView.RefreshButton.ButtonPressed += new EventHandler(RefreshPressed);
		_mainScreen.AddView(_connectionView);
		_connectionView.Show(false);
	}
	
	void Update()
	{
		if (Time.realtimeSinceStartup > lastRefreshTime + refreshTimer)
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Visitors");
		}
		
		/*if (GUI.Button (new Rect(10,500,200,100),"Show Joinable Games"))
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Visitors");
		}*/
		
		HostData[] joinableGames = MasterServer.PollHostList();
		foreach (HostData element in joinableGames)
		{
			if (element.playerLimit == element.connectedPlayers)
			{
				continue;
			}
			
			string gameInfo = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
			
			
			//If Connect
			/*{
				Network.Connect(element);
				
					
				string[] lines = Regex.Split(element.comment, ",");
				GameState.SetCurrentLevel(lines[0]);
				GameState.SavePrimaryWeapon(lines[1]);
				GameState.SaveSecondaryWeapon(lines[2]);
				GameState.SetIsServer(false);
				
				
				Application.LoadLevel(lines[0]);
			}*/
		}
	}
	
	public void RefreshPressed(object sender)
	{
		lastRefreshTime = Time.realtimeSinceStartup;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("Visitors");
	}
	
		
	
	
	public void CreatePressed(object sender)
	{
		GameState.SetIsServer(true);
		GameState.SetPlayerNumber(_connectionView.NumberOfPlayersSelector.Number);
		Application.LoadLevel("PlanetEarthScene");
	}
}