using UnityEngine;
using System.Collections;
using System.Linq;

public class MouseRayShooter : MonoBehaviour
{
	public int ExplosionForce = 25000;
	public int DestroyForce = 5000;
	public float DestroyForceUp = 1.0f;

	void Start()
	{
	}

	void Update()
	{
        if (Input.GetKey(KeyCode.Space))
        {

            Application.LoadLevel((Application.loadedLevel+1) % Application.levelCount);
        }

		if (Input.GetButtonDown("Fire1"))
		{
			AddDamage();
			return;
		}

		if (Input.GetButtonDown("Fire2"))
		{
			AddExplosion();
			return;
		}
	}

	private void AddExplosion()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit = Physics.RaycastAll(ray).OrderBy(a => a.distance).FirstOrDefault();
		if (hit.collider == null)
			return;
		var colliders = Physics.OverlapSphere(hit.point, 1.0f);
		foreach (var col in colliders)
		{
			if (col == null || col.rigidbody == null)
				continue;

			col.rigidbody.AddExplosionForce(ExplosionForce, hit.point, 1.0f, DestroyForceUp);
		}
	}

	private void AddDamage()
	{
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var hit = Physics.RaycastAll(ray).OrderBy(a => a.distance).FirstOrDefault();
        if (hit.collider == null)
            return;
        var pieceDestructable = hit.collider.gameObject.GetComponent<PieceDestructable>();
        if (pieceDestructable == null)
            return;
        var damageNeededForDestruction = pieceDestructable.HitPoints - pieceDestructable.DestructionHitPoints;

        pieceDestructable.TakeDamage(damageNeededForDestruction,
                                     ray.direction,
                                     Vector3.zero,
                                     gameObject,
                                     DestroyForce,
                                     0.0f);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 500, 20), "Left Mouse Button: Destroy at MousePosition");
		GUI.Label(new Rect(10, 30, 500, 20), "Right Mouse Button: Add Explosion Force");
	}
}
