// GameView
// By Nathan Swenson

using UnityEngine;
using System.Collections;

public class GameView : MonoBehaviour 
{
	private bool _hasFocus;
	private Vector2 _centerPosition;
	private Vector2 _size;
	protected ArrayList _subviews;
	
	// To accomodate proper view ordering based on Unity's GUITextures, each view
	// is assigned a range of z values: [_zValue, _zValue + _zRange) which it
	// splits up evenly among its subviews, and which lets views appear in the
	// proper order based on where they are in the view hierarchy.
	protected float _zValue;
	protected float _zRange;
	
	private GameView _parent;
	protected GameScreen _screen;
	private GameViewState _state;
	private GameViewState _lastState;
	protected float _animationTimer;
	protected float _animationDuration;
	
	// This bool should be set to "true" in your Init method if you wish to override
	// the custom Show behavior (calling Show(animated) on all subviews) and write your own
	// Show() method instead. You should still call base.Show(animated) in your Show() method.
	protected bool _overrideDefaultShowBehavior;
	
	// This bool should be set to "true" in your Init method if you wish to override
	// the custom Hide behavior (calling Hide(animated) on all subviews) and write your own
	// Hide() method instead. You should still call base.Hide(animated) in your Hide() method.
	protected bool _overrideDefaultHideBehavior;
	
	public enum GameViewAnchor { TopLeftAnchor, TopMiddleAnchor, TopRightAnchor,
						MiddleLeftAnchor, MiddleAnchor, MiddleRightAnchor,
						BottomLeftAnchor, BottomMiddleAnchor, BottomRightAnchor};
	
	public enum GameViewState { Hidden, Showing, AnimatingIn, AnimatingOut }
	
	public virtual void Init()
	{
		_subviews = new ArrayList();	
		_state = GameViewState.Hidden;
		_lastState = GameViewState.Hidden;
		_overrideDefaultShowBehavior = false;
		_overrideDefaultHideBehavior = false;
		_zValue = 0;
		_zRange = 0;
		_hasFocus = true;
		
		// Note: change this later!
		_animationDuration = 0.5f;
	}
	
	// Call this method to show your view as well as all of its subviews on the screen. 
	// All textures and other assets needed to show the view are instatiated here.
	// If you pass in "true" and the view has an animate-in effect, it will be shown.
	
	// Note: All views should override this method and call this base implementation, followed
	// by their Show behavior. If no custom behavior is needed, Show(animated) should be called
	// on all subviews, with a call like this: foreach(GameView view in _subviews) view.Show(animated);
	// This is not done automatically in the base class, in case custom behavior is desired.
	public virtual void Show(bool animated)
	{
		if(animated) _state = GameViewState.AnimatingIn;
		else _state = GameViewState.Showing;
		if(!_overrideDefaultShowBehavior) foreach(GameView view in _subviews) view.Show(animated);
	}
	
	// Call this method to hide your view, as well as any of its subviews from the screen.
	// All textures and other assets needed to show the view are destroyed here.
	// If you pass in "true" and the view has an animate-out effect, it will be shown.
	public virtual void Hide(bool animated)
	{
		if(animated) _state = GameViewState.AnimatingOut;
		else
		{
			_state = GameViewState.Hidden;
			OnHidden();
		}
		if(!_overrideDefaultHideBehavior) foreach(GameView view in _subviews) view.Hide(animated);
	}
	
	// This method is called when the hiding animation (if there was one) is complete,
	// and it is safe to destroy things that are no longer being drawn to the screen.
	protected virtual void OnHidden()
	{
		
	}
	
	public virtual bool HasFocus
	{
		get{return _hasFocus;}
		set
		{
			_hasFocus = value;
			foreach(GameView view in _subviews)
			{
				view.HasFocus = value;	
			}
		}
	}
	
	// This view's parent view
	public GameView Parent
	{
		get{return _parent;}
	}
	
	// This view's GameScreen
	public GameScreen Screen
	{
		get{return _screen;}
		set{_screen = value;}
	}
	
	// This view's z value
	public float ZValue
	{
		get{return _zValue;}
		set
		{
			_zValue = value;
			UpdateZValues();
		}
	}
	
