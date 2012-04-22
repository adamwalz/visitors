using UnityEngine;
using System.Collections;

public class PauseMenu : GameView
{
	
	private ImageView _black;
	private ImageView _background;
	private ButtonView _mainMenu;
	private ButtonView _reset;
	private ButtonView _resume;
	
	public ButtonView MainMenuButton
	{
		get{return _mainMenu;}	
	}
	public ButtonView ResetButton
	{
		get{return _reset;}	
	}
	public ButtonView ResumeButton
	{
		get{return _resume;}	
	}
	
	public override void Init()
	{
		base.Init();
		
		_black = (ImageView)gameObject.AddComponent("ImageView");
		_black.Init();
		_black.TextureName = "black";
		AddSubview(_black);
		
		_background = (ImageView)gameObject.AddComponent("ImageView");
		_background.Init();
		_background.TextureName = "PauseBackground";
		AddSubview(_background);
		
		_mainMenu = (ButtonView)gameObject.AddComponent("ButtonView");
		_mainMenu.Init();
		_mainMenu.ButtonImageName = "main menu";
		_mainMenu.HighlightImageName = "main menuHighlight";
		AddSubview(_mainMenu);
		
		_reset = (ButtonView)gameObject.AddComponent("ButtonView");
		_reset.Init();
		_reset.ButtonImageName = "reset";
		_reset.HighlightImageName = "resetHighlight";
		AddSubview(_reset);
		
		_resume = (ButtonView)gameObject.AddComponent("ButtonView");
		_resume.Init();
		_resume.ButtonImageName = "resume";
		_resume.HighlightImageName = "resumeHighlight";
		AddSubview(_resume);
	}
	
	public override void RefreshContent()
	{
		
		_black.Size = Size;
		_black.Position = new Vector2(0, 0);
		
		_background.Size = new Vector2(256, 256);
		_background.Position = new Vector2(0, 0);
		
		_mainMenu.Size = new Vector2(200, 60);
		_mainMenu.SetPosition(new Vector2(0, 50), GameView.GameViewAnchor.TopMiddleAnchor);
		
		_reset.Size = new Vector2(200, 60);
		_reset.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.TopMiddleAnchor);
		
		_resume.Size = new Vector2(200, 60);
		_resume.SetPosition(new Vector2(0, -50), GameView.GameViewAnchor.TopMiddleAnchor);
		
	}
	

}
