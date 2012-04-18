// GameState
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

// GameState is a static class in charge of saving the portion of the game's state that
// should be persistent between application launches, including the top score on each
// level and the player's current weapon selections. 
public class GameState : MonoBehaviour 
{
	public static void SaveScore(int level, int score)
	{
		PlayerPrefs.SetInt("score" + level, score);
		PlayerPrefs.Save();
	}
	
	public static int LoadScore(int level)
	{
		return PlayerPrefs.GetInt("score" + level);
	}
	
	public static void SavePrimaryWeapon(string weaponName)
	{
		PlayerPrefs.SetString("primaryWeapon", weaponName);
		PlayerPrefs.Save();
	}
	
	public static string LoadPrimaryWeapon()
	{
		return PlayerPrefs.GetString("primaryWeapon");
	}
	
	public static void SaveSecondaryWeapon(string weaponName)
	{
		PlayerPrefs.SetString("secondaryWeapon", weaponName);
		PlayerPrefs.Save();
	}
	
	public static string LoadSecondaryWeapon()
	{
		return PlayerPrefs.GetString("secondaryWeapon");
	}
}