	// This view's z range
	public float ZRange
	{
		get{return _zRange;}
		set
		{
			_zRange = value;
			UpdateZValues();
		}
	}
	
	public GameViewState State
	{
		get{return _state;}
		set{_state = value;}
	}
	
	public float AnimationDuration
	{
		get{return _animationDuration;}	
	}
	
	// Getter and setter for the view's position, which is measured in points
	// and is in the coordinate system of its parent view. If the parent view
	// is a GameScreen, the coordinate system is the device's full screen in points.
	public Vector2 GetPosition(GameViewAnchor anchor)
	{
		return _centerPosition + AnchorOffset(anchor);
	}
	
	public void SetPosition(Vector2 position, GameViewAnchor anchor)
	{
		_centerPosition = position - AnchorOffset(anchor);
	}
	
	// Convenient Position property, which gets/sets the position of the "middle" of our view.
	public Vector2 Position
	{
		get{return _centerPosition;}
		set{_centerPosition = value;}
	}
	
	// Computes the absolute position of the view on the screen, measured in points.
	public Vector2 GetAbsolutePosition(GameViewAnchor anchor)
	{
		GameView parent = _parent;
		Vector2 offset = _centerPosition;
		
		// walk up the view hierarchy, adding the positions of our ancestors
		// to our "global" offset until we arrive at the root.
		while(parent != null)
		{
			offset += parent.GetPosition(GameViewAnchor.MiddleAnchor);
			parent = parent.Parent;
		}
		
		return offset + AnchorOffset(anchor);
	}
	
	// Based on our size, compute the offset (in points) to the given
	// anchor point, measured from the center of our view.
	public Vector2 AnchorOffset(GameViewAnchor anchor)
	{
		float horizOffset = _size.x / 2.0f;
		float vertOffset = _size.y / 2.0f;
		
		switch (anchor)
		{
		case GameViewAnchor.TopLeftAnchor:
			return new Vector2(- horizOffset, vertOffset);
		case GameViewAnchor.TopMiddleAnchor:
			return new Vector2(0.0f, vertOffset);
		case GameViewAnchor.TopRightAnchor:
			return new Vector2(horizOffset, vertOffset);
		case GameViewAnchor.MiddleLeftAnchor:
			return new Vector2(- horizOffset, 0.0f);
		case GameViewAnchor.MiddleAnchor:
			return new Vector2(0.0f, 0.0f);
		case GameViewAnchor.MiddleRightAnchor:
			return new Vector2(horizOffset, 0.0f);
		case GameViewAnchor.BottomLeftAnchor:
			return new Vector2(- horizOffset, - vertOffset);
		case GameViewAnchor.BottomMiddleAnchor:
			return new Vector2(0.0f, - vertOffset);
		case GameViewAnchor.BottomRightAnchor:
			return new Vector2(horizOffset, - vertOffset);
		default:
			return new Vector2(0.0f, 0.0f);
		}
	}
	
	// Getters and setters for size
	public Vector2 Size
	{
		get{return _size;}
		set{_size = value;}
	}
	

	// This method is called on each GameView in the view hierarchy every update cycle
	// to update the various aspects of the view. You should not call this method yourself
	// and should not override it unless you also call this base class's implementation
	public virtual void UpdateView()
	{
		if(_state != _lastState) _animationTimer = 0.0f;
		
		// Check to see if it is time to change animation states
		if(_state == GameViewState.AnimatingIn)
		{
			if(_animationTimer > _animationDuration)_state = GameViewState.Showing;
		}
		if(_state == GameViewState.AnimatingOut)
		{
			if(_animationTimer > _animationDuration)
			{
				_state = GameViewState.Hidden;
				OnHidden();
			}
		}
		
		if(_state != _lastState) _animationTimer = 0.0f;
		
		if(_state == GameViewState.AnimatingIn || _state == GameViewState.AnimatingOut || _state == GameViewState.Showing)
		{
			RefreshContent();
			if(_state == _lastState) _animationTimer += Time.deltaTime;
		}
		_lastState = _state;
		
		foreach(GameView subview in _subviews)
		{
			subview.UpdateView();
		}
	}
	
