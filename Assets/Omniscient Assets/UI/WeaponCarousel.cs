using UnityEngine;
using System.Collections;

public class WeaponCarousel : GameView
{
	private WeaponCarouselButton _weaponOne;
	private WeaponCarouselButton _weaponTwo;
	private WeaponCarouselButton _weaponThree;
	private WeaponCarouselButton _transitionWeapon;
	private ButtonView _leftButton;
	private ButtonView _rightButton;
	private int _selectedWeaponIndex;
	private int _carouselWeaponListOffset;
	private int _primaryWeaponIndex;
	public event EventHandler WeaponSelected;
	
	// -1 means animating left, 0 means not animating, 1 means animating to the right
	private int _carouselAnimating;
	private float _carouselAnimationTimer;
	private float _carouselAnimationSpeed;
	
	public int SelectedWeaponIndex
	{
		get{return _selectedWeaponIndex;}
		set{SelectWeapon(value);}
	}
	
	public int PrimaryWeaponIndex
	{
		get{return _primaryWeaponIndex;}
		set{_primaryWeaponIndex = value;}
	}
	
	public int CarouselWeaponListOffset
	{
		get{return _carouselWeaponListOffset;}
		set{_carouselWeaponListOffset = value;}
	}
	
	public override void Init ()
	{
		base.Init ();
		
		_carouselWeaponListOffset = 0;
		_selectedWeaponIndex = 0;
		_primaryWeaponIndex = -1;
		_carouselAnimating = 0;
		_carouselAnimationTimer = 0.0f;
		_carouselAnimationSpeed = 300.0f;
		
		string[] weaponTextures = Weapon.WeaponIDs();
		
		_weaponOne = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponOne.Init();
		_weaponOne.WeaponID = weaponTextures[0];
		_weaponOne.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponOne);
		
