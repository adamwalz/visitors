// HUDView
// By Nathan Swenson

using UnityEngine;
using System.Collections;

// The interface an object must implement to be notified when a HUDView is done animating in or out
public interface IHUDViewAnimationCompletionHandler
{
	// This is called after Show(true) is called to let you know when the view has been fully
	// animated onto the screen.
	void HUDViewAnimatedIn();
	// This lets you know when the view has been animated out and its resources have been
	// freed from memory.
	void HUDViewAnimatedOut();
}

// Any object wishing to be treated as a view for our game's HUD should implement this interface
public interface IHUDView
{
	// Initializes the GUITextures composing the view and shows them, optionally by animating them in.
	// If you need to initialize your view with certain data, you can either write a separate setter
	// for that data, and/or write your own Show() method. If you write your own Show() method, make sure
	// to have reasonable default behavior for when the default Show() is called.
	void Show(bool animated);
	
	// Hides the view from the user, optionally by animating them out. When the view is done hiding, the 
	// resources associated with it are destroyed to free up memory.
	void Hide(bool animated);
	
	// Set whether or not callbacks will occur for button presses, and whether buttons will be highlighted
	// when pressed. Make sure to do the right thing if you disable interaction while a button is pressed.
	void SetUserInteractionEnabled(bool enabled);
	bool IsUserInteractionEnabled();
	
	// Sets this view's animation completion handler (see IHUDViewAnimationCompletionHandler)
	IHUDViewAnimationCompletionHandler GetAnimationCompletionHandler();
	
	// Get's this view's animation completion handler
	void SetAnimationCompletionHandler(IHUDViewAnimationCompletionHandler handler);
	
	// Recalculates positions and states of all the GUITextures each frame. It also sends off events
	// such as animation completions and button presses. 
	void Update();
}