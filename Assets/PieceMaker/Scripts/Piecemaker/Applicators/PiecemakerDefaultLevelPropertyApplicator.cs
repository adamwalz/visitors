using UnityEngine;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using MeshUtilities;

namespace Piecemaker
{
	public class DefaultLevelPropertyApplicatorBase: LevelPropertyApplicator
	{
		public enum ColliderType
		{
			None,
			Box,
			Sphere,
			Capsule,
			MeshOrBox,
			MeshOrSphere,
			MeshOrCapsule
		}

		[ApplyOnlyOnChildsAttribute]
		[HelpAttribute("Specifies the percentage value of loosened sub shards at which this shard will collapse. A value of 0.0 will immediately collapse this shard, 0.5 allows 50% of all sub shards to be loosened before this shard will collapse.")]
		public float PartialDestructionPercentage { get; set; }
		
		[HelpAttribute("How many seconds can this shard be invisible to all cameras until it will be removed from the scene.")]
		public int InvisibleDeathTime { get; set; }
		
		[HelpAttribute("How much health points does this shard have until it will be destructed/loosened.\n\nExample: 50 HealthPoints\n- Loosened at 25 damage points received.\n- Spawns sub shards when 50 damage points received.")]
		public int HealthPoints { get; set; }

        [ApplyOnlyOnLastChildAttribute]
        [HelpAttribute("Specifies the prefab which should be spawned when this shard should be destroyed. If not set the shard will not be destroyed.")]
        public GameObject SpawnOnDestruct { get; set; }

        public bool UseGravity { get; set; }

        public float Mass { get; set; }
		
		public float Drag { get; set; }
		
		public float AngularDrag { get; set; }
		
		public ColliderType Collider { get; set; }
		
		public int Layer { get; set; }
		
		public DefaultLevelPropertyApplicatorBase()
			: base()
		{
			PartialDestructionPercentage = 0;
			HealthPoints = 10;
            UseGravity = true;
			Mass = 1.0f;
			Drag = 0.0f;
			AngularDrag = 0.05f;
			InvisibleDeathTime = 10;
			Collider = ColliderType.MeshOrBox;
			Layer = 0;
		}
		
		public DefaultLevelPropertyApplicatorBase(DefaultLevelPropertyApplicatorBase other)
			: this()
		{
            PartialDestructionPercentage = other.PartialDestructionPercentage;
            HealthPoints = other.HealthPoints;
            UseGravity = other.UseGravity;
			Mass = other.Mass;
			Drag = other.Drag;
			AngularDrag = other.AngularDrag;
			InvisibleDeathTime = other.InvisibleDeathTime;
			Collider = other.Collider;
			Layer = other.Layer;
		}
		
		public override void ApplyOnTarget(GameObject gameObject, ApplyTargetType targetType, Mesh shardMesh, Mesh collisionMesh, Piecemaker.Settings settings)
		{
			gameObject.layer = Layer;
			
			var rigidBody = gameObject.GetComponent<Rigidbody>();
			if (rigidBody == null)
			{
				rigidBody = gameObject.AddComponent<Rigidbody>();
				rigidBody.mass = Mass;
                rigidBody.useGravity = UseGravity;
				rigidBody.drag = Drag;
				rigidBody.angularDrag = AngularDrag;
				rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}

            if (InvisibleDeathTime > 0 && targetType != ApplyTargetType.Root)
            {
                var destroyWhenNotVisible = gameObject.AddComponent<DestroyWhenNotVisible>();
                destroyWhenNotVisible.MaxInvisibleTime = InvisibleDeathTime;
            }
			
			if (gameObject.collider == null)
			{
				if (Collider == ColliderType.MeshOrBox || 
				    Collider == ColliderType.MeshOrSphere  || 
				    Collider == ColliderType.MeshOrCapsule)
				{
                    if (collisionMesh != null && collisionMesh.triangles.Length / 3 <= 255)
					{
						MeshCollider collider = gameObject.AddComponent<MeshCollider>();
                        collider.convex = true;
                        collider.sharedMesh = collisionMesh;
					}
				}
				if (Collider == ColliderType.Box || (Collider == ColliderType.MeshOrBox && gameObject.collider == null))
				{
					var collider = gameObject.AddComponent<BoxCollider>();
					collider.size = shardMesh.bounds.size;
					collider.center = shardMesh.bounds.center;
				}
				else if (Collider == ColliderType.Sphere || (Collider == ColliderType.MeshOrSphere && gameObject.collider == null))
				{
					var collider = gameObject.AddComponent<SphereCollider>();
					collider.radius = shardMesh.bounds.extents.magnitude;
					collider.center = shardMesh.bounds.center;
				}
				else if (Collider == ColliderType.Capsule || (Collider == ColliderType.MeshOrCapsule && gameObject.collider == null))
				{
					var collider = gameObject.AddComponent<CapsuleCollider>();
					collider.height = shardMesh.bounds.extents.y * 2;
					collider.radius = Mathf.Max(shardMesh.bounds.extents.x, shardMesh.bounds.extents.z);
					collider.center = shardMesh.bounds.center;
				}
				
				gameObject.collider.sharedMaterial = settings.PhysicMaterial;
			}
			
			var meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = shardMesh;
			}
			
