using UnityEngine;
using System.Collections;

public class WeaponCarousel : GameView
{
	private WeaponCarouselButton _weaponOne;
	private WeaponCarouselButton _weaponTwo;
	private WeaponCarouselButton _weaponThree;
	private ButtonView _leftButton;
	private ButtonView _rightButton;
	private int _selectedWeaponIndex;
	private int _carouselWeaponListOffset;
	private int _primaryWeaponIndex;
	
	public event EventHandler WeaponSelected;
	
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
		
		_leftButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_leftButton.Init();
		_leftButton.ButtonImageName = "CarouselLeft";
		_leftButton.HighlightImageName = "CarouselLeft";
		_leftButton.ButtonPressed += new EventHandler(LeftButtonPressed);
		AddSubview(_leftButton);
		
		_rightButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_rightButton.Init();
		_rightButton.ButtonImageName = "CarouselRight";
		_rightButton.HighlightImageName = "CarouselRight";
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
			SetCarouselWeaponListOffset(_carouselWeaponListOffset - 1);
		}
	}
	
	public void RightButtonPressed(object sender)
	{
		if(_carouselWeaponListOffset < Weapon.WeaponIDs().Length - 3)
		{
			SetCarouselWeaponListOffset(_carouselWeaponListOffset + 1);
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
	}
	
	public override void RefreshContent()
	{
		RefreshCarouselStuff();
		
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
	}
}
