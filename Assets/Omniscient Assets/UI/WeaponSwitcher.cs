using UnityEngine;
using System.Collections;

public class WeaponSwitcher : GameView
{

	private ButtonView _weaponOne;
	private ButtonView _weaponTwo;
	private ImageView _weaponHighlighter;
	private int _currentWeapon;
	public EventHandler WeaponSwitched;
	
	public int CurrentWeapon
	{
		get{return _currentWeapon;}
		set{_currentWeapon = value;}
	}
	
	public override void Init()
	{
		base.Init();
		
		_currentWeapon = 0;
		
		_weaponHighlighter = (ImageView)gameObject.AddComponent("ImageView");
		_weaponHighlighter.Init();
		_weaponHighlighter.TextureName = "weaponHighlighter";
		AddSubview(_weaponHighlighter);
		
		_weaponOne = (ButtonView)gameObject.AddComponent("ButtonView");
		_weaponOne.Init();
		_weaponOne.ButtonImageName = GameState.LoadPrimaryWeapon();
		_weaponOne.HighlightImageName = GameState.LoadPrimaryWeapon();
		_weaponOne.ButtonPressed += new EventHandler(ButtonPressed);
		AddSubview(_weaponOne);
		
		_weaponTwo = (ButtonView)gameObject.AddComponent("ButtonView");
		_weaponTwo.Init();
		_weaponTwo.ButtonImageName = GameState.LoadSecondaryWeapon();
		_weaponTwo.HighlightImageName = GameState.LoadSecondaryWeapon();
		_weaponTwo.ButtonPressed += new EventHandler(ButtonPressed);
		AddSubview(_weaponTwo);
	}
	
	private void ButtonPressed(object sender)
	{
		if(sender == _weaponOne && _currentWeapon != 0)
		{
			_currentWeapon = 0;
			if(WeaponSwitched != null) WeaponSwitched(this);
		}
		if(sender == _weaponTwo && _currentWeapon != 1)
		{
			_currentWeapon = 1;
			if(WeaponSwitched != null) WeaponSwitched(this);
		}
	}
	
	public override void RefreshContent()
	{
		_weaponOne.Size = new Vector2(Size.y, Size.y);
		_weaponOne.SetPosition(new Vector2(- Size.x / 2.0f , 0), GameView.GameViewAnchor.MiddleLeftAnchor);
		
		_weaponTwo.Size = new Vector2(Size.y, Size.y);
		_weaponTwo.SetPosition(new Vector2(Size.x / 2.0f , 0), GameView.GameViewAnchor.MiddleRightAnchor);
		
		_weaponHighlighter.Size = new Vector2(Size.y + 8, Size.y + 8);
		Color highlighterColor = _weaponHighlighter.ImageGUITexture.color;
		if(State == GameView.GameViewState.Showing) highlighterColor.a = 0.25f + Mathf.Abs(Mathf.Cos(_animationTimer)) / 3.0f;
		_weaponHighlighter.ImageGUITexture.color = highlighterColor;
		if(_currentWeapon == 0) _weaponHighlighter.Position = _weaponOne.Position;
		else _weaponHighlighter.Position = _weaponTwo.Position;
	}
}
