// TextView
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

public class TextView : GameView
{
	private string _text;
	private GUIText _GUIText;
	
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

	// Use this for initialization
	public new void Init () 
	{
		base.Init();
		_text = "Placeholder";
	}
	
	public override void Show(bool animated)
	{
		base.Show(animated);
		_GUIText = (GUIText)gameObject.AddComponent("GUIText");
		_GUIText.anchor = TextAnchor.LowerLeft;
		_GUIText.fontSize = 50;
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
