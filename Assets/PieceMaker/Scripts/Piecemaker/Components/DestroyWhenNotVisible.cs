using UnityEngine;
using System.Collections;

public class DestroyWhenNotVisible : MonoBehaviour
{
	public int MaxInvisibleTime = 1;
	private float invisibleTime;
	
	void Update()
	{
		if (this.renderer == null || !this.renderer.enabled)
			return;
		
		invisibleTime += Time.deltaTime;
		if (invisibleTime >= MaxInvisibleTime)
			DestroyObject(this.gameObject);
	}
	
    void OnWillRenderObject()
    {
       	invisibleTime = 0;
    }
}
 