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
		if(id.Equals("DestroyBall"))
		{
			weapon._id = "DestroyBall";
			weapon._name = "Destroyer";
			weapon._description = "Ball that disintegrates the first thing it touches";
			weapon._unlockRequirements = "Conquer 1 Civilization";
		}
		else if(id.Equals("StrongBall"))
		{
			weapon._id = "StrongBall";
			weapon._name = "Stone";
			weapon._description = "Ball with a heavy mass to inflict damage to stone buildings";
			weapon._unlockRequirements = "None";
		}
		else if(id.Equals("Fireball1"))
		{
			weapon._id = "Fireball1";
			weapon._name = "Fireball";
			weapon._description = "Heavy ball lit fire to damage both stone and wood buildings";
			weapon._unlockRequirements = "None";
		}
		else if(id.Equals("Fireball2"))
		{
			weapon._id = "Fireball2";
			weapon._name = "Exploder";
			weapon._description = "Fireball which explodes on impact";
			weapon._unlockRequirements = "Conquer 2 Civilizations";
		}
		else if(id.Equals("Fireball3"))
		{
			weapon._id = "Fireball3";
			weapon._name = "Acid Blast";
			weapon._description = "Light weapon which does burning damage on impact";
			weapon._unlockRequirements = "Conquer 3 Civilizations";
		}
		else if(id.Equals("Fireball4"))
		{
			weapon._id = "Fireball4";
			weapon._name = "Cherry Bomb";
			weapon._description = "Light weapon which can be shot into crevices and explode inside buildings";
			weapon._unlockRequirements = "Conquer 3 Civilizations";
		}
		else if(id.Equals("Fireball5"))
		{
			weapon._id = "Fireball5";
			weapon._name = "Water Cannon";
			weapon._description = "Light weapon which lowers the defesnses of buildings";
			weapon._unlockRequirements = "Conquer 2 Civilizations";
		}
		else if(id.Equals("tornado"))
		{
			weapon._id = "tornado";
			weapon._name = "weapon one";
			weapon._description = "Inflicts continuous damage to rip buildings apart over time";
			weapon._unlockRequirements = "Conquer 4 Civilizations";
		}
		else
			return null;
		
		return weapon;
	}
	
	public static string[] WeaponIDs()
	{
		return new string[]{"DestroyBall", 
							"StrongBall",
							"Fireball1", 
							"Fireball2",
							"Fireball3",
							"Fireball4",
							"Fireball5",
							"tornado"
							};
	}
	
	// A computed value to determine if the weapon is currently locked
	public bool isLocked()
	{
		return true;
	}
}
