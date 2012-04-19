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
	
	public int SelectedWeaponIndex
	{
		get{return _selectedWeaponIndex;}
		set{SelectWeapon(value);}
	}
	
	public override void Init ()
	{
		base.Init ();
		_weaponOne = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponOne.Init();
		_weaponOne.WeaponID = "FireballWeapon";
		_weaponOne.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponOne);
		
		_weaponTwo = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponTwo.Init();
		_weaponTwo.WeaponID = "FireballWeapon";
		_weaponTwo.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponTwo);
		
		_weaponThree = (WeaponCarouselButton)gameObject.AddComponent("WeaponCarouselButton");
		_weaponThree.Init();
		_weaponThree.WeaponID = "FireballWeapon";
		_weaponThree.ButtonPressed += new EventHandler(CarouselButtonPressed);
		AddSubview(_weaponThree);
		
		_leftButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_leftButton.Init();
		_leftButton.ButtonImageName = "CarouselLeft";
		_leftButton.HighlightImageName = "CarouselLeft";
		AddSubview(_leftButton);
		
		_rightButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_rightButton.Init();
		_rightButton.ButtonImageName = "CarouselRight";
		_rightButton.HighlightImageName = "CarouselRight";
		AddSubview(_rightButton);
	}
	
	public void CarouselButtonPressed(object sender)
	{
		if(sender == _weaponOne) SelectWeapon(0);
		if(sender == _weaponTwo) SelectWeapon(1);
		if(sender == _weaponThree) SelectWeapon(2);
	}
	
	public void SelectWeapon(int weaponIndex)
	{
		_selectedWeaponIndex = weaponIndex;
		_weaponOne.Selected = false;
		_weaponTwo.Selected = false;
		_weaponThree.Selected = false;
		if(_selectedWeaponIndex == 0) _weaponOne.Selected = true;
		if(_selectedWeaponIndex == 1) _weaponTwo.Selected = true;
		if(_selectedWeaponIndex == 2) _weaponThree.Selected = true;
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
	}
}
