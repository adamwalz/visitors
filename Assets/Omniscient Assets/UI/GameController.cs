using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	
	private	GUIText clientMessage;
	private	GUIText serverMessage;
	private GameScreen _mainScreen;
	private SearchingView _searchingView;
	private PlayingView _playingView;
	
	// Use this for initialization
	void Start () 
	{
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_searchingView = (SearchingView)gameObject.AddComponent("SearchingView");
		_searchingView.Init();
		_searchingView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_searchingView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_searchingView.PauseButton.ButtonPressed += new EventHandler(PauseButtonPressed);
		_searchingView.PrintButton.ButtonPressed += new EventHandler(PrintButtonPressed);
		_searchingView.PlayWithoutButton.ButtonPressed += new EventHandler(PlayWithoutButtonPressed);
		_mainScreen.AddView(_searchingView);
		
		_playingView = (PlayingView)gameObject.AddComponent("PlayingView");
		_playingView.Init();
		_playingView.Size = new Vector2(_mainScreen.Size.x, _mainScreen.Size.y);
		_playingView.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_playingView.PauseButton.ButtonPressed += new EventHandler(PauseButtonPressed);
		_playingView.SwitchWeaponButton.ButtonPressed += new EventHandler(SwitchWeaponButtonPressed);
		_mainScreen.AddView(_playingView);
		_searchingView.Show(true);
	}
	
	public void SwitchWeaponButtonPressed(object sender)
	{
		if(_playingView.Switcher.CurrentWeapon == 0) _playingView.Switcher.CurrentWeapon = 1;
		else _playingView.Switcher.CurrentWeapon = 0;
	}
	
	public void PauseButtonPressed(object sender)
	{
		//Multiplayer Server
		if(Network.peerType == NetworkPeerType.Server)
		{
			Application.LoadLevel("sampleHUDnetworking");
		}
		
		//Multiplayer Client
		if(Network.peerType == NetworkPeerType.Client)
		{

			
			//clientMessage.gameObject.active = true;
			
			Instantiate(clientMessage);
			clientMessage.text = "A Message has been sent to Player 1 Requesting a Level Reset";
		
			StartCoroutine(ClientMessageTimer());
			
			//Send Message to Server
			networkView.RPC("PrintSeverMessage", RPCMode.Server);
		}
		
		//singlePlayer
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			Application.LoadLevel("sampleHUD");
		}
	}
	
	public void PrintButtonPressed(object sender)
	{
		UtilityPlugin.PrintARCard();
	}
	
	public void TransitionButtonPressed(object sender)
	{
		StartCoroutine(TransitionToSearchingView());	
	}
	
	public void PlayWithoutButtonPressed(object sender)
	{
		StartCoroutine(TransitionToPlayingView());
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
	
	//Timer Functions to delete the message/// 
	IEnumerator ClientMessageTimer()
	{
		yield return new WaitForSeconds(2);
		
		//Deactivate Message
		//clientMessage.gameObject.active = false;
		Destroy(clientMessage);
	}

	IEnumerator ServerMessageTimer()
	{
		yield return new WaitForSeconds(2);
		
		//Deactivate Message
		//clientMessage.gameObject.active = false;
		Destroy(serverMessage);
	}
			
	// Update is called once per frame
	void Update () 
	{
	
	}
}
