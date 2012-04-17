using UnityEngine;
using System.Collections;

public class SearchingView : GameView
{
	ButtonView _pauseButton;
	ButtonView _printButton;
	ButtonView _playWithoutButton;
	ImageView _crosshairImage;
	
	public ButtonView PauseButton
	{
		get{return _pauseButton;}	
	}
	
	public ButtonView PrintButton
	{
		get{return _printButton;}	
	}
	
	public ButtonView PlayWithoutButton
	{
		get{return _playWithoutButton;}	
	}
	
	public override void Init()
	{
		base.Init();
		
		_overrideDefaultShowBehavior = true;
		_overrideDefaultHideBehavior = true;
		
		_pauseButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_pauseButton.Init();
		_pauseButton.ButtonImageName = "pauseButton";
		_pauseButton.HighlightImageName = "pauseButtonPressed";
		AddSubview(_pauseButton);
		
		_crosshairImage = (ImageView)gameObject.AddComponent("ImageView");
		_crosshairImage.Init();
		_crosshairImage.TextureName = "crosshairSearching";
		AddSubview(_crosshairImage);
		
		_printButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_printButton.Init();
		_printButton.ButtonImageName = "printButton";
		_printButton.HighlightImageName = "printButtonPressed";
		AddSubview(_printButton);
		
		_playWithoutButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_playWithoutButton.Init();
		_playWithoutButton.ButtonImageName = "playWithoutButton";
		_playWithoutButton.HighlightImageName = "playWithoutButtonPressed";
		AddSubview(_playWithoutButton);
	}
	
	public override void Show(bool animated)
	{
		base.Show(animated);
		foreach(GameView view in _subviews)
		{
			if(view != _crosshairImage && view != _pauseButton) view.Show(animated);	
		}
		_crosshairImage.Show(false);
		_pauseButton.Show(false);
	}
	
	public override void Hide(bool animated)
	{
		base.Hide(animated);
		foreach(GameView view in _subviews)
		{
			if(view != _crosshairImage && view != _pauseButton) view.Hide(animated);	
		}
	}
	
	protected override void OnHidden()
	{
		_crosshairImage.Hide(false);
		_pauseButton.Hide(false);
	}
	
	public override void RefreshContent()
	{
		if(State == GameView.GameViewState.Showing || State == GameView.GameViewState.AnimatingIn)
		{
			_pauseButton.Size = new Vector2(60, 60);
			Vector2 pauseButtonPosition = AnchorOffset(GameView.GameViewAnchor.TopRightAnchor);
			pauseButtonPosition -= new Vector2(0, 0);
			_pauseButton.SetPosition(pauseButtonPosition, GameViewAnchor.TopRightAnchor);
		
			_crosshairImage.Size = new Vector2(100, 100);
			_crosshairImage.Position = new Vector2(0, 0);
		
			_printButton.Size = new Vector2(100, 100);
			Vector2 printButtonPosition = AnchorOffset(GameView.GameViewAnchor.BottomLeftAnchor);
			printButtonPosition += new Vector2(10, -10);
			_printButton.SetPosition(printButtonPosition, GameViewAnchor.BottomLeftAnchor);
		
			_playWithoutButton.Size = new Vector2(100, 100);
			Vector2 playWithoutButtonPosition = AnchorOffset(GameView.GameViewAnchor.BottomRightAnchor);
			playWithoutButtonPosition += new Vector2(-10, -10);
			_playWithoutButton.SetPosition(playWithoutButtonPosition, GameViewAnchor.BottomRightAnchor);
		}
		
		if(State == GameView.GameViewState.AnimatingOut)
		{
			_crosshairImage.Size = new Vector2(100 - (_animationTimer / _animationDuration) * 50, 100 - (_animationTimer / _animationDuration) * 50);
		}
	}
}
