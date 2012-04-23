using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinningCollisions : MonoBehaviour
{

	private static int CollisionsToWin;
	private GameObject view;
	private bool hasWon;

	void Start()
	{
		if (GameState.GetCurrentLevel().Contains("Egypt"))
			CollisionsToWin = 25;
		else if (GameState.GetCurrentLevel().Contains("Castle"))
			CollisionsToWin = 4;
		else
			CollisionsToWin = 5;
		
		hasWon = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (!contact.otherCollider.gameObject.tag.Equals("weapon"))
			{	
				CollisionsToWin--;
				Debug.Log("Piece fell off image target, collision left = " + CollisionsToWin);
				Destroy(contact.otherCollider.gameObject);
			}
			
			if (CollisionsToWin <= 0 && !hasWon)
			{
				// Check if the player did good enough to get cool prizes
				view = GameObject.Find ("ViewObject");
				GameController controller;
				controller = view.GetComponent("GameController") as GameController;
				float score = controller._playingView.WeaponBar.Energy;
				if (score > 50 && GameState.LoadIsLocked()) // unlock something
				{
					GameState.SaveIsLocked(false);
					// TODO: pop up message to say that new weapon is unlocked
					Debug.Log("New weapon unlocked");
				}
				
				// Call the winning popup
				hasWon = true;
				controller.ShowEndGameMenu(true, (int)score);
			}
		}
	}
}
