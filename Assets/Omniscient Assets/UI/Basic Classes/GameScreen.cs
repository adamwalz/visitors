using UnityEngine;
using System.Collections;

public class GameScreen : MonoBehaviour 
{
	private ArrayList _views;
	private GameView _firstResponder;
	
	public void AddView(GameView view)
	{
		if(_views == null) _views = new ArrayList();
		_views.Add(view);
		view.Screen = this;
		UpdateZValues();
	}
	
	public void RemoveView(GameView view)
	{
		if(_views == null) _views = new ArrayList();
		_views.Remove(view);
		view.Screen = null;
		UpdateZValues();
	}
	
	private void UpdateZValues()
	{
		foreach(GameView view in _views)
		{
			view.ZValue = _views.IndexOf(view) * 1000;
			view.ZRange = 1000;
		}
	}
	
	public Vector2 Size
	{
		get
		{ 
			return new Vector2(Screen.width / UtilityPlugin.ContentScaleFactor(), Screen.height / UtilityPlugin.ContentScaleFactor());
		}
	}
	
	public void Update()
	{
		// Handle view position / animation updates
		foreach(GameView view in _views)
		{
			view.UpdateView();
		}
		
		// Get reverse view list so we can start with the "top-most" element
		// when searching for views that may respond to our touch
		ArrayList reverseViews = (ArrayList)_views.Clone();
		reverseViews.Reverse();
		
		// Handle touch input
		if(Input.GetMouseButtonDown(0))
		{
			_firstResponder = null;
			int count = 0;
			while(_firstResponder == null && count < reverseViews.Count)
			{
				_firstResponder = ((GameView)reverseViews[count]).FindFirstResponder(Input.mousePosition);
				count++;
			}
			if(_firstResponder != null) _firstResponder.TouchBegan(Input.mousePosition);
		}
		if(Input.GetMouseButtonUp(0))
		{
			if(_firstResponder != null) _firstResponder.TouchEnded(Input.mousePosition);	
		}
	}
}
