using UnityEngine;
using System.Collections;

public class WeaponCarouselButton : ButtonView
{
	private ImageView _weaponImage;
	
	// This is for overlaying a "locked" icon over the weapon
	// or a "primary weapon" icon to indicate it is already
	// your primary weapon
	private ImageView _overlayImage;
	private ImageView _haloImage;
	
	private string _weaponID;
	private bool _selected;
	
	public string WeaponID
	{
		get {return _weaponID;}
		set 
		{
			_weaponID = value;
			_weaponImage.TextureName = _weaponID;
		}
	}
	
	public ImageView Overlay
	{
		get {return _overlayImage;}
	}
	
	public bool Selected
	{
		get {return _selected;}
		set {_selected = value;}
	}
	
	public override void Init()
	{
		base.Init();
		ButtonImageName = "CarouselButtonBackground";
		HighlightImageName = "CarouselButtonBackground";
		
		_haloImage = (ImageView)gameObject.AddComponent("ImageView");
		_haloImage.Init();
		_haloImage.TextureName = "CarouselButtonHalo";
		AddSubview(_haloImage);
		
		_weaponImage = (ImageView)gameObject.AddComponent("ImageView");
		_weaponImage.Init();
		AddSubview(_weaponImage);
		
		_overlayImage = (ImageView)gameObject.AddComponent("ImageView");
		_overlayImage.Init();
		_overlayImage.TextureName = "";
		AddSubview(_overlayImage);
		
		_selected = false;
	}
	
	public override bool RespondsToTouchInput()
	{
		return !_disabled;
	}
	
	public override void RefreshContent()
	{
		base.RefreshContent();
		_weaponImage.Size = new Vector2(Size.x / 1.2f, Size.y / 1.2f);
		_weaponImage.Position = new Vector2(0, 0);
		
		_overlayImage.Size = Size;
		_overlayImage.Position = new Vector2(0, 0);
		
		if(_selected && _haloImage.State != GameView.GameViewState.Showing)_haloImage.Show(false);
		if(!_selected && _haloImage.State != GameView.GameViewState.Hidden)_haloImage.Hide(false);
		_haloImage.Size = new Vector2(Size.x + 20, Size.y + 20);
		_haloImage.Position = new Vector2(0, 0);
	}
	
}
