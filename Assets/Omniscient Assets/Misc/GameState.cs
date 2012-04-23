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
	static bool _locked;
	static Stack _scenes;
	
	public static void ResetGameState()
	{
		_currentLevel = "";
		_isServer = false;
		_isAugmented = false;
		_playerNumber = 0;
	}
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
	
	public static bool LoadIsLocked()
	{
		return (PlayerPrefs.GetInt("isLocked") != 0);
	}
	
	public static void SaveIsLocked(bool isLocked)
	{
		int locked = 0;
		if(isLocked) locked = 1;
		PlayerPrefs.SetInt("isLocked", locked);
		PlayerPrefs.Save();
	}
	
	public static void GoToScene(GameScreen screen, string scene)
	{
		LoadingView loading = (LoadingView)screen.gameObject.AddComponent("LoadingView");
		loading.Size = screen.Size;
		loading.Position = new Vector2(screen.Size.x / 2, screen.Size.y / 2);
		loading.Init();
		screen.AddView(loading);
		loading.Show(false);
		screen.StartCoroutine(LoadSceneSoon(scene));
	}
	
	public static IEnumerator LoadSceneSoon(string scene)
	{
		yield return new WaitForSeconds(0.001f);
		Application.LoadLevel(scene);
	}
	
	// Back button stuff
	
	public static void PopScene()
	{
		if(_scenes == null) _scenes = new Stack();
		if(_scenes.Count > 0) 
		{
			_scenes.Pop();
			Application.LoadLevel((string)_scenes.Pop());
		}
	}
	
	public static void PushScene(string scene)
	{
		if(_scenes == null) _scenes = new Stack();
		_scenes.Push(scene);
	}
}
