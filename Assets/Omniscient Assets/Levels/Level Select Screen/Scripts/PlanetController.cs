using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour 
{
	private GameScreen _mainScreen;
	//private ButtonView _leftButton;
	//private ButtonView _rightButton;
	private float previousX;
	private float currentX;
	private float changeOfX;
	private bool addEndingRotation = false;
	private ButtonView _backButton;
	

	
	// Use this for initialization
	/*
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
	}*/
	
	void Start ()
	{
		// Push our current scene onto the game state (so we can go back to it with a back button)
		GameState.PushScene(Application.loadedLevelName);
				
		_mainScreen = (GameScreen)gameObject.AddComponent("GameScreen");
		_backButton = (ButtonView)gameObject.AddComponent("ButtonView");
		_backButton.Init();
		_backButton.ButtonImageName = "backNavigationButton";
		_backButton.HighlightImageName = "backNavigationButtonHighlight";
		_backButton.Size = new Vector2(40, 40);
		Vector2 backButtonPosition = new Vector2(10, _mainScreen.Size.y - 10);
		_backButton.SetPosition(backButtonPosition, GameView.GameViewAnchor.TopLeftAnchor);
		_backButton.ButtonPressed += new EventHandler(BackPressed);
		_mainScreen.AddView(_backButton);
		_backButton.Show(false);
	}
	
	public void BackPressed(object sender)
	{
		GameState.PopScene();
	}
	
	void Update()
	{
		Vector3 rotation = new Vector3(0,0,0);
		if(Input.GetMouseButtonDown(0))
		{
			previousX = Input.mousePosition.x;
		}
		
		if(Input.GetMouseButton(0))
		{
			currentX = Input.mousePosition.x;
			changeOfX = (float)((currentX-previousX));
			changeOfX = changeOfX * (float)2.0;
			
			rotation = new Vector3(0, -changeOfX, 0);
			transform.Rotate(rotation * Time.deltaTime*2.0F);
			
			previousX = Input.mousePosition.x;
		}
		
		
		if (Input.GetMouseButtonUp(0))
		{
			addEndingRotation = true;
			changeOfX = changeOfX/2;
		}
			
		
		if (addEndingRotation == true)
		{
			if (changeOfX > 10)
			{
				changeOfX = changeOfX - (float)2.0;
				rotation = new Vector3(0, -changeOfX, 0);
				transform.Rotate(rotation * Time.deltaTime*2.0F);		
			}
			
			else if (changeOfX < -10)
			{
				changeOfX = changeOfX + (float)2.0;
				rotation = new Vector3(0, -changeOfX, 0);
				transform.Rotate(rotation * Time.deltaTime*2.0F);		
			}
			
			else
			{
				changeOfX = 0;
				addEndingRotation = false;

			}
			
		}
				
		
		
	}
	
	void ChangeIsPositive(float change, Vector3 rotation)
	{
		change = change - (float)2.0;
		rotation = new Vector3(0, -change, 0);
		transform.Rotate(rotation * Time.deltaTime*2.0F);
			
		if(change <= 0)
		{
			addEndingRotation = false;
		}
	}
	
	void ChangeIsNegative(float change, Vector3 rotation)
	{
		change = change + (float)2.0;
		rotation = new Vector3(0, -change, 0);
		transform.Rotate(rotation * Time.deltaTime*2.0F);
			
		if(change >= 0)
		{
			addEndingRotation = false;
		}
	}
}