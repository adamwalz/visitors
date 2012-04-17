using UnityEngine;
using System.Collections;

public class WeaponSwitcher : GameView
{

	private ImageView _weaponOne;
	private ImageView _weaponTwo;
	private ImageView _weaponHighlighter;
	private int _currentWeapon;
	
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
		
		_weaponOne = (ImageView)gameObject.AddComponent("ImageView");
		_weaponOne.Init();
		_weaponOne.TextureName = "fireballWeapon";
		AddSubview(_weaponOne);
		
		_weaponTwo = (ImageView)gameObject.AddComponent("ImageView");
		_weaponTwo.Init();
		_weaponTwo.TextureName = "blueWeaponOne";
		AddSubview(_weaponTwo);
		
	}
	
	public override void RefreshContent()
	{
		_weaponOne.Size = new Vector2(45, 45);
		_weaponOne.Position = new Vector2(0, 0);
		
		_weaponTwo.Size = new Vector2(45, 45);
		_weaponTwo.Position = new Vector2(50, 0);
		
		_weaponHighlighter.Size = new Vector2(50, 50);
		Color highlighterColor = _weaponHighlighter.ImageGUITexture.color;
		if(State == GameView.GameViewState.Showing) highlighterColor.a = 0.5f + Mathf.Abs(Mathf.Cos(_animationTimer)) / 4.0f;
		_weaponHighlighter.ImageGUITexture.color = highlighterColor;
		if(_currentWeapon == 0) _weaponHighlighter.Position = _weaponOne.Position;
		else _weaponHighlighter.Position = _weaponTwo.Position;
	}
}
