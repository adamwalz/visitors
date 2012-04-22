using UnityEngine;
using System.Collections;

public class NetworkGameListCell : ButtonView
{
	private ImageView _levelPreviewImage;
	private TextView _gameNameText;
	private TextView _gameDescriptionText;
	private ImageView _primaryWeapon;
	private ImageView _secondaryWeapon;
	private NetworkGameListCellInfo _info;
	
	public string LevelName
	{
		get{return _levelPreviewImage.TextureName;}
		set{_levelPreviewImage.TextureName = value;}
	}
	
	public NetworkGameListCellInfo Info
	{
		get{return _info;}
		set
		{
			_info = value;
			_levelPreviewImage.TextureName = value.LevelName;
			_gameNameText.Text = value.GameName;
			_gameDescriptionText.Text = value.GameDescription;
			_primaryWeapon.TextureName = value.PrimaryWeaponImageName;
			_secondaryWeapon.TextureName = value.SecondaryWeaponImageName;
		}
	}
	
	public override void Init()
	{
		base.Init();
		this.ButtonImageName = "black";
		this.HighlightImageName = "yellow";
		
		_levelPreviewImage = (ImageView)gameObject.AddComponent("ImageView");
		_levelPreviewImage.Init();
		AddSubview(_levelPreviewImage);
		
		_gameNameText = (TextView)gameObject.AddComponent("TextView");
		_gameNameText.Init();
		_gameNameText.Text = "";
		_gameNameText.Small = false;
		AddSubview(_gameNameText);
		
		_gameDescriptionText = (TextView)gameObject.AddComponent("TextView");
		_gameDescriptionText.Init();
		_gameDescriptionText.Text = "";
		_gameDescriptionText.Small = true;
		AddSubview(_gameDescriptionText);
		
		_primaryWeapon = (ImageView)gameObject.AddComponent("ImageView");
		_primaryWeapon.Init();
		AddSubview(_primaryWeapon);
		
		_secondaryWeapon = (ImageView)gameObject.AddComponent("ImageView");
		_secondaryWeapon.Init();
		AddSubview(_secondaryWeapon);
	}
	
	public override void RefreshContent()
	{
		base.RefreshContent();
		_levelPreviewImage.Size = new Vector2(50, 40);
		_levelPreviewImage.SetPosition(new Vector2(AnchorOffset(GameView.GameViewAnchor.MiddleLeftAnchor).x + 2, 0),GameView.GameViewAnchor.MiddleLeftAnchor);
		
		_gameNameText.InternalGUIText.anchor = TextAnchor.UpperLeft;
		Vector2 gameNamePosition = _levelPreviewImage.GetPosition(GameView.GameViewAnchor.TopRightAnchor) + new Vector2(2, 2);
		_gameNameText.Position = gameNamePosition;
		
		_gameDescriptionText.InternalGUIText.anchor = TextAnchor.LowerLeft;
		Vector2 gameDescriptionPosition = _levelPreviewImage.GetPosition(GameView.GameViewAnchor.BottomRightAnchor) + new Vector2(2, -2);
		_gameDescriptionText.Position = gameDescriptionPosition;
		
		_secondaryWeapon.Size = new Vector2(40, 40);
		Vector2 secondaryWeaponPosition = AnchorOffset(GameView.GameViewAnchor.MiddleRightAnchor) + new Vector2(-2, 0);
		_secondaryWeapon.SetPosition(secondaryWeaponPosition, GameView.GameViewAnchor.MiddleRightAnchor);
		
		_primaryWeapon.Size = new Vector2(40, 40);
		Vector2 primaryWeaponPosition = _secondaryWeapon.GetPosition(GameView.GameViewAnchor.MiddleLeftAnchor);
		_primaryWeapon.SetPosition(primaryWeaponPosition, GameView.GameViewAnchor.MiddleRightAnchor);
	}
}
