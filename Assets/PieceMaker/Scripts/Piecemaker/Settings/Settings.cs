using System.Collections.Generic;
using MeshUtilities;
using SerializaahNS;
using UnityEngine;

namespace Piecemaker
{		
	[System.Serializable]
    public class Settings : Serializaah
	{
		public List<LevelData> LevelData = new List<LevelData>(new LevelData[] {new LevelData() { IsRoot = true }, new LevelData()});
		
		public string PrefabName = "New Destruction Prefab";

		public Mesh Mesh;

		public Material[] Materials;

		public Material CutMaterial;
		public bool SplitMeshIslands = true;
		public bool CreateCollisionMeshs = true;

		public PhysicMaterial PhysicMaterial;

		public SliceAreaSettings SliceAreaSettings = new SliceAreaSettings();
	}
}