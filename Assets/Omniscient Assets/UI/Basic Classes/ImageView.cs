// ImageView
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

public class ImageView : GameView 
{
	private string _textureName;
	private GUITexture _imageGUITexture;
	
	// The filename of the texture to be used
	public string TextureName
	{
		get { return _textureName; }
		set { 
				_textureName = value;
				if(State != GameView.GameViewState.Hidden)
				{
					if(_imageGUITexture != null) Destroy(_imageGUITexture);
					_imageGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
					Texture2D tex = LoadTexture(_textureName);
					_imageGUITexture.texture = tex;
				}
			}
	}
	
	public GUITexture ImageGUITexture
	{
		get{return _imageGUITexture;}	
	}

	// Use this for initialization
	public new void Init () 
	{
		base.Init();
		_textureName = "";
	}
	
	// Given the provided texture name, attempts to load a texture. If the device has a high-resolution screen,
	// this method will look for the file textureName@2x instead of textureName. If this can't be found, it
	// will try to find the non-@2x version to use instead.
	private Texture2D LoadTexture(string textureName)
	{
		Texture2D tex = null;
		if(UtilityPlugin.ContentScaleFactor() == 2) tex = (Texture2D)Resources.Load(textureName + "@2x", typeof(Texture2D));
		if(tex == null) tex = (Texture2D)Resources.Load(textureName, typeof(Texture2D));
		
		return tex;
	}
	
	public override void Show(bool animated)
	{
		base.Show(animated);
		_imageGUITexture = (GUITexture)((GameObject)Instantiate(Resources.Load("HUDElement"))).GetComponent(typeof(GUITexture));
		Texture2D tex = LoadTexture(_textureName);
		_imageGUITexture.texture = tex;
		_imageGUITexture.pixelInset = new Rect(0, 0, 0, 0);
	}
	
	protected override void OnHidden()
	{
		Destroy(_imageGUITexture);
	}
	
	// Position the GUITexture correctly based on our absolute position
	public override void RefreshContent()
	{
		Vector2 absPosition = GetAbsolutePosition(GameViewAnchor.BottomLeftAnchor);
		float contentScaleFactor = (float)UtilityPlugin.ContentScaleFactor();
		_imageGUITexture.pixelInset = new Rect(absPosition.x * contentScaleFactor, absPosition.y * contentScaleFactor, Size.x * contentScaleFactor, Size.y * contentScaleFactor);
		_imageGUITexture.transform.position = new Vector3(0, 0, _zValue);
		Color color = _imageGUITexture.color;
		if(State == GameView.GameViewState.AnimatingIn)
		{
			color.a = _animationTimer / _animationDuration * 0.5f;
			_imageGUITexture.color = color;
		}
		if(State == GameView.GameViewState.AnimatingOut)
		{
			color.a = 0.5f - _animationTimer / _animationDuration * 0.5f;
			_imageGUITexture.color = color;
		}
	}
}
