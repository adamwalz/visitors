using UnityEngine;
using System.Collections;

// The delegate type for list events
public delegate void ListEventHandler(object sender, int index);

public class NetworkGameListView : GameView
{
	private ImageView _black;
	private NetworkGameListCellInfo[] _infoForCells;
	private ButtonView _upButton;
	private ButtonView _downButton;
	private int _listPosition;
	private NetworkGameListCell _cellOne;
	private NetworkGameListCell _cellTwo;
	
	public event ListEventHandler ListItemPressed;
	
	public NetworkGameListCellInfo[] InfoForCells
	{
		get{return _infoForCells;}
		set
		{
			_infoForCells = value;
		}
	}
	
	public override void Init()
	{
		base.Init();
		_infoForCells = new NetworkGameListCellInfo[0];
		_listPosition = 0;
		
		_black = (ImageView)gameObject.AddComponent("ImageView");
		_black.Init();
		_black.TextureName = "black";
		AddSubview(_black);
		
		_upButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_upButton.Init();
		_upButton.ButtonImageName = "numberSelectorUp";
		_upButton.HighlightImageName = "numberSelectorUpHighlight";
		_upButton.ButtonPressed += new EventHandler(UpButtonPressed);
		AddSubview(_upButton);
		
		_downButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_downButton.Init();
		_downButton.ButtonImageName = "numberSelectorDown";
		_downButton.HighlightImageName = "numberSelectorDownHighlight";
		_downButton.ButtonPressed += new EventHandler(DownButtonPressed);
		AddSubview(_downButton);
		
		_cellOne = (NetworkGameListCell)gameObject.AddComponent("NetworkGameListCell");
		_cellOne.Init();
		_cellOne.ButtonPressed += new EventHandler(CellPressed);
		AddSubview(_cellOne);
		
		_cellTwo = (NetworkGameListCell)gameObject.AddComponent("NetworkGameListCell");
		_cellTwo.Init();
		_cellTwo.ButtonPressed += new EventHandler(CellPressed);
		AddSubview(_cellTwo);
	}
	
	private void CellPressed(object sender)
	{
		if(ListItemPressed != null)
		{
			if(sender == _cellOne)
				ListItemPressed(this, _listPosition);
			if(sender == _cellTwo)
				ListItemPressed(this, _listPosition + 1);
		}
	}
	
	private void UpButtonPressed(object sender)
	{ 
		_listPosition--;
	}
	
	private void DownButtonPressed(object sender)
	{
		_listPosition++;
	}
	
	public override void RefreshContent()
	{
		
		_black.Size = Size;
		_black.Position = new Vector2(0, 0);
		
		_upButton.Size = new Vector2(60, 40);
		_upButton.SetPosition(AnchorOffset(GameView.GameViewAnchor.TopRightAnchor), GameView.GameViewAnchor.TopRightAnchor);
		
		_downButton.Size = new Vector2(60, 40);
		_downButton.SetPosition(AnchorOffset(GameView.GameViewAnchor.BottomRightAnchor), GameView.GameViewAnchor.BottomRightAnchor);
		
		_upButton.Disabled = false;
		_downButton.Disabled = false;
		if(_listPosition == 0) _upButton.Disabled = true;
		if(_infoForCells.Length > _listPosition - 1) _downButton.Disabled = true;
		
		// le hack
		_cellOne.Position = new Vector2(9000, -9000);
		_cellTwo.Position = new Vector2(-9000, 9000);
		
		if(_infoForCells.Length > _listPosition)
		{
			_cellOne.Size = new Vector2(Size.x - 60, 45);
			_cellOne.SetPosition(AnchorOffset(GameView.GameViewAnchor.TopLeftAnchor), GameView.GameViewAnchor.TopLeftAnchor);
			_cellOne.Info = _infoForCells[_listPosition];
		}
		
		if(_infoForCells.Length > _listPosition + 1)
		{
			_cellTwo.Size = new Vector2(Size.x - 60, 45);
			_cellTwo.SetPosition(_cellOne.GetPosition(GameView.GameViewAnchor.BottomLeftAnchor), GameView.GameViewAnchor.TopLeftAnchor);
			_cellTwo.Info = _infoForCells[_listPosition + 1];
		}
	}
}
