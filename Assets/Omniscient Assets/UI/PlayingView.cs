using UnityEngine;
using System.Collections;

public class PlayingView : GameView 
{
	ButtonView _pauseButton;
	ButtonView _switchWeaponButton;
	ButtonView _fireButton;
	ImageView _crosshairImage;
	WeaponBar _weaponBar;
	WeaponSwitcher _switcher;
	
	public ButtonView PauseButton
	{
		get{return _pauseButton;}	
	}
	
	public ButtonView SwitchWeaponButton
	{
		get{return _switchWeaponButton;}	
	}
	
	public ButtonView FireButton
	{
		get{return _fireButton;}	
	}
	
	public WeaponSwitcher Switcher
	{
		get{return _switcher;}	
	}
	
	public WeaponBar WeaponBar
	{
		get{return _weaponBar;}	
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
		_crosshairImage.TextureName = "crosshair";
		AddSubview(_crosshairImage);
		
		_switchWeaponButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_switchWeaponButton.Init();
		_switchWeaponButton.ButtonImageName = "switchWeaponButton";
		_switchWeaponButton.HighlightImageName = "switchWeaponButtonPressed";
		AddSubview(_switchWeaponButton);
		
		_fireButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_fireButton.Init();
		_fireButton.ButtonImageName = "fireButton";
		_fireButton.HighlightImageName = "fireButtonPressed";
		AddSubview(_fireButton);
		
		_weaponBar = (WeaponBar)gameObject.AddComponent("WeaponBar");
		_weaponBar.Init();
		AddSubview(_weaponBar);
		
		_switcher = (WeaponSwitcher)gameObject.AddComponent("WeaponSwitcher");
		_switcher.Init();
		AddSubview(_switcher);
		
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
			
			_crosshairImage.Size = new Vector2(50, 50);
			_crosshairImage.Position = new Vector2(0, 0);
		
			_switchWeaponButton.Size = new Vector2(100, 100);
			Vector2 switchWeaponButtonPosition = AnchorOffset(GameView.GameViewAnchor.BottomLeftAnchor);
			switchWeaponButtonPosition += new Vector2(10, -10);
			_switchWeaponButton.SetPosition(switchWeaponButtonPosition, GameViewAnchor.BottomLeftAnchor);
		
			_fireButton.Size = new Vector2(100, 100);
			Vector2 fireButtonPosition = AnchorOffset(GameView.GameViewAnchor.BottomRightAnchor);
			fireButtonPosition += new Vector2(-10, -10);
			_fireButton.SetPosition(fireButtonPosition, GameViewAnchor.BottomRightAnchor);
			
			_weaponBar.Size = new Vector2(250, 60);
			_weaponBar.SetPosition(AnchorOffset(GameView.GameViewAnchor.BottomMiddleAnchor), GameView.GameViewAnchor.BottomMiddleAnchor);
			_weaponBar.Position = _weaponBar.Position + new Vector2(0, 20);
			
			_switcher.Size = new Vector2(130, 60);
			_switcher.SetPosition(_switchWeaponButton.GetPosition(GameView.GameViewAnchor.TopLeftAnchor), GameView.GameViewAnchor.BottomMiddleAnchor);
			_switcher.Position += new Vector2(20.0f, -20.0f);
		}
		
		if(State == GameView.GameViewState.AnimatingOut)
		{
			_crosshairImage.Size = new Vector2(50 + (_animationTimer / _animationDuration) * 50, 50  + (_animationTimer / _animationDuration) * 50);
		}
	}
}
