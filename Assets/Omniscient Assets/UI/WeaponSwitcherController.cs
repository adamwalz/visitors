// WeaponSwitcherController
// Made by Nathan Swenson

using UnityEngine;
using System.Collections;

public class WeaponSwitcherController : MonoBehaviour 
{
	private GameScreen _mainScreen;
	private ImageView _primaryBackground;
	private ImageView _secondaryBackground;
	private ButtonView _backButton;
	private ButtonView _chooseButton;
	private ButtonView _doneButton;
	private WeaponCarousel _carousel;
	private TextView _nameView;
	private TextView _descriptionView;
	
	// Use this for initialization
	void Start () 
	{
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		
		_primaryBackground = (ImageView)gameObject.AddComponent("ImageView");
		_primaryBackground.Init();
		_primaryBackground.TextureName = "SelectPrimaryWeapon";
		_primaryBackground.Size = _mainScreen.Size;
		_primaryBackground.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_mainScreen.AddView(_primaryBackground);
		
		_secondaryBackground = (ImageView)gameObject.AddComponent("ImageView");
		_secondaryBackground.Init();
		_secondaryBackground.TextureName = "SelectSecondaryWeapon";
		_secondaryBackground.Size = _mainScreen.Size;
		_secondaryBackground.SetPosition(new Vector2(0, 0), GameView.GameViewAnchor.BottomLeftAnchor);
		_mainScreen.AddView(_secondaryBackground);
		
		_backButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_backButton.Init();
		_backButton.ButtonImageName = "back";
		_backButton.HighlightImageName = "back";
		_backButton.Size = new Vector2(100, 40);
		_backButton.SetPosition(new Vector2(10, 10), GameView.GameViewAnchor.BottomLeftAnchor);
		_backButton.ButtonPressed += new EventHandler(BackButtonPressed);
		_mainScreen.AddView(_backButton);
		
		_chooseButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_chooseButton.Init();
		_chooseButton.ButtonImageName = "choose";
		_chooseButton.HighlightImageName = "choose";
		_chooseButton.Size = new Vector2(100, 40);
		_chooseButton.SetPosition(new Vector2(_mainScreen.Size.x - 10, 10), GameView.GameViewAnchor.BottomRightAnchor);
		_chooseButton.ButtonPressed += new EventHandler(ChooseButtonPressed);
		_mainScreen.AddView(_chooseButton);
		
		_doneButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_doneButton.Init();
		_doneButton.ButtonImageName = "done";
		_doneButton.HighlightImageName = "done";
		_doneButton.Size = new Vector2(100, 40);
		_doneButton.SetPosition(new Vector2(_mainScreen.Size.x - 10, 10), GameView.GameViewAnchor.BottomRightAnchor);
		_doneButton.ButtonPressed += new EventHandler(DoneButtonPressed);
		_mainScreen.AddView(_doneButton);
		
		_carousel = (WeaponCarousel)gameObject.AddComponent("WeaponCarousel");
		_carousel.Init();
		_carousel.Size = new Vector2(_mainScreen.Size.x, 200);
		_carousel.SetPosition(new Vector2(_mainScreen.Size.x / 2, _mainScreen.Size.y / 2), GameView.GameViewAnchor.MiddleAnchor);
		_carousel.WeaponSelected += new EventHandler(WeaponSelected);
		_carousel.Show(false);
		_mainScreen.AddView(_carousel);
		
		_nameView = (TextView)gameObject.AddComponent("TextView");
		_nameView.Init();
		_nameView.Position = new Vector2(_mainScreen.Size.x / 2, 80);
		_nameView.Text = "Hello";
		_nameView.Small = false;
		_nameView.Show(false);
		_mainScreen.AddView(_nameView);
		
		_descriptionView = (TextView)gameObject.AddComponent("TextView");
		_descriptionView.Init();
		_descriptionView.Position = new Vector2(_mainScreen.Size.x / 2, 60);
		_descriptionView.Text = "Hello";
		_descriptionView.Small = true;
		_descriptionView.Show(false);
		_mainScreen.AddView(_descriptionView);
		
		TransitionToPrimaryWeapon();
	}
	
	public void WeaponSelected(object sender)
	{
		string weaponID = Weapon.WeaponIDs()[_carousel.SelectedWeaponIndex];
		_nameView.Text = Weapon.GetWeaponById(weaponID).Name;
		_descriptionView.Text = Weapon.GetWeaponById(weaponID).Description;
		
		_chooseButton.Disabled = false;
		_doneButton.Disabled = false;
		
		if(Weapon.GetWeaponById(weaponID).isLocked()) 
		{
			_descriptionView.Text = Weapon.GetWeaponById(weaponID).UnlockRequirements;
			_chooseButton.Disabled = true;
			_doneButton.Disabled = true;
		}
	}
	
	public void BackButtonPressed(object sender)
	{
		TransitionToPrimaryWeapon();
	}
	
	public void ChooseButtonPressed(object sender)
	{
		string weaponID = Weapon.WeaponIDs()[_carousel.SelectedWeaponIndex];
		GameState.SavePrimaryWeapon(weaponID);
		TransitionToSecondaryWeapon();
	}
	
	public void DoneButtonPressed(object sender)
	{
		string weaponID = Weapon.WeaponIDs()[_carousel.SelectedWeaponIndex];
		GameState.SaveSecondaryWeapon(weaponID);
		Application.LoadLevel(GameState.GetCurrentLevel());	
	}
	
	public void TransitionToPrimaryWeapon()
	{
		_carousel.PrimaryWeaponIndex = -1;
		
		_doneButton.Hide(false);
		_secondaryBackground.Hide(false);
		_backButton.Hide(false);
		_chooseButton.Show(false);
		_primaryBackground.Show(false);
	}
	
	public void TransitionToSecondaryWeapon()
	{
		_carousel.PrimaryWeaponIndex = _carousel.SelectedWeaponIndex;
		if(_carousel.SelectedWeaponIndex == 0) _carousel.SelectWeapon(1);
		else _carousel.SelectWeapon(0);
		_carousel.CarouselWeaponListOffset = 0;
		
		_primaryBackground.Hide(false);
		_chooseButton.Hide(false);
		_secondaryBackground.Show(false);
		_backButton.Show(false);
		_doneButton.Show(false);
	}
}
