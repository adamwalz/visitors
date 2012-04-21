using UnityEngine;
using System.Collections;

public class WeaponBar : GameView
{
	private ImageView _backgroundBar;
	private ImageView _energyImage;
	private float _energy;
	
	public float Energy
	{
		get{return _energy;}
		set
		{
			_energy = value;
			if(_energy < 0) _energy = 0;
		}
	}
	
	public override void Init()
	{
		base.Init();
		
		_energy = 100.0f;
		_energyImage = (ImageView)gameObject.AddComponent("ImageView");
		_energyImage.Init();
		_energyImage.TextureName = "energy";
		AddSubview(_energyImage);
		
		_backgroundBar = (ImageView)gameObject.AddComponent("ImageView");
		_backgroundBar.Init();
		_backgroundBar.TextureName = "energyBarCropped";
		AddSubview(_backgroundBar);
		
		
	}
	
	public override void RefreshContent()
	{
		_backgroundBar.Size = Size;
		_backgroundBar.Position = new Vector2(0, 0);
		_energyImage.Size = new Vector2(Size.x * 0.8f  * (_energy / 100.0f), Size.y * 0.8f);
		_energyImage.SetPosition(_backgroundBar.GetPosition(GameView.GameViewAnchor.MiddleLeftAnchor), GameView.GameViewAnchor.MiddleLeftAnchor);
		_energyImage.Position += new Vector2(Size.x * 0.1f, 0);
	}
}
