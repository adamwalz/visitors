using UnityEngine;
using System.Collections;

public class ConnectionView : GameView
{
	private ImageView _background;
	private ImageView _joinGame;
	private ButtonView _refreshButton;
	private ImageView _createGame;
	private TextView _numberOfPlayers;
	private NumberSelectorView _numberSelectorView;
	private ButtonView _createGameButton;
	// private ButtonView _backButton;
	
	public override void Init()
	{
		base.Init();
		_background = (ImageView)gameObject.AddComponent("ImageView");
		_background.Init();
		_background.TextureName = "MultiplayerBackground";
		AddSubview(_background);
		
		_joinGame = (ImageView)gameObject.AddComponent("ImageView");
		_joinGame.Init();
		_joinGame.TextureName = "JoinGame";
		AddSubview(_joinGame);
		
		_refreshButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_refreshButton.Init();
		_refreshButton.ButtonImageName = "refreshButton";
		_refreshButton.HighlightImageName = "refreshButtonHighlight";
		AddSubview(_refreshButton);
		
		_createGame = (ImageView)gameObject.AddComponent("ImageView");
		_createGame.Init();
		_createGame.TextureName = "CreateGame";
		AddSubview(_createGame);
		
		_createGameButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_createGameButton.Init();
		_createGameButton.ButtonImageName = "create";
		_createGameButton.HighlightImageName = "createHighlight";
		AddSubview(_createGameButton);
		
		_numberOfPlayers = (TextView)gameObject.AddComponent("TextView");
		_numberOfPlayers.Init();
		_numberOfPlayers.Text = "Number of players: ";
		AddSubview(_numberOfPlayers);
		
		_numberSelectorView = (NumberSelectorView)gameObject.AddComponent("NumberSelectorView");
		_numberSelectorView.Init();
		_numberSelectorView.Number = 1;
		_numberSelectorView.MinimumNumber = 2;
		_numberSelectorView.MaximumNumber = 4;
		AddSubview(_numberSelectorView);
		
		/*
		_backButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_backButton.Init();
		_backButton.ButtonImageName = "backNavigationButton";
		_backButton.ButtonImageName = "backNavigationButtonHighlight";
		*/
	}
	
	public ButtonView CreateGameButton
	{
		get{return _createGameButton;}	
	}
	
	public ButtonView RefreshButton
	{
		get{return _refreshButton;}	
	}
	
	public NumberSelectorView NumberOfPlayersSelector
	{
		get{return _numberSelectorView;}
	}
	
	public override void RefreshContent()
	{
		_background.Size = Size;
		_background.Position = new Vector2(0, 0);
		
		_joinGame.Size = new Vector2(391.0f / 2.0f, 69.0f / 2.0f);
		Vector2 joinGamePosition = AnchorOffset(GameView.GameViewAnchor.TopLeftAnchor) + new Vector2(20, -60);
		_joinGame.SetPosition(joinGamePosition, GameView.GameViewAnchor.TopLeftAnchor);
		
		_refreshButton.Size = new Vector2(35, 35);
		Vector2 refreshButtonPosition = _joinGame.GetPosition(GameView.GameViewAnchor.MiddleRightAnchor) + new Vector2(20, 0);
		_refreshButton.SetPosition(refreshButtonPosition, GameView.GameViewAnchor.MiddleLeftAnchor);
		
		_createGame.Size = new Vector2(492.0f / 2.0f, 69.0f / 2.0f);
		Vector2 createGamePosition = AnchorOffset(GameView.GameViewAnchor.BottomLeftAnchor) + new Vector2(20, +100);
		_createGame.SetPosition(createGamePosition, GameView.GameViewAnchor.BottomLeftAnchor);
				
		_numberSelectorView.Size = new Vector2(80, 100);
		Vector2 numberSelectorPosition = AnchorOffset(GameView.GameViewAnchor.BottomMiddleAnchor) + new Vector2(20, 0);
		_numberSelectorView.SetPosition(numberSelectorPosition, GameView.GameViewAnchor.BottomMiddleAnchor);
		
		Vector2 numberOfPlayersPosition = AnchorOffset(GameView.GameViewAnchor.BottomLeftAnchor) + new Vector2(0, _numberSelectorView.Size.y / 2);
		_numberOfPlayers.InternalGUIText.anchor = TextAnchor.MiddleLeft;
		_numberOfPlayers.Position = numberOfPlayersPosition;
		
		_createGameButton.Size = new Vector2(320.0f / 2.5f, 97.0f / 2.5f);
		Vector2 createGameButtonPosition = _numberSelectorView.GetPosition(GameView.GameViewAnchor.MiddleRightAnchor);
		createGameButtonPosition += new Vector2(10, 0);
		_createGameButton.SetPosition(createGameButtonPosition, GameView.GameViewAnchor.MiddleLeftAnchor);
	}
}
