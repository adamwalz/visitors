using UnityEngine;
using System.Collections;

public class LoadingView : GameView
{
	private ImageView _black;
	private ImageView _loading;
	
	public override void Init()
	{
		base.Init();	
		
		_black = (ImageView)gameObject.AddComponent("ImageView");
		_black.Init();
		_black.TextureName = "black";
		_black.Size = Size;
		_black.Position = new Vector2(0, 0);
		AddSubview(_black);
		
		_loading = (ImageView)gameObject.AddComponent("ImageView");
		_loading.Init();
		_loading.Size = new Vector2(256, 69);
		_loading.Position = new Vector2(0, 0);
		_loading.TextureName = "loading";
		AddSubview(_loading);
	}
}
