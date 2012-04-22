using UnityEngine;
using System.Collections;

public class NumberSelectorView : GameView 
{
	private ButtonView _topArrowButton;
	private ButtonView _bottomArrowButton;
	private TextView _numberView;
	private int _number;
	private int _maxNumber;
	private int _minNumber;
	
	public int Number
	{
		get{return _number;}
		set{_number = value;}
	}
	
	public int MinimumNumber
	{
		get{return _minNumber;}
		set
		{
			_minNumber = value;
			if(_number < _minNumber) 
				_number = _minNumber;
		}
	}
	
	public int MaximumNumber
	{
		get{return _maxNumber;}
		set
		{
			_maxNumber = value;
			if(_number > _maxNumber) 
				_number = _maxNumber;
		}
	}
	
	public override void Init()
	{
		base.Init();
		_minNumber = 0;
		_maxNumber = 10;
		_number = 0;
		
		_topArrowButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_topArrowButton.Init();
		_topArrowButton.ButtonImageName = "numberSelectorUp";
		_topArrowButton.HighlightImageName = "numberSelectorUpHighlight";
		_topArrowButton.ButtonPressed += new EventHandler(UpPressed);
		AddSubview(_topArrowButton);
		
		_bottomArrowButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_bottomArrowButton.Init();
		_bottomArrowButton.ButtonImageName = "numberSelectorDown";
		_bottomArrowButton.HighlightImageName = "numberSelectorDownHighlight";
		_bottomArrowButton.ButtonPressed += new EventHandler(DownPressed);
		AddSubview(_bottomArrowButton);
		
		_numberView = (TextView)gameObject.AddComponent("TextView");
		_numberView.Init();
		AddSubview(_numberView);
	}
	
	public void UpPressed(object sender)
	{
		_number++;
		if(_number > _maxNumber) 
			_number = _maxNumber;
	}
	
	public void DownPressed(object sender)
	{
		_number--;	
		if(_number < _minNumber) 
			_number = _minNumber;
	}
	
	public override void RefreshContent()
	{
		_topArrowButton.Size = new Vector2(Size.x, 40);
		Vector2 topArrowButtonPosition = AnchorOffset(GameView.GameViewAnchor.TopLeftAnchor);
		_topArrowButton.SetPosition(topArrowButtonPosition, GameView.GameViewAnchor.TopLeftAnchor);
		
		_bottomArrowButton.Size = new Vector2(Size.x, 40);
		Vector2 bottomArrowButtonPosition = AnchorOffset(GameView.GameViewAnchor.BottomLeftAnchor);
		_bottomArrowButton.SetPosition(bottomArrowButtonPosition, GameView.GameViewAnchor.BottomLeftAnchor);
		
		_numberView.Position = new Vector2(0, 0);
		_numberView.Text = _number.ToString();
		
		_bottomArrowButton.Disabled = false;
		_topArrowButton.Disabled = false;
		if(_number == _maxNumber) _topArrowButton.Disabled = true;
		if(_number == _minNumber) _bottomArrowButton.Disabled = true;
	}
}
