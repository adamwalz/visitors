using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour 
{
	private GameScreen _mainScreen;
	private ButtonView _leftButton;
	private ButtonView _rightButton;

	
	// Use this for initialization
	void Start () 
	{
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		
		_leftButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_leftButton.Init();
		_leftButton.ButtonImageName = "CarouselLeft";
		_leftButton.HighlightImageName = "CarouselLeft";
		_leftButton.SetPosition(new Vector2(35, _mainScreen.Size.y / 2), GameView.GameViewAnchor.MiddleAnchor);
		_leftButton.Size = new Vector2(60, 250);
		_leftButton.Show(false);
		_mainScreen.AddView(_leftButton);
		
		_rightButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_rightButton.Init();
		_rightButton.ButtonImageName = "CarouselRight";
		_rightButton.HighlightImageName = "CarouselRight";
		_rightButton.SetPosition(new Vector2(_mainScreen.Size.x - 35, _mainScreen.Size.y / 2), GameView.GameViewAnchor.MiddleAnchor);
		_rightButton.Size = new Vector2(60, 250);
		_rightButton.Show(false);
		_mainScreen.AddView(_rightButton);
		
		print("Planet controller working!");
	}
	
	void Update()
	{
		Vector3 rotation = new Vector3(0,0,0);
		if(_leftButton.Highlighted)
		{
			rotation = new Vector3(0, 50, 0);
			transform.Rotate(rotation * Time.deltaTime*2.0F);	
		}
		if(_rightButton.Highlighted)
		{
			rotation = new Vector3(0, -50, 0);
			transform.Rotate(rotation * Time.deltaTime*2.0F);
		}
	}
}
