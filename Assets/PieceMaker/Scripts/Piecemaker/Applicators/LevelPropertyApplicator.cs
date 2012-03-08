using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MeshUtilities;

namespace Piecemaker
{
	public enum ApplyTargetType
	{
		Root,
		Shard
	}
	
	public abstract class LevelPropertyApplicator
	{
		public abstract void ApplyOnTarget(GameObject target, ApplyTargetType targetType, Mesh shardMesh, Mesh collisionMesh, Piecemaker.Settings settings);
		public abstract void ApplyOnParentHull(GameObject hull, Piecemaker.Settings settings);
        public abstract void ApplySpawnOnDestructPrefab(GameObject target, GameObject prefab);
		
		public abstract LevelPropertyApplicator Clone();
	}
	
	public class LevelPropertyApplicatorAttribute : NamedAttribute
	{
		public bool IsDefault { get; set; }
		public System.Type ReplacesType { get; set; }
		
		public LevelPropertyApplicatorAttribute()
			: base()
		{
		}
		
		public LevelPropertyApplicatorAttribute(string name)
			: base(name)
		{
		}
		
		static KeyValuePair<LevelPropertyApplicatorAttribute, System.Type>[] availableAttributes;
		public static KeyValuePair<LevelPropertyApplicatorAttribute, System.Type>[] GetAvailable()
		{
			if (availableAttributes == null)
			{
				availableAttributes = NamedAttribute.GetAvailable<LevelPropertyApplicatorAttribute>();
				availableAttributes = availableAttributes.Where(pair =>
					{
						return !availableAttributes.Any(pair2 => pair2.Key.ReplacesType == pair.Value);
					}).ToArray();
			}
			return availableAttributes;
		}
	}
	
	public class ApplyOnlyOnChildsAttribute : System.Attribute {}
    public class ApplyOnlyOnLastChildAttribute : System.Attribute {}
}