		_weaponTwo = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponTwo.Init();
		_weaponTwo.WeaponID = weaponTextures[1];
		_weaponTwo.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponTwo);
		
		_weaponThree = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponThree.Init();
		_weaponThree.WeaponID = weaponTextures[2];
		_weaponThree.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponThree);
		
		_transitionWeapon = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_transitionWeapon.Init();
		_transitionWeapon.WeaponID = weaponTextures[3];
		AddSubview(_transitionWeapon);
		
		_leftButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_leftButton.Init();
		_leftButton.ButtonImageName = "CarouselLeft";
		_leftButton.HighlightImageName = "CarouselLeftHighlight";
		_leftButton.ButtonPressed += new EventHandler(LeftButtonPressed);
		AddSubview(_leftButton);
		
		_rightButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_rightButton.Init();
		_rightButton.ButtonImageName = "CarouselRight";
		_rightButton.HighlightImageName = "CarouselRightHighlight";
		_rightButton.ButtonPressed += new EventHandler(RightButtonPressed);
		AddSubview(_rightButton);
	}
	
	public void CarouselButtonPressed(object sender)
	{
		if(sender == _weaponOne) SelectWeapon(_carouselWeaponListOffset);
		if(sender == _weaponTwo) SelectWeapon(_carouselWeaponListOffset + 1);
		if(sender == _weaponThree) SelectWeapon(_carouselWeaponListOffset + 2);
	}
	
	public void LeftButtonPressed(object sender)
	{
		if(_carouselWeaponListOffset > 0) 
		{
			_carouselAnimating = -1;
		}
	}
	
	public void RightButtonPressed(object sender)
	{
		if(_carouselWeaponListOffset < Weapon.WeaponIDs().Length - 3)
		{
			_carouselAnimating = 1;
		}
	}
	
	public void SelectWeapon(int weaponIndex)
	{
		_selectedWeaponIndex = weaponIndex;
		if(WeaponSelected != null) WeaponSelected(this);
	}
	
	
	public void SetCarouselWeaponListOffset(int index)
	{
		_carouselWeaponListOffset = index;
	}
	
	public void RefreshCarouselStuff()
	{
		string[] weaponTextures = Weapon.WeaponIDs();
		
		Weapon WeaponOne = Weapon.GetWeaponById(Weapon.WeaponIDs()[_carouselWeaponListOffset]);
		Weapon WeaponTwo = Weapon.GetWeaponById(Weapon.WeaponIDs()[_carouselWeaponListOffset + 1]);
		Weapon WeaponThree = Weapon.GetWeaponById(Weapon.WeaponIDs()[_carouselWeaponListOffset + 2]);
		
		SelectWeapon(_selectedWeaponIndex);
		_weaponOne.Selected = false;
		_weaponTwo.Selected = false;
		_weaponThree.Selected = false;
		if(_selectedWeaponIndex == _carouselWeaponListOffset) _weaponOne.Selected = true;
		if(_selectedWeaponIndex == _carouselWeaponListOffset + 1) _weaponTwo.Selected = true;
		if(_selectedWeaponIndex == _carouselWeaponListOffset + 2) _weaponThree.Selected = true;
		
		_weaponOne.Disabled = false;
		_weaponTwo.Disabled = false;
		_weaponThree.Disabled = false;
		
		_weaponOne.Overlay.TextureName = "";
		_weaponTwo.Overlay.TextureName = "";
		_weaponThree.Overlay.TextureName = "";
		
		if(WeaponOne.isLocked()) _weaponOne.Overlay.TextureName = "lock";
		if(WeaponTwo.isLocked()) _weaponTwo.Overlay.TextureName = "lock";
		if(WeaponThree.isLocked()) _weaponThree.Overlay.TextureName = "lock";
		
		_weaponOne.WeaponID = weaponTextures[_carouselWeaponListOffset];
		_weaponTwo.WeaponID = weaponTextures[_carouselWeaponListOffset + 1];
		_weaponThree.WeaponID = weaponTextures[_carouselWeaponListOffset + 2];
		
		if(_carouselWeaponListOffset == _primaryWeaponIndex)
		{
			_weaponOne.Overlay.TextureName = "PrimaryWeaponOverlay";
			_weaponOne.Disabled = true;
		}
		if(_carouselWeaponListOffset + 1 == _primaryWeaponIndex)
		{
			_weaponTwo.Overlay.TextureName = "PrimaryWeaponOverlay";
			_weaponTwo.Disabled = true;
		}
		if(_carouselWeaponListOffset + 2 == _primaryWeaponIndex)
		{
			_weaponThree.Overlay.TextureName = "PrimaryWeaponOverlay";
			_weaponThree.Disabled = true;
		}
		
		_leftButton.Disabled = false;
		_rightButton.Disabled = false;
		
		if(_carouselWeaponListOffset == 0)
		{
			_leftButton.Disabled = true;	
		}
		if(_carouselWeaponListOffset + 3 == weaponTextures.Length)
		{
			_rightButton.Disabled = true;	
		}
		
		// Animation
		_transitionWeapon.Overlay.TextureName = "";
		_transitionWeapon.Selected = false;
		
		Weapon TransitionWeapon = null;
		if(_carouselAnimating == -1) TransitionWeapon = Weapon.GetWeaponById(Weapon.WeaponIDs()[_carouselWeaponListOffset - 1]);
		if(_carouselAnimating == 1) TransitionWeapon = Weapon.GetWeaponById(Weapon.WeaponIDs()[_carouselWeaponListOffset + 3]);
		
		if(_carouselAnimating == -1)
		{	
			// Set transition button content
			_transitionWeapon.WeaponID = TransitionWeapon.ID;
			if(TransitionWeapon.isLocked()) _transitionWeapon.Overlay.TextureName = "lock";
			if(_primaryWeaponIndex == _carouselWeaponListOffset - 1) _transitionWeapon.Overlay.TextureName = "PrimaryWeaponOverlay";
			if(_selectedWeaponIndex == _carouselWeaponListOffset - 1) _transitionWeapon.Selected = true;
			
			_weaponOne.Position += new Vector2(_carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_weaponTwo.Position += new Vector2(_carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_weaponThree.Position += new Vector2(_carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_transitionWeapon.Position = new Vector2(-240 + _carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			if(_weaponTwo.Position.x >= 120) 
			{
				_carouselAnimating = 0;
				SetCarouselWeaponListOffset(_carouselWeaponListOffset - 1);
			}
		}
		
		// Animation
		if(_carouselAnimating == 1)
		{
			// Set transition button content
			_transitionWeapon.WeaponID = TransitionWeapon.ID;
			if(TransitionWeapon.isLocked()) _transitionWeapon.Overlay.TextureName = "lock";
			if(_primaryWeaponIndex == _carouselWeaponListOffset + 3) _transitionWeapon.Overlay.TextureName = "PrimaryWeaponOverlay";
			if(_selectedWeaponIndex == _carouselWeaponListOffset + 3) _transitionWeapon.Selected = true;

			
			_weaponOne.Position += new Vector2(- _carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_weaponTwo.Position += new Vector2(- _carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_weaponThree.Position += new Vector2(- _carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			_transitionWeapon.Position = new Vector2(240 - _carouselAnimationTimer * _carouselAnimationSpeed, 0.0f);
			
			if(_weaponTwo.Position.x <= -120) 
			{
				_carouselAnimating = 0;
				SetCarouselWeaponListOffset(_carouselWeaponListOffset + 1);
			}
		}
	}
	
	public override void RefreshContent()
	{
		_leftButton.Size = new Vector2(45, 100);
		_leftButton.Position = new Vector2(-210, 0);
		
		_rightButton.Size = new Vector2(45, 100);
		_rightButton.Position = new Vector2(210, 0);
		
		_weaponOne.Size = new Vector2(100, 100);
		_weaponOne.Position = new Vector2(-120, 0);
		
		_weaponTwo.Size = new Vector2(100, 100);
		_weaponTwo.Position = new Vector2(0, 0);
		
		_weaponThree.Size = new Vector2(100, 100);
		_weaponThree.Position = new Vector2(120, 0);
		
		_transitionWeapon.Size = new Vector2(100, 100);
		_transitionWeapon.Position = new Vector2(9001, -9001);

		RefreshCarouselStuff();
		if(_carouselAnimating == 0)_carouselAnimationTimer = 0;
		else _carouselAnimationTimer += Time.deltaTime;
	}
}