	// This method is called every frame, and should be used to update this view's content
	// based on our current position, size, and animation state.
	public virtual void RefreshContent()
	{
		
	}
	
	private void UpdateZValues()
	{
		foreach(GameView view in _subviews)
		{
			float subviewRange = _zRange * 1.0f / _subviews.Count;
			view.ZValue = _zValue + subviewRange * _subviews.IndexOf(view);
			view.ZRange = subviewRange;
		}
	}
	
	public void AddSubview(GameView subview)
	{
		_subviews.Add(subview);
		subview._parent = this;
		subview.Screen = _screen;
		subview.HasFocus = this.HasFocus;
		UpdateZValues();
	}
	
	public void RemoveSubview(GameView subview)
	{
		if(_subviews.Contains(subview))
		{
			_subviews.Remove(subview);
			subview._parent = null;
			subview.Screen = null;
		}
		UpdateZValues();
	}
	
	// This method should return true only if the view is a button
	// or something that intercepts touches and does something with
	// them. If a view receives a touch and this method returns true,
	// it will receive TouchBegan, TouchMoved, TouchEnded, and
	// TouchCancelled events.
	public virtual bool RespondsToTouchInput()
	{
		return false;
	}
	
	// Finds the top-most view that can do something with the touch.
	// A view can receive the touch only if these four conditions are met:
	// 1. The view is showing
	// 2. The view has focus
	// 3. The view responds to touch input (see RespondsToTouchInput)
	// 4. The touch intersects the view's rectangle
	// If neither this view nor any of its subviews is able to receive the touch,
	// null is returned.
	public GameView FindFirstResponder(Vector2 touchPixelLocation)
	{
		// If this view doesn't meet one of the first two conditions, neither can our children.
		// Return null in this case.
		if(_state != GameViewState.Showing || HasFocus == false) return null;
		
		// See if the touch even intersects our view rectangle
		if(!HitTest(TouchPositionInView(touchPixelLocation))) return null;
		
		// At this point, we can see that the touch is within our rectangle and we are in a position
		// to do something with it. Let's first see if any of our subviews are able to receive it.
		ArrayList reverseViewList = (ArrayList)_subviews.Clone();
		reverseViewList.Reverse();
		foreach(GameView view in reverseViewList)
		{
			// If one of our child views is/has a valid first responder, return that because
			// it is the top-most view.
			GameView childResponder = view.FindFirstResponder(touchPixelLocation);
			if(childResponder != null) return childResponder;
		}
		
		// If none of our children want the touch input but we do, return ourself. Otherwise return null.
		if(RespondsToTouchInput()) return this;
		return null;
	}
	
	public virtual void TouchBegan(Vector2 touch)
	{
	}
	
	public virtual void TouchEnded(Vector2 touch)
	{	
	}
	
	// Converts the touch position from the global pixel coordinate system
	// to this view's local "points" coordinate system, which is measured
	// from (0, 0) in the top-left corner to (Size.x, Size.y) in the bottom-right.
	public Vector2 TouchPositionInView(Vector2 touchPosition)
	{
		int contentScaleFactor = UtilityPlugin.ContentScaleFactor();
		Vector2 globalTouchPosition = new Vector2(touchPosition.x / contentScaleFactor, touchPosition.y / contentScaleFactor);
		Vector2 globalViewPosition = GetAbsolutePosition(GameViewAnchor.TopLeftAnchor);
		return new Vector2(globalTouchPosition.x - globalViewPosition.x, -globalTouchPosition.y + globalViewPosition.y);
	}
	
	// Simple method that takes a point within this view's coordinate system
	// ((0, 0) in the top-left corner to (Size.x, Size.y) in the bottom-right)
	// and determines whether it intersects this view. Goes hand-in-hand with
	// the TouchPositionInView method.
	public bool HitTest(Vector2 position)
	{
		if(position.x >= 0 && position.y >= 0 && position.x <= Size.x && position.y <= Size.y) return true;
		return false;
	}
}
