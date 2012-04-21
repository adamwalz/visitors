using UnityEngine;
using System.Collections;

public class ButtonView : GameView
{
	private ImageView _buttonImage;
	private string _buttonImageName;
	private string _highlightImageName;
	private bool _isHighlighted;
	public event EventHandler ButtonPressed;
	protected bool _disabled;
	private Color _color;
	
	// The filename of the texture to be used for this button, when it is not highlighted
	public string ButtonImageName
	{
		get { return _buttonImageName; }
		set { _buttonImageName = value; }
	}
	
	// I just added this so buttons can be fadey
	public Color ButtonColor
	{
		get {return _color;}
		set {_color = value;}
	}
	
	public bool Highlighted
	{
		get{return _isHighlighted;}
	}
	
	// Use this to disable a button
	public bool Disabled
	{
		get {return _disabled;}
		set {_disabled = value;}
	}
	
	// The filename of the texture to be used for this button, when it is not highlighted
	public string HighlightImageName
	{
		get { return _highlightImageName; }
		set { _highlightImageName = value; }
	}

	// Use this for initialization
	public override void Init ()
	{
		base.Init();
		
		_color.r = 0.5f;
		_color.g = 0.5f;
		_color.b = 0.5f;
		_color.a = 0.5f;
		
		_buttonImage = (ImageView)gameObject.AddComponent("ImageView");
		_buttonImage.Init();
		_buttonImage.TextureName = _buttonImageName;
		_isHighlighted = false;
		_disabled = false;
		AddSubview(_buttonImage);
	}
	
	// Position the GUITexture correctly based on our absolute position
	public override void RefreshContent()
	{
		_buttonImage.Position = new Vector2(0, 0);
		_buttonImage.Size = Size;
		if(_isHighlighted) _buttonImage.TextureName = _highlightImageName;
		else _buttonImage.TextureName = _buttonImageName;
		Color color = _buttonImage.ImageGUITexture.color;
		if(_disabled)
		{
			color.r = 0.3f;
			color.g = 0.3f;
			color.b = 0.3f;
			color.a = 0.2f;
		}
		else
		{
			color = _color;
		}
		_buttonImage.ImageGUITexture.color = color;
	}
	
	public override bool RespondsToTouchInput()
	{
		return !_disabled;
	}
	
	public override void TouchBegan (Vector2 touch)
	{
		_isHighlighted = true;
	}
	
	public override void TouchEnded (Vector2 touch)
	{
		if(HitTest(TouchPositionInView(touch)))
		{
			if(ButtonPressed != null) ButtonPressed(this);
		}
		_isHighlighted = false;
	}
}