			var meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = gameObject.AddComponent<MeshRenderer>();
				var materials = settings.Materials.ToList();
				if (settings.CutMaterial != null && targetType == ApplyTargetType.Shard)
					materials.Add(settings.CutMaterial);
				meshRenderer.sharedMaterials = materials.ToArray();
			}
		}
		
		public override void ApplyOnParentHull(GameObject hull, Piecemaker.Settings settings)
		{
			var container = hull.GetComponent<ShardContainer>();
			if (container != null)
				container.PartialDestructionPercentage = PartialDestructionPercentage;
		}

        public override void ApplySpawnOnDestructPrefab(GameObject target, GameObject prefab)
        {
        }
		
		public override LevelPropertyApplicator Clone()
		{
			return new DefaultLevelPropertyApplicatorBase(this);
		}
    }
	
	[LevelPropertyApplicator("Piecemaker Default Properties", IsDefault=true)]
	public class PiecemakerDefaultLevelPropertyApplicator : DefaultLevelPropertyApplicatorBase
	{
        [HelpAttribute("When true, the resulting shards will take damage when a collision with an impact force greater than MaxHitPoints/10 occurs, the difference between the force and this threshold will be used as damage value.")]
        public bool AllowCollisionDamage { get; set; }

		public PiecemakerDefaultLevelPropertyApplicator()
			: base()
		{
            AllowCollisionDamage = true;
        }
		
		public PiecemakerDefaultLevelPropertyApplicator(PiecemakerDefaultLevelPropertyApplicator other)
			: base(other)
		{
            AllowCollisionDamage = other.AllowCollisionDamage;
		}
		
		public override void ApplyOnTarget(GameObject gameObject, ApplyTargetType targetType, Mesh shardMesh, Mesh collisionMesh, Piecemaker.Settings settings)
		{
			base.ApplyOnTarget(gameObject, targetType, shardMesh, collisionMesh, settings);
			
			var pureDestructable = gameObject.GetComponent<PiecePureDestructable>();
			if (pureDestructable == null)
			{
				pureDestructable = gameObject.AddComponent<PiecePureDestructable>();
				pureDestructable.DestructionHitPoints = -HealthPoints / 2;
				pureDestructable.MaxHitPoints = HealthPoints / 2;
                pureDestructable.AllowCollisionDamage = AllowCollisionDamage;
                if (SpawnOnDestruct)
                {
                    pureDestructable.SpawnOnDestructPrefab = SpawnOnDestruct;
                    pureDestructable.SpawnPrefabAtBoundsCenter = true;
                }
			}
		}

        public override void ApplySpawnOnDestructPrefab(GameObject target, GameObject prefab)
        {
            base.ApplySpawnOnDestructPrefab(target, prefab);

            var destructable = target.GetComponent<PieceDestructable>();
            if (destructable != null)
                destructable.SpawnOnDestructPrefab = prefab;
        }
		
		public override LevelPropertyApplicator Clone()
		{
			return new PiecemakerDefaultLevelPropertyApplicator(this);
		}
	}
}
