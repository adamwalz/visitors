// IP Address of the server
var remoteIP = "127.0.0.1";
// Port of the server (to connect)
var remotePort = 25000;
// Port of the server (to listen)
var listenPort = 25000;
// For enabling/disabling NAT usage
var useNAT = false;
// Variables to show your IP and Port
var yourIP = "";
var yourPort = "";
var gameName = "Game Name";
var playerNumberString = "2";
var playerNumberInt : int; 
var lastRefreshTime = -1000.0;
var refreshTimer = 0.5;

var nextLevel = "MP-Castle-AR";

function OnGUI ()
{
	// Checking if you are connected to the server or not
	if (Network.peerType == NetworkPeerType.Disconnected)
	{
		//gameName = GUI.TextField(new Rect(120,105,100,20),gameName);
		gameName = SystemInfo.deviceName;
		GUI.Label (Rect (350, 510, 100, 50), "Number of Players:");
		playerNumberString = GUI.TextField(new Rect(350, 550, 50, 30), playerNumberString);
		
		if (GUI.Button (new Rect(220,500,120,100),"Start New Game"))
		{
			
			Application.LoadLevel("PlanetEarthScene");
			/*if ((int.TryParse(playerNumberString, playerNumberInt)) && (playerNumberInt > 1) && (playerNumberInt < 5))
			{
				try
				{
					Network.InitializeServer(32, listenPort, false);
					Network.maxConnections = playerNumberInt - 1;
					
					MasterServer.updateRate = 3;
					MasterServer.RegisterHost("Visitors", gameName, "Open");
				
				
					// Notify our objects that the level and the network is ready
					for (var go : GameObject in FindObjectsOfType(GameObject))
					{
						// About OnNetworkLoadedLevel later
						go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
					}
				
					Application.LoadLevel(nextLevel);
				}
				
				catch (err)
				{
					Debug.Log("Exception catched: " + err);
				}
			}*/
		}
	
			
		

		//The next two If Statements deal with refreshing current Available Games
		if (Time.realtimeSinceStartup > lastRefreshTime + refreshTimer)
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Visitors");
		}
		
		if (GUI.Button (new Rect(10,500,200,100),"Show Joinable Games"))
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("Visitors");
		}
	
		var data : HostData[] = MasterServer.PollHostList();
		
		// Go through all the hosts in the host list
		for (var element in data)
		{
			if (element.playerLimit == element.connectedPlayers)
			{
				continue;
			}
			
			GUILayout.BeginHorizontal();
			var availableGamesSyle:GUIStyle = new GUIStyle();
			availableGamesSyle.fontSize = 30;
			
			var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
			GUILayout.Label(name, availableGamesSyle);
			
			
			/*GUILayout.Space(5);
			var hostInfo : String;
			hostInfo = "[";
			
			for (var host in element.ip)
			{
				hostInfo = hostInfo + host + ":" + element.port + " ";
			}
				
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);
			GUILayout.Space(5);
			GUILayout.Label(element.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			*/
			GUILayout.Space(10);
			if (GUILayout.Button("Connect"))
			{
				// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
				Network.Connect(element);
				Application.LoadLevel(nextLevel);			
			}
			
			GUILayout.EndHorizontal();	
		}
	
	}
	
	else
	{
		// If connected
		
		// Getting your ip address and port
		var ipaddress = Network.player.ipAddress;
		var port = Network.player.port.ToString();
		
		//GUI.Label(new Rect(140,20,250,40),"IP Adress: "+ipaddress+":"+port);
		
		if (GUI.Button (new Rect(10,10,100,50),"Disconnect"))
		{
			// Disconnect from the server
			Network.Disconnect(200);
			Destroy(GameObject.Find("Player(Clone)"));
			print("Disconnect");
		}
	}
}

function OnConnectedToServer() 
{
	// Notify our objects that the level and the network are ready
	for (var go : GameObject in FindObjectsOfType(GameObject))
		go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);		
}

