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
		
		_listView = (NetworkGameListView)gameObject.AddComponent("NetworkGameListView");
		_listView.Init();
		_listView.Size = new Vector2(_mainScreen.Size.x - 60, 90);
		_listView.SetPosition(new Vector2(30, 222), GameView.GameViewAnchor.TopLeftAnchor);
		_listView.ListItemPressed += new ListEventHandler(JoinButtonPressed);
		_mainScreen.AddView(_listView);
		_listView.Show(false);
		
		RefreshGames();
	}
	
	public void JoinButtonPressed(object sender, int index)
	{
		ConnectToGame(index);
	}
	
	public void RefreshPressed(object sender)
	{
		RefreshGames();
	}
	
	public void RefreshGames()
	{
		lastRefreshTime = Time.realtimeSinceStartup;
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