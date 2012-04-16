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
var refreshTimer = 10.0;

function OnGUI ()
{
	// Checking if you are connected to the server or not
	if (Network.peerType == NetworkPeerType.Disconnected)
	{
		gameName = GUI.TextField(new Rect(120,105,100,20),gameName);
		GUI.Label (Rect (10, 135, 100, 50), "Number of Players:");
		playerNumberString = GUI.TextField(new Rect(120, 140, 50, 20), playerNumberString);
		
		if (GUI.Button (new Rect(10,100,100,30),"Start Server"))
		{
			
			
			if ((int.TryParse(playerNumberString, playerNumberInt)) && (playerNumberInt > 1))
			{
				try
				{
					Network.InitializeServer(32, listenPort, false);
					Network.maxConnections = playerNumberInt - 1;
					
					MasterServer.updateRate = 3;
					MasterServer.RegisterHost("BlockGame", gameName, "Open");
				
				
					// Notify our objects that the level and the network is ready
					for (var go : GameObject in FindObjectsOfType(GameObject))
					{
						// About OnNetworkLoadedLevel later
						go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
					}
				
					Application.LoadLevel("sampleHUDnetworking");
				}
				
				catch (err)
				{
					Debug.Log("Exception catched: " + err);;
				}
			}
		}
	
			
		

		//The next two If Statements deal with refreshing current Available Games
		if (Time.realtimeSinceStartup > lastRefreshTime + refreshTimer)
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("BlockGame");
		}
		
		if (GUI.Button (new Rect(10,60,210,30),"Refresh available Servers"))
		{
			lastRefreshTime = Time.realtimeSinceStartup;
			MasterServer.ClearHostList();
			MasterServer.RequestHostList("BlockGame");
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
			var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
			GUILayout.Label(name);
			GUILayout.Space(5);
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
			
			if (GUILayout.Button("Connect"))
			{
				// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
				Network.Connect(element);
				Application.LoadLevel("sampleHUDnetworking");			
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

