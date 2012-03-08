using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleController : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
{
	
	private HUDSearchingView _searchingView;
	private HUDGameView _gameView;
	// Use this for initialization
	void Start () 
	{
		_searchingView = GetComponent<HUDSearchingView>();
		_gameView = GetComponent<HUDGameView>();
		_searchingView.Show(false);
		_searchingView.setController(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	// IHUDSearchingViewController methods
	public void HUDSearchingViewPrintButtonPressed()
	{
		AirprintPlugin.PrintARCard();
	}
	
	public void HUDSearchingViewPlayWithoutButtonPressed()
	{
		_searchingView.Hide(false);
		_gameView.SetController(this);
		List<Texture2D> list = new List<Texture2D>();
		Texture2D tex = (Texture2D)Resources.Load("tempWeaponOne", typeof(Texture2D));
		list.Add(tex);
		tex = (Texture2D)Resources.Load("tempWeaponTwo", typeof(Texture2D));
		list.Add(tex);
		_gameView.Show(list, false);
	}
	
	public void HUDSearchingViewPauseButtonPressed()
	{
		Application.LoadLevel("sampleHUD");
	}
	
	public void HUDSearchingViewMenuButtonPressed()
	{
		Application.LoadLevel("VisitorsMainScene");	
	}
	
	// IHUDGameViewController methods
	public void HUDGameViewWeaponsSwitched(int newWeapon)
	{
		
	}
	
	public void HUDGameViewFireButtonPressed()
	{
		_gameView.Energy = _gameView.Energy - 1.0f;
	}
	
	public void HUDGameViewPauseButtonPressed()
	{
		Application.LoadLevel("sampleHUD");
	}
	
	public void HUDGameViewMenuButtonPressed()
	{
		Application.LoadLevel("VisitorsMainScene");	
	}
}
