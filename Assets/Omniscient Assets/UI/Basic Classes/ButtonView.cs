using UnityEngine;
using System.Collections;

public delegate void EventHandler(object sender);

public class ButtonView : GameView
{
	private ImageView _buttonImage;
	private string _buttonImageName;
	private string _highlightImageName;
	private bool _isHighlighted;
	public event EventHandler ButtonPressed;
	
	// The filename of the texture to be used for this button, when it is not highlighted
	public string ButtonImageName
	{
		get { return _buttonImageName; }
		set { _buttonImageName = value; }
	}
	
	// The filename of the texture to be used for this button, when it is not highlighted
	public string HighlightImageName
	{
		get { return _highlightImageName; }
		set { _highlightImageName = value; }
	}

	// Use this for initialization
	public new void Init ()
	{
		base.Init();
		_buttonImage = (ImageView)gameObject.AddComponent("ImageView");
		_buttonImage.Init();
		_buttonImage.TextureName = _buttonImageName;
		_isHighlighted = false;
		AddSubview(_buttonImage);
	}
	
	// Position the GUITexture correctly based on our absolute position
	public override void RefreshContent()
	{
		_buttonImage.Position = new Vector2(0, 0);
		_buttonImage.Size = Size;
		if(_isHighlighted) _buttonImage.TextureName = _highlightImageName;
		else _buttonImage.TextureName = _buttonImageName;
	}
	
	public override bool RespondsToTouchInput()
	{
		return true;
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
