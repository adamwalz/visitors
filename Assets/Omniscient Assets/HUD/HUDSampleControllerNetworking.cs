using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDSampleControllerNetworking : MonoBehaviour, IHUDSearchingViewController, IHUDGameViewController
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
		Texture2D tex = (Texture2D)Resources.Load("fireballWeapon", typeof(Texture2D));
		list.Add(tex);
		tex = (Texture2D)Resources.Load("blueWeaponOne", typeof(Texture2D));
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
		GameObject cam = GameObject.Find("ARCamera");
		Debug.Log(cam.transform.position);
		GameObject thePrefab = (GameObject)Resources.Load("StrongBall");
		GameObject instance = (GameObject)Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
		Vector3 fwd = cam.transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
		
		
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
