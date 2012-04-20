using UnityEngine;
using System.Collections;

public class GameEndMenu : GameView 
{
	private ImageView _black;
	private ImageView _menuBackground;
	private TextView _scoreText;
	private ButtonView _mainMenuButton;
	private ButtonView _resetButton;
	private bool _victory;
	private int _score;
	
	public bool Victory
	{
		get{return _victory;}
		set{_victory = value;}
	}
	
	public int Score
	{
		get{return _score;}
		set{_score = value;}
	}
	
	public ButtonView MainMenuButton
	{
		get{return _mainMenuButton;}	
	}
	
	public ButtonView ResetButton
	{
		get{return _resetButton;}	
	}
	
	public override void Init()
	{
		base.Init();
		
		_victory = false;
		_score = 0;
		
		_black = (ImageView)gameObject.AddComponent("ImageView");
		_black.Init();
		_black.TextureName = "black";
		AddSubview(_black);
		
		_menuBackground = (ImageView)gameObject.AddComponent("ImageView");
		_menuBackground.Init();
		AddSubview(_menuBackground);
		
		_scoreText = (TextView)gameObject.AddComponent("TextView");
		_scoreText.Init();
		_scoreText.Text = "Score: ???";
		_scoreText.FontSize = 30;
		AddSubview(_scoreText);
		
		_mainMenuButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_mainMenuButton.Init();
		_mainMenuButton.ButtonImageName = "miniMenuButton";
		_mainMenuButton.HighlightImageName = "miniMenuButton";
		AddSubview(_mainMenuButton);
		
		_resetButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_resetButton.Init();
		_resetButton.ButtonImageName = "miniResetButton";
		_resetButton.HighlightImageName = "miniResetButton";
		AddSubview(_resetButton);
	}
	
	public override void RefreshContent()
	{
		_black.Size = Size;
		_black.Position = new Vector2(0, 0);
		
		_menuBackground.Size = new Vector2(256, 165);
		_menuBackground.Position = new Vector2(0, 0);
		if(_victory)_menuBackground.TextureName = "VictoryBackground";
		else _menuBackground.TextureName = "FailureBackground";
		
		_scoreText.Position = new Vector2(0, 10);
		if(_victory)_scoreText.Text = "Score: " + _score;
		else _scoreText.Text = "";
		
		_mainMenuButton.Size = new Vector2(100, 40);
		_mainMenuButton.SetPosition(new Vector2(-110, -25), GameView.GameViewAnchor.TopLeftAnchor);
		
		_resetButton.Size = new Vector2(100, 40);
		_resetButton.SetPosition(new Vector2(110, -25), GameView.GameViewAnchor.TopRightAnchor);
	}
}
