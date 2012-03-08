using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MeshUtilities;

namespace Piecemaker
{		
	public class LevelData
	{
		public LevelData()
		{
			SliceXCount = 1;
			SliceYCount = 1;
			SliceZCount = 1;
			
			var availableLevelPropertyApplicators =
				    (from a in System.AppDomain.CurrentDomain.GetAssemblies()
				    from t in a.GetTypes()
				    let attributes = t.GetCustomAttributes(typeof(Piecemaker.LevelPropertyApplicatorAttribute), true)
				    where attributes != null && attributes.Length > 0
				    select new KeyValuePair<string, System.Type>(attributes.Cast<Piecemaker.LevelPropertyApplicatorAttribute>().FirstOrDefault().Name, t))
						.ToArray();
			PropertyApplicators = availableLevelPropertyApplicators.Where(pair =>
				{
					return ((Piecemaker.LevelPropertyApplicatorAttribute)(pair.Value.GetCustomAttributes(typeof(Piecemaker.LevelPropertyApplicatorAttribute), true)[0])).IsDefault;
				}).Select(pair => (Piecemaker.LevelPropertyApplicator)System.Activator.CreateInstance(pair.Value)).ToList();
		}
		
		public LevelData(LevelData other)
		{
			SliceXCount = other.SliceXCount;
			SliceYCount = other.SliceYCount;
			SliceZCount = other.SliceZCount;
			SliceChaos = other.SliceChaos;
			PropertyApplicators = other.PropertyApplicators.Select(p => p.Clone()).ToList();
			PrefabTemplate = other.PrefabTemplate;
		}
		
		public int SliceXCount;
		public int SliceYCount;
		public int SliceZCount;
		public Vector3 SliceChaos;

		public bool IsRoot;

        public List<LevelPropertyApplicator> PropertyApplicators = new List<LevelPropertyApplicator>();
		
		public GameObject PrefabTemplate;
	}
}