// GameState
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

// GameState is a static class in charge of saving the portion of the game's state that
// should be persistent between application launches, including the top score on each
// level and the player's current weapon selections. 
public class GameState : MonoBehaviour 
{
	static string _currentLevel;
	static bool _isServer;
	static bool _isAugmented;
	static int _playerNumber;
	
	public static void SaveScore(string level, int score)
	{
		PlayerPrefs.SetInt("score" + level, score);
		PlayerPrefs.Save();
	}
	
	public static void SetCurrentLevel(string level, bool isAugmented)
	{
		_currentLevel = level;
		_isAugmented = isAugmented;
		Debug.Log("Saving level: " + _currentLevel + ", Augmented: " + _isAugmented);
	}
	
	public static string GetCurrentLevel()
	{
		Debug.Log("Entering level: " + _currentLevel);
		return _currentLevel;
	}
	
	public static int LoadScore(string level)
	{
		return PlayerPrefs.GetInt("score" + level);
	}
	
	public static void SavePrimaryWeapon(string weaponName)
	{
		
		PlayerPrefs.SetString("primaryWeapon", weaponName);
		PlayerPrefs.Save();
		Debug.Log("Saving primary weapon: " + weaponName);
	}
	
	public static string LoadPrimaryWeapon()
	{
		string weaponName = PlayerPrefs.GetString("primaryWeapon");
		Debug.Log("Loading primary weapon: " + weaponName);
		return weaponName;
	}
	
	public static void SaveSecondaryWeapon(string weaponName)
	{
		PlayerPrefs.SetString("secondaryWeapon", weaponName);
		PlayerPrefs.Save();
		Debug.Log("Saving secondary weapon: " + weaponName);
	}
	
	public static string LoadSecondaryWeapon()
	{
		string weaponName = PlayerPrefs.GetString("secondaryWeapon");
		Debug.Log("Loading primary weapon: " + weaponName);
		return weaponName;
	}

	public static bool GetIsServer()
	{
		return _isServer;
	}
	
	public static void SetIsServer(bool multiplayerBoolean)
	{
		_isServer = multiplayerBoolean;
	}
	
	public static int GetPlayerNumber()
	{
		return _playerNumber;
	}
	
	public static void SetPlayerNumber(int numberOfPlayers)
	{
		_playerNumber = numberOfPlayers;
	}
	
	public static bool GetIsAugmented()
	{
		return _isAugmented;
	}
}
