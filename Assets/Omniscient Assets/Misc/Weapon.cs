// Weapon
// Created by Nathan Swenson

using UnityEngine;
using System.Collections;

public class Weapon : Object 
{
	private string _id;
	private string _name;
	private string _description;
	private string _unlockRequirements;
	
	// The internal identifier for this weapon. This should also be
	// used as the texture name for the weapon's icon.
	public string ID
	{
		get{return _id;}	
	}
	
	// The full name for this weapon, which the user will see
	public string Name
	{
		get{return _name;}
	}
	
	// A short description for this weapon
	public string Description
	{
		get{return _description;}	
	}
	
	// If the weapon is initially locked, this string specifies
	// to the user how to unlock it
	public string UnlockRequirements
	{
		get{return _unlockRequirements;}	
	}
	
	public static Weapon GetWeaponById(string id)
	{
		Weapon weapon = new Weapon();
		if(id.Equals("FireballWeapon"))
		{
			weapon._id = "FireballWeapon";
			weapon._name = "weapon one";
			weapon._description = "Hello, this is a cool weapon!";
			weapon._unlockRequirements = "None";
			return weapon;
		}
		
		return null;
	}
	
	public static string[] WeaponIDs()
	{
		return new string[]{"FireballWeapon"};
	}
	
	// A computed value to determine if the weapon is currently locked
	public bool isLocked()
	{
		return false;
	}
}
