using UnityEngine;
using System.Collections;

public class NetworkGameListCellInfo : Object 
{
	private string _levelPreviewImage;
	private string _gameNameText;
	private string _gameDescriptionText;
	private string _primaryWeapon;
	private string _secondaryWeapon;
	
	public string LevelName
	{
		get{return _levelPreviewImage;}
		set{_levelPreviewImage = value;}
	}
	
	public string GameName
	{
		get{return _gameNameText;}
		set{_gameNameText = value;}
	}
	
	public string GameDescription
	{
		get{return _gameDescriptionText;}
		set{_gameDescriptionText = value;}
	}
	
	public string PrimaryWeaponImageName
	{
		get{return _primaryWeapon;}
		set{_primaryWeapon = value;}
	}
	
	public string SecondaryWeaponImageName
	{
		get{return _secondaryWeapon;}
		set{_secondaryWeapon = value;}
	}
}
