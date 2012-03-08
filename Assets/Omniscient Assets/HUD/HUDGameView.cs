using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IHUDGameViewController
{
	void HUDGameViewWeaponsSwitched(int newWeapon);
	void HUDGameViewFireButtonPressed();
	void HUDGameViewPauseButtonPressed();
	void HUDGameViewMenuButtonPressed();
}

public class HUDGameView : MonoBehaviour, IHUDView
{
	private GUITexture _printButtonGUITexture;
	private GUITexture _fireButtonGUITexture;
	private GUITexture _pauseButtonGUITexture;
	private GUITexture _menuButtonGUITexture;
	private GUITexture _activeButton;
	private GUITexture _crosshairGUITexture;
	private GUITexture _energyBarGUITexture;
	private GUITexture _energyGUITexture;
	private GUITexture _highlighterGUITexture;
	private List<GUITexture> _weapons;
	
	private IHUDGameViewController _controller;
	private bool _isShowing = false;
	private float _energy = 100.0f;
	private int _currentWeapon = 0;
	private bool _userInteractionEnabled = true;
	private IHUDViewAnimationCompletionHandler _completionHandler;
	
	public void Show(bool animated)
	{
		_isShowing = true;
		_weapons = new List<GUITexture>();
		_currentWeapon = 0;
		
		// Setup "print" button
		_printButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_printButtonGUITexture.transform.position = new Vector3(0, 0, 0);
		_printButtonGUITexture.pixelInset = new Rect(0, -50, 256, 256);
		Texture tex = (Texture2D)Resources.Load("switchWeaponButton", typeof(Texture2D));
		_printButtonGUITexture.texture = tex;
		
		// Setup "fire" button
		_fireButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_fireButtonGUITexture.transform.position = new Vector3(1, 0, 0);
		_fireButtonGUITexture.pixelInset = new Rect(-256, -50, 256, 256);
		tex = (Texture2D)Resources.Load("fireButton", typeof(Texture2D));
		_fireButtonGUITexture.texture = tex;
		
		// Setup "pause" button
		_pauseButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_pauseButtonGUITexture.transform.position = new Vector3(1, 1, 0);
		_pauseButtonGUITexture.pixelInset = new Rect(-128, -120, 128, 128);
		tex = (Texture2D)Resources.Load("pauseButton", typeof(Texture2D));
		_pauseButtonGUITexture.texture = tex;
		
		// Setup "crosshair"
		_crosshairGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_crosshairGUITexture.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
		_crosshairGUITexture.pixelInset = new Rect(-64, -64, 128, 128);
		tex = (Texture2D)Resources.Load("crosshair", typeof(Texture2D));
		_crosshairGUITexture.texture = tex;
		
		// Setup "energy bar"
		_energyBarGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_energyBarGUITexture.transform.position = new Vector3(0.5f, 0.0f, 0.0f);
		_energyBarGUITexture.pixelInset = new Rect(-256, 0, 512, 512);
		tex = (Texture2D)Resources.Load("energyBar", typeof(Texture2D));
		_energyBarGUITexture.texture = tex;
		
		// Setup energy
		_energyGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_energyGUITexture.transform.position = new Vector3(0.5f, 0.0f, 0.0f);
		_energyGUITexture.pixelInset = new Rect(-256, 0, 320, 64);
		tex = (Texture2D)Resources.Load("energy", typeof(Texture2D));
		_energyGUITexture.texture = tex;
		
		// Setup weapon highlighter
		_highlighterGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_highlighterGUITexture.transform.position = new Vector3(0.05f, 0.4f, 0.0f);
		_highlighterGUITexture.pixelInset = new Rect(-35, -35, 70, 70);
		tex = (Texture2D)Resources.Load("weaponHighlighter", typeof(Texture2D));
		_highlighterGUITexture.texture = tex;
		
		// Setup "menu" button
		_menuButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_menuButtonGUITexture.transform.position = new Vector3(1, 1, 0);
		_menuButtonGUITexture.pixelInset = new Rect(-256, -120, 128, 128);
		tex = (Texture2D)Resources.Load("menuButton", typeof(Texture2D));
		_menuButtonGUITexture.texture = tex;
	}
	
	public void Show(List<Texture2D> weaponTextures, bool animated)
	{
		// Call default Show() first
		Show(animated);
		
		// Then make GUITextures from the weapon textures
		if(weaponTextures == null || weaponTextures.Count == 0)
		{
			print("HUDGameView: Y u no initialize me with any weapons?");
			return;
		}
		foreach(Texture2D texture in weaponTextures)
		{
			GUITexture weapon = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
			weapon.transform.position = new Vector3(0.05f + 0.08f * weaponTextures.IndexOf(texture), 0.4f, 0.0f);
			weapon.pixelInset = new Rect(-32, -32, 64, 64);
			weapon.texture = texture;
			_weapons.Add(weapon);
		}
	}
	
	// Hides the view from the user, optionally by animating them out. When the view is done hiding, the 
	// resources associated with it are destroyed to free up memory.
	public void Hide(bool animated)
	{
		_isShowing = false;
		Destroy(_printButtonGUITexture);
		Destroy(_fireButtonGUITexture);
		Destroy(_pauseButtonGUITexture);
		Destroy(_crosshairGUITexture);
		Destroy(_energyBarGUITexture);
		Destroy(_menuButtonGUITexture);
	}
	
	// Set whether or not callbacks will occur for button presses, and whether buttons will be highlighted
	// when pressed. Make sure to do the right thing if you disable interaction while a button is pressed.
	public void SetUserInteractionEnabled(bool enabled)
	{
		_userInteractionEnabled = enabled;	
	}
	public bool IsUserInteractionEnabled()
	{
		return _userInteractionEnabled;
	}
	
	// Sets this view's animation completion handler (see IHUDViewAnimationCompletionHandler)
	public IHUDViewAnimationCompletionHandler GetAnimationCompletionHandler()
	{
		return _completionHandler;
	}
	
	// Get's this view's animation completion handler
	public void SetAnimationCompletionHandler(IHUDViewAnimationCompletionHandler handler)
	{
		_completionHandler = handler;
	}
	
	// Recalculates positions and states of all the GUITextures each frame. It also sends off events
	// such as animation completions and button presses. 
	public void Update ()
	{
		// Don't do anything unless we're showing
		if(_isShowing == false) return;
		
		// Update the energy bar with the current value of _energy
		_energyGUITexture.pixelInset = new Rect(_energyBarGUITexture.pixelInset.x + 64.0f, _energyBarGUITexture.pixelInset.y + 62.0f, 3.9f * Energy, 64);
		
		HandleInput();
		_highlighterGUITexture.transform.position = new Vector3(0.05f + 0.08f * _currentWeapon, 0.4f, 0.0f);
		
	}
	
	private void HandleInput()
	{
		// If mouse is down, highlight any button that it hits and set that as the "active" button
		if(Input.GetMouseButtonDown(0))
		{
			if(_printButtonGUITexture.HitTest(Input.mousePosition))
			{
				_printButtonGUITexture.texture = (Texture2D)Resources.Load("switchWeaponButtonPressed");
				_activeButton = _printButtonGUITexture;
			}
			if(_fireButtonGUITexture.HitTest(Input.mousePosition))
			{
				_fireButtonGUITexture.texture = (Texture2D)Resources.Load("fireButtonPressed");
				_activeButton = _fireButtonGUITexture;
			}
			if(_pauseButtonGUITexture.HitTest(Input.mousePosition))
			{
				_pauseButtonGUITexture.texture = (Texture2D)Resources.Load("pauseButtonPressed");
				_activeButton = _pauseButtonGUITexture;
			}
			if(_menuButtonGUITexture.HitTest(Input.mousePosition))
			{
				_menuButtonGUITexture.texture = (Texture2D)Resources.Load("menuButtonPressed");
				_activeButton = _menuButtonGUITexture;
			}
		}
		
		// If mouse is released, set all buttons to their non-highlighted state. If the active button
		// (the one that the user initially pressed) has been released, call the appropriate pressed
		// method on our controller.
		if(Input.GetMouseButtonUp(0))
		{
			_printButtonGUITexture.texture = (Texture2D)Resources.Load("switchWeaponButton");
			_fireButtonGUITexture.texture = (Texture2D)Resources.Load("fireButton");
			_pauseButtonGUITexture.texture = (Texture2D)Resources.Load("pauseButton");
			_menuButtonGUITexture.texture = (Texture2D)Resources.Load("menuButton");
			
			if(_printButtonGUITexture.HitTest(Input.mousePosition))
			{
				 if(_activeButton == _printButtonGUITexture)
				{
					_currentWeapon++;
					if(_currentWeapon >= _weapons.Count) _currentWeapon = 0;
					if(_controller != null)_controller.HUDGameViewWeaponsSwitched(_currentWeapon);
				}
			}
			if(_fireButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _fireButtonGUITexture)_controller.HUDGameViewFireButtonPressed();
			}
			if(_pauseButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _pauseButtonGUITexture)_controller.HUDGameViewPauseButtonPressed();
			}
			if(_menuButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _menuButtonGUITexture)_controller.HUDGameViewMenuButtonPressed();
			}
		}	
	}
	
	public void SetController(IHUDGameViewController controller)
	{
		_controller = controller;
	}
	
	// Energy bar goes from 0 to 100
	public float Energy
	{
		get	{return _energy;}
		set 
		{
			_energy = value; 
			if(_energy > 100.0f) _energy = 100.0f; 
			if(_energy < 0.0f) _energy = 0.0f;
		}
	}
}
