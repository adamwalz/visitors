using UnityEngine;
using System.Collections;

public class ConstrainMovementUntilCollision : MonoBehaviour
{
	private bool constrained;
	
	void Start()
	{
		this.gameObject.rigidbody.constraints = RigidbodyConstraints.FreezePositionX |
												RigidbodyConstraints.FreezePositionY |
												RigidbodyConstraints.FreezePositionZ | 
												RigidbodyConstraints.FreezeRotation;
		constrained = true;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (!constrained)
			return;
		
		foreach (ContactPoint contact in collision.contacts)
		{
			if (contact.otherCollider.gameObject.tag.Equals("weapon") ||
				contact.otherCollider.gameObject.tag.Equals("broken"))
			{
				this.gameObject.rigidbody.constraints = RigidbodyConstraints.None;
				//this.gameObject.tag = "broken";
				constrained = false;
				Debug.Log("Broken: No more constraints");
				return;
			}
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		OnCollisionEnter(collision);
	}
	
	//void OnCollisionStay(Collision collision)
	//{
	//	OnCollisionEnter(collision);
	//}
}
