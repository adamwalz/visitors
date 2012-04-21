// TextView
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

public class TextView : GameView
{
	private string _text;
	private GUIText _GUIText;
	private bool _small;
	
	public GUIText InternalGUIText
	{
		get{return _GUIText;}
	}
	
	// The text for the GUIText
	public string Text
	{
		get { return _text; }
		set { 
				_text = value;
				if(State != GameView.GameViewState.Hidden)
				{
					_GUIText.text = _text;
				}
			}
	}
	
	public bool Small
	{
		get { return _small;}
		set { _small = value;}
	}
	
	// Use this for initialization
	public new void Init () 
	{
		base.Init();
		_small = false;
		_text = "Placeholder";
	}
	
	public override void Show(bool animated)
	{
		base.Show(animated);
		
		if(_GUIText == null && _small == false) _GUIText = (GUIText)((GameObject)Instantiate(Resources.Load("GUITextForTextView"))).GetComponent(typeof(GUIText));
		else if(_GUIText == null && _small == true) _GUIText = (GUIText)((GameObject)Instantiate(Resources.Load("GUITextForTextViewSmall"))).GetComponent(typeof(GUIText));
		
		_GUIText.anchor = TextAnchor.MiddleCenter;
		_GUIText.text = _text;
	}
	
	protected override void OnHidden()
	{
		Destroy(_GUIText);
	}
	
	// Position the GUITexture correctly based on our absolute position
	public override void RefreshContent()
	{
		Vector2 absPosition = GetAbsolutePosition(GameViewAnchor.BottomLeftAnchor);
		float contentScaleFactor = (float)UtilityPlugin.ContentScaleFactor();
		_GUIText.pixelOffset = new Vector2(absPosition.x * contentScaleFactor, absPosition.y * contentScaleFactor);
		_GUIText.transform.position = new Vector3(0, 0, _zValue);
	}
}
