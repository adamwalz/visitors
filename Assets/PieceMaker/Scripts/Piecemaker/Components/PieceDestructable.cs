using UnityEngine;
using System.Linq;
using System.Collections;

public class PieceDestructable : MonoBehaviour
{
    public bool IsDestroyed { get { return isDestroyed; } }
	private bool isDestroyed = false;
	private int maxHitPoints = 100;
	
	public float HitPoints = 100;
	public bool IsDead = false;
	
	public bool IsDestructable = false;
	public int DestructionHitPoints = -20;

    public bool AllowCollisionDamage = true;

    public GameObject SpawnOnDestructPrefab;
    public bool SpawnPrefabAtBoundsCenter;
	
	public int MaxHitPoints
	{
		get { return maxHitPoints; }
		set
		{
			maxHitPoints = value;
			HitPoints = maxHitPoints;
		}
	}
	
	void Start() 
	{
        if (SpawnOnDestructPrefab != null)
            PrefabCache.Instance.AddToCache(SpawnOnDestructPrefab);
		this.StartInternal();
	}
	
	void Update()
	{
		this.UpdateInternal();
	}
	
	void OnCollisionEnter(Collision collisionInfo)
	{
        if (!AllowCollisionDamage)
            return;

		var force = collisionInfo.impactForceSum.magnitude;
		if (force > MaxHitPoints * 0.1f)
		{
			var hitDirection = Vector3.Normalize(this.transform.position - collisionInfo.transform.position);
			TakeDamage(force - MaxHitPoints * 0.1f, hitDirection, transform.position, this.gameObject);
		}
	}
	
	public virtual void StartInternal() { }
	
	public virtual void UpdateInternal() { }
		
	public virtual void Die(GameObject instigator)
	{
		if (this.rigidbody != null)
			this.rigidbody.isKinematic = false;

        if (this.transform.parent != null)
        {
            var shardContainer = this.transform.parent.GetComponent<ShardContainer>();
            if (shardContainer != null)
                shardContainer.OnShardReleased(this.gameObject);
        }
		
		IsDead = true;
	}
	
	public void TakeDamage(float ammount, Vector3 damageDirection, Vector3 damageGiverPosition, GameObject instigator)
	{
		TakeDamage(ammount, damageDirection, damageGiverPosition, instigator, 0.0f, 0.0f, 0.0f);
	}
		
	public void TakeDamage(float ammount, Vector3 damageDirection, Vector3 damageGiverPosition, GameObject instigator, float force, float upForce)
	{
		TakeDamage(ammount, damageDirection, damageGiverPosition, instigator, force, upForce, 0.0f);
	}
	
	public virtual void TakeDamage(float ammount, Vector3 damageDirection, Vector3 damageGiverPosition, GameObject instigator, float force, float upForce, float explosionRadius)
	{
		HitPoints -= ammount;
		if (HitPoints <= 0.0f && !IsDead)
			Die(instigator);
		
		if (force > 0 && rigidbody != null)
			rigidbody.AddExplosionForce(force, damageGiverPosition, explosionRadius, upForce);
		
		if (IsDestructable && !isDestroyed && HitPoints <= DestructionHitPoints)
			Destruct(instigator, damageDirection, damageGiverPosition, force, upForce, explosionRadius);
	}
	
	protected void Destruct(GameObject instigator, Vector3 damageDirection, Vector3 damageGiverPosition, float force, float upForce, float explosionRadius)
	{
		isDestroyed = true;

        var hasSubDestructable = SpawnOnDestructPrefab != null;
		if (hasSubDestructable)
		{
			Vector3 appropriateScale = new Vector3(0.18f/transform.localScale.x,0.18f/transform.localScale.y,0.18f/transform.localScale.z); 
			GameObject imageTarget = GameObject.Find("ImageTarget");
            GameObject spawned;
            if (SpawnPrefabAtBoundsCenter)
                spawned = PrefabCache.Instance.CreateInstance(SpawnOnDestructPrefab, this.collider.bounds.center, transform.rotation, appropriateScale);
            else
                spawned = PrefabCache.Instance.CreateInstance(SpawnOnDestructPrefab, transform.position, transform.rotation, appropriateScale);
            spawned.transform.parent = imageTarget.transform;
			
            if (spawned.rigidbody != null && rigidbody != null)
                spawned.rigidbody.velocity = rigidbody.velocity;
			var childContainer = spawned.GetComponent<ShardContainer>();
            if (childContainer)
            {
                if (rigidbody != null)
                {
                    childContainer.InitialVelocity = rigidbody.velocity;
                }
                childContainer.Initialize();
            }
		}
		
		for (int i = 0; i < transform.childCount; ++i)
		{
			var child = transform.GetChild(i);
			var pureDestructable = child.GetComponent<PiecePureDestructable>();
			if (pureDestructable != null)
			{
				pureDestructable.Activate(instigator);					
				hasSubDestructable = true;
			}
            if (child.rigidbody != null && rigidbody != null)
                child.rigidbody.velocity = rigidbody.velocity;
		}
		this.transform.DetachChildren();
		
		if (hasSubDestructable || gameObject.GetComponent<DestroyWhenNotVisible>() == null)
			Destroy(gameObject);
	}
	
	public virtual void Heal(float ammount, Vector3 healGiverPosition, GameObject instigator, bool allowOverHeal)
	{
		if (!allowOverHeal && HitPoints + ammount > MaxHitPoints)
			HitPoints = MaxHitPoints;
		else
			HitPoints += ammount;
	}
}
