using UnityEngine;

public class PiecePureDestructable : PieceDestructable
{
	void Awake()
	{
		this.IsDestructable = true;
		this.DestructionHitPoints = 0;
	}
	
	public override void StartInternal()
	{
		base.StartInternal();
		
		if (this.gameObject.renderer != null)
			this.gameObject.renderer.enabled = true;
	}
	
	public virtual void Activate(GameObject instigator)
	{
		this.gameObject.active = true;
		Die(instigator);
	}
	
	public override void Die(GameObject instigator)
	{
		base.Die(instigator);
		if (this.IsDead)
			return;
	}
}
