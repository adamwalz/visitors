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
	private ArrayList _joinableGames;
	private NetworkGameListView _listView;
	private bool _waitingToJoin;
	private HostData _currentGame;
	
	// Use this for initialization
	void Start () 
	{
		// Push our current scene onto the game state (so we can go back to it with a back button)
		GameState.PushScene(Application.loadedLevelName);
		
		_joinableGames = new ArrayList();
		_waitingToJoin = false;
		
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_connectionView = (ConnectionView)gameObject.AddComponent("ConnectionView");
		_connectionView.Init();
		_connectionView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_connectionView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_connectionView.CreateGameButton.ButtonPressed += new EventHandler(CreatePressed);
		_connectionView.RefreshButton.ButtonPressed += new EventHandler(RefreshPressed);
		_connectionView.BackButton.ButtonPressed += new EventHandler(BackPressed);
		_mainScreen.AddView(_connectionView);
		_connectionView.Show(false);
		
		_listView = (NetworkGameListView)gameObject.AddComponent("NetworkGameListView");
		_listView.Init();
		_listView.Size = new Vector2(_mainScreen.Size.x - 60, 90);
		_listView.SetPosition(new Vector2(30, 222), GameView.GameViewAnchor.TopLeftAnchor);
		_listView.ListItemPressed += new ListEventHandler(JoinButtonPressed);
		_mainScreen.AddView(_listView);
		_listView.Show(false);
		
		// Reset all the things
		RefreshGames();
		GameState.ResetGameState();
	}
	
	public void BackPressed(object sender)
	{
		GameState.PopScene();
	}
	
	public void JoinButtonPressed(object sender, int index)
	{
		Debug.Log("The Game we should join is: " + ((HostData)_joinableGames[index]).gameName);
		ConnectToGame(index);
	}
	
	public void RefreshPressed(object sender)
	{
		RefreshGames();
	}
	
	public void RefreshGames()
	{
		MasterServer.ClearHostList();
		MasterServer.RequestHostList("Visitors");
	}
	
	public void Update()
	{
		HostData[] games = MasterServer.PollHostList();
				
		_joinableGames.Clear();
		foreach (HostData element in games)
		{
			if (element.playerLimit == element.connectedPlayers)
			{
				continue;
			}
			
			_joinableGames.Add(element);
		}
		
		ArrayList cellInfo = new ArrayList();
		
		foreach(HostData element in _joinableGames)
		{
			NetworkGameListCellInfo info = new NetworkGameListCellInfo();
			string[] lines = Regex.Split(element.comment, ",");
			
			info.GameName = element.gameName;
			info.LevelName = lines[0];
			info.PrimaryWeaponImageName = lines[1];
			info.SecondaryWeaponImageName = lines[2];
			info.GameDescription = "Players: " + element.connectedPlayers + " / " + element.playerLimit;
			cellInfo.Add(info);
		}
		
		_listView.InfoForCells = (NetworkGameListCellInfo[])cellInfo.ToArray(typeof(NetworkGameListCellInfo));
	}
		
	public void ConnectToGame(int gameIndex)
	{
		HostData element = (HostData)_joinableGames[gameIndex];
		
		// Check all the things
		RefreshGames();
		if (   !(((HostData)_joinableGames[gameIndex]).gameName).Equals(element.gameName)
			|| !(((HostData)_joinableGames[gameIndex]).connectedPlayers).Equals(element.connectedPlayers)
			|| !(((HostData)_joinableGames[gameIndex]).ip).Equals(element.ip)
			|| !(((HostData)_joinableGames[gameIndex]).playerLimit).Equals(element.playerLimit))
			return;
		
		Debug.Log("Connection Hit, refresh occurs");
		Debug.Log("The Game we actually join is: " + ((HostData)_joinableGames[gameIndex]).gameName);
					
		string[] lines = Regex.Split(element.comment, ",");
		GameState.SetCurrentLevel(lines[0], true);
		GameState.SavePrimaryWeapon(lines[1]);
		GameState.SaveSecondaryWeapon(lines[2]);
		GameState.SetIsServer(false);
		
		_waitingToJoin = true;
		_currentGame = element;
		
	}
	
	void OnMasterServerEvent(MasterServerEvent theEvent)
	{
		if(theEvent == MasterServerEvent.HostListReceived)
		{
			if(_waitingToJoin == true)
			{
				Debug.Log("The Event recieved was: " + theEvent.ToString());
				HostData[] games = MasterServer.PollHostList();
				foreach(HostData data in games)
				{
					Debug.Log("Current Checking Host Data is: " + data.ToString());
					if(_currentGame.gameName == data.gameName)
					{
						Debug.Log("Connecting to Host Data: " + data.ToString());
						Network.Connect(_currentGame);
						GameState.GoToScene(_mainScreen, GameState.GetCurrentLevel());
					}
				}
				
				_waitingToJoin = false;
			}
		}
	}
	
	public void CreatePressed(object sender)
	{
		GameState.SetIsServer(true);
		GameState.SetPlayerNumber(_connectionView.NumberOfPlayersSelector.Number);
		Application.LoadLevel("PlanetEarthScene");
	}
}