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
			CollisionsToWin = 50;
		else if (GameState.GetCurrentLevel().Contains("Castle"))
			CollisionsToWin = 4;
		
		hasWon = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			if (!contact.otherCollider.gameObject.tag.Equals("weapon"))
			{	
				CollisionsToWin--;
				Destroy(contact.otherCollider.gameObject);
				Debug.Log("Piece fell off image target, collision left = " + CollisionsToWin);
			}
			
			if (CollisionsToWin <= 0 && !hasWon)
			{
				// Check if the player did good enough to get cool prizes
				view = GameObject.Find ("ViewObject");
				GameController controller;
				controller = view.GetComponent("GameController") as GameController;
				float score = controller._playingView.WeaponBar.Energy;
				if (score > 50) // unlock something
					Debug.Log("energy pretty high");
				
				// Call the winning popup
				hasWon = true;
				controller.ShowEndGameMenu(true, (int)score);
			}
		}
	}
}
