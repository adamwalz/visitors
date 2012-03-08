using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ShardContainer : MonoBehaviour
{
	public float PartialDestructionPercentage = 0.0f;
    public Vector3 InitialVelocity;

	int shardsReleased = 0;
	int shardsAvailable = 0;
	bool spawnedShards = false;
	bool initialized = false;
	
	void Start()
	{
		Initialize();
	}
	
	public void Initialize()
	{
		if (initialized)
			return;
		
		initialized = true;
		
		if (PartialDestructionPercentage <= 0.0f)
			SpawnShards();
		else
		{
            InitialVelocity = Vector3.zero;
			shardsAvailable = this.transform.childCount;
			for (int i = 0; i < shardsAvailable; ++i)
			{
                var childGameObject = this.transform.GetChild(i).gameObject;
                childGameObject.rigidbody.isKinematic = true;
                childGameObject.active = true;
			}
		}
	}
	
	public void OnShardReleased(GameObject shard)
	{
		shardsReleased++;
		shard.transform.parent = null;
		
		if (shardsReleased < shardsAvailable * PartialDestructionPercentage)
			return;
		
		SpawnShards();
	}
	
	public void SpawnShards()
	{
		if (spawnedShards)
			return;
		
		spawnedShards = true;
		
		for (int i = 0; i < this.transform.childCount; ++i)
		{
			var childGameObject = this.transform.GetChild(i).gameObject;
			childGameObject.active = true;
            if (childGameObject.rigidbody != null)
            {
                childGameObject.rigidbody.isKinematic = false;
                childGameObject.rigidbody.velocity = InitialVelocity;
            }
		}

		this.transform.DetachChildren();
		Destroy(this.gameObject);
	}
}
