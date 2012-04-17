using UnityEngine;
using System.Collections;

public interface IHUDSearchingViewController
{
	void HUDSearchingViewPrintButtonPressed();
	void HUDSearchingViewPlayWithoutButtonPressed();
	void HUDSearchingViewPauseButtonPressed();
	void HUDSearchingViewMenuButtonPressed();
}

public class HUDSearchingView : MonoBehaviour, IHUDView
{
	private GUITexture _printButtonGUITexture;
	private GUITexture _playWithoutButtonGUITexture;
	private GUITexture _pauseButtonGUITexture;
	private GUITexture _menuButtonGUITexture;
	private GUITexture _activeButton;
	private GUITexture _crosshairGUITexture;
	private IHUDSearchingViewController _controller;
	private bool _isShowing = false;
	private bool _userInteractionEnabled = false;
	private IHUDViewAnimationCompletionHandler _completionHandler;
	
	// Initializes the GUITextures composing the view and shows them, optionally by animating them in.
	// If you need to initialize your view with certain data, you can either write a separate setter
	// for that data, and/or write your own Show() method. If you write your own Show() method, make sure
	// to have reasonable default behavior for when the default Show() is called.
	public void Show(bool animated)
	{
		_isShowing = true;
		
		// Setup "print" button
		_printButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_printButtonGUITexture.transform.position = new Vector3(0, 0, 0);
		_printButtonGUITexture.pixelInset = new Rect(0, -50, 256, 256);
		Texture tex = (Texture2D)Resources.Load("printButton", typeof(Texture2D));
		_printButtonGUITexture.texture = tex;
		
		// Setup "play without" button
		_playWithoutButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_playWithoutButtonGUITexture.transform.position = new Vector3(1, 0, 0);
		_playWithoutButtonGUITexture.pixelInset = new Rect(-256, -50, 256, 256);
		tex = (Texture2D)Resources.Load("playWithoutButton", typeof(Texture2D));
		_playWithoutButtonGUITexture.texture = tex;
		
		// Setup "pause" button
		_pauseButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_pauseButtonGUITexture.transform.position = new Vector3(1, 1, 0);
		_pauseButtonGUITexture.pixelInset = new Rect(-128, -120, 128, 128);
		tex = (Texture2D)Resources.Load("pauseButton", typeof(Texture2D));
		_pauseButtonGUITexture.texture = tex;
		
		// Setup "crosshair"
		_crosshairGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_crosshairGUITexture.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
		_crosshairGUITexture.pixelInset = new Rect(-128, -128, 256, 256);
		tex = (Texture2D)Resources.Load("crosshairSearching", typeof(Texture2D));
		_crosshairGUITexture.texture = tex;
		
		// Setup "menu" button
		_menuButtonGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		_menuButtonGUITexture.transform.position = new Vector3(1, 1, 0);
		_menuButtonGUITexture.pixelInset = new Rect(-256, -120, 128, 128);
		tex = (Texture2D)Resources.Load("menuButton", typeof(Texture2D));
		_menuButtonGUITexture.texture = tex;
	}
	
	// Hides the view from the user, optionally by animating them out. When the view is done hiding, the 
	// resources associated with it are destroyed to free up memory.
	public void Hide(bool animated)
	{
		_isShowing = false;
		Destroy(_printButtonGUITexture);
		Destroy(_playWithoutButtonGUITexture);
		Destroy(_pauseButtonGUITexture);
		Destroy(_crosshairGUITexture);
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
	public void Update()
	{
		// Don't do anything unless we're showing
		if(_isShowing == false) return;
		
		// If mouse is down, highlight any button that it hits and set that as the "active" button
		if(Input.GetMouseButtonDown(0))
		{
			if(_printButtonGUITexture.HitTest(Input.mousePosition))
			{
				_printButtonGUITexture.texture = (Texture2D)Resources.Load("printButtonPressed");
				_activeButton = _printButtonGUITexture;
			}
			if(_playWithoutButtonGUITexture.HitTest(Input.mousePosition))
			{
				_playWithoutButtonGUITexture.texture = (Texture2D)Resources.Load("playWithoutButtonPressed");
				_activeButton = _playWithoutButtonGUITexture;
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
			_printButtonGUITexture.texture = (Texture2D)Resources.Load("printButton");
			_playWithoutButtonGUITexture.texture = (Texture2D)Resources.Load("playWithoutButton");
			_pauseButtonGUITexture.texture = (Texture2D)Resources.Load("pauseButton");
			_menuButtonGUITexture.texture = (Texture2D)Resources.Load("menuButton");
			
			if(_printButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _printButtonGUITexture)_controller.HUDSearchingViewPrintButtonPressed();
			}
			if(_playWithoutButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _playWithoutButtonGUITexture)_controller.HUDSearchingViewPlayWithoutButtonPressed();
			}
			if(_pauseButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _pauseButtonGUITexture)_controller.HUDSearchingViewPauseButtonPressed();
			}
			if(_menuButtonGUITexture.HitTest(Input.mousePosition))
			{
				if(_controller != null && _activeButton == _menuButtonGUITexture)_controller.HUDSearchingViewMenuButtonPressed();
			}
		}
	}
	
	public void setController(IHUDSearchingViewController controller)
	{
		_controller = controller;
	}
	
}
