using UnityEngine;
using System.Collections;

public class WeaponCarouselButton : ButtonView
{
	private ImageView _weaponImage;
	
	// This is for overlaying a "locked" icon over the weapon
	// or a "primary weapon" icon to indicate it is already
	// your primary weapon
	private ImageView _overlayImage;
	
	private string _weaponID;
	
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
	
	public override void Init()
	{
		base.Init();
		ButtonImageName = "CarouselButtonBackground";
		HighlightImageName = "CarouselButtonBackground";
		
		_weaponImage = (ImageView)gameObject.AddComponent("ImageView");
		_weaponImage.Init();
		AddSubview(_weaponImage);
		
		_overlayImage = (ImageView)gameObject.AddComponent("ImageView");
		_overlayImage.Init();
		_overlayImage.TextureName = "PrimaryWeaponOverlay";
		AddSubview(_overlayImage);
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
	}
	
}
