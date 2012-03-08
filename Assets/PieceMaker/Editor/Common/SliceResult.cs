using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using MeshUtilities;

namespace Piecemaker
{
	public class SliceResult
	{
		static readonly string destructionPath = "DestructionPrefabs";
		static readonly string plainPrefabPath = "Assets/Resources/" + destructionPath;
		
		public Settings Settings;
		public LevelData LevelData;
		public VirtualMesh Mesh;
		public VirtualMesh CollisionMesh;
		public int Level;
		public int ShardId;
		public List<SliceResult> Children = new List<SliceResult>();

        public class SliceResultTempData : System.IDisposable
        {
            public string FinalPrefabPath;
            public GameObject NewGameObject;
            public GameObject ParentHull;
            public string FinalPrefabName;
            public List<GameObject> Prefabs;
            public GameObject NewHullObject;
            public SliceResult MainResult;

            #region IDisposable Members

            public void Dispose()
            {
                if (NewGameObject != null)
                    Object.DestroyImmediate(NewGameObject);
                NewGameObject = null;

                if (NewHullObject != null)
                    Object.DestroyImmediate(NewHullObject);
                NewHullObject = null;
            }

            #endregion
        }
		
		public SliceResult(VirtualMesh mesh, VirtualMesh collisionMesh, int level, int shardId, Settings settings, LevelData levelData)
		{
			Settings = settings;
			LevelData = levelData;
			Mesh = mesh;
			CollisionMesh = collisionMesh;
			Level = level;
			ShardId = shardId;
		}

        public void Export(GameObject parentHull, string finalPrefabName, List<GameObject> prefabs, List<SliceResultTempData> sliceResultTempDatas, ref int assetIndex)
        {
            var result = new SliceResultTempData()
            {
                ParentHull = parentHull,
                FinalPrefabName = finalPrefabName,
                Prefabs = prefabs,
                MainResult = this
            };

            ExportSelf(parentHull, finalPrefabName, result);

            sliceResultTempDatas.Add(result);
            assetIndex++;
        }

        public bool ExportChildren(SliceResultTempData data, List<SliceResultTempData> sliceResultTempDatas, ref int assetIndex)
        {
            if (Children.Any())
            {
                var child = Children[0];
                Children.Remove(child);

                child.Export(data.NewHullObject, data.FinalPrefabName, data.Prefabs, sliceResultTempDatas, ref assetIndex);

                if (Children.Any())
                    return true;

                Object prefab = AssetDatabase.LoadAssetAtPath(data.FinalPrefabPath, typeof(GameObject));
                if (!prefab)
                    prefab = PrefabUtility.CreateEmptyPrefab(data.FinalPrefabPath);

                EditorUtility.ReplacePrefab(data.NewHullObject, prefab, ReplacePrefabOptions.Default);
                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                AssetDatabase.StopAssetEditing();
                AssetDatabase.ImportAsset(data.FinalPrefabPath);
                AssetDatabase.StartAssetEditing();

                var newPrefabGO = (GameObject)AssetDatabase.LoadAssetAtPath(data.FinalPrefabPath, typeof(GameObject));

                foreach (var property in LevelData.PropertyApplicators)
                    property.ApplySpawnOnDestructPrefab(!LevelData.IsRoot ? data.NewGameObject : data.ParentHull, newPrefabGO);

                data.Prefabs.Add(newPrefabGO);

                //Object.DestroyImmediate(data.NewHullObject);
            }
            return false;
        }

        private void ExportSelf(GameObject parentHull, string finalPrefabName, SliceResultTempData data)
        {
            var meshPath = plainPrefabPath + "/" + finalPrefabName;
            var shardPrefabName = GetShardPrefabName(finalPrefabName, ShardId, Level) + ".prefab";
            data.FinalPrefabPath = meshPath + "/" + shardPrefabName;
            //data.PrefabName = destructionPath + "/" + finalPrefabName + "/" + shardPrefabName.Replace(".prefab", string.Empty);

            data.NewGameObject = null;

            if (!LevelData.IsRoot)
            {
                var newMeshPath = meshPath + "/" + GetShardName(ShardId, Level) + "Model.obj";
                var newRealMesh = Mesh.ToMesh(1.0f);
                newRealMesh.Optimize();

                AssetDatabase.CreateAsset(newRealMesh, newMeshPath);
                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                var meshImporter = ModelImporter.GetAtPath(newMeshPath) as ModelImporter;
                if (meshImporter != null)
                {
                    meshImporter.normalImportMode = ModelImporterTangentSpaceMode.Calculate;
                    meshImporter.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
                    //meshImporter.addCollider = CollisionMesh == null;
                }

                AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                if (CollisionMesh != null)
                {
                    var newCollisionMeshPath = meshPath + "/" + GetShardName(ShardId, Level) + "CollisionModel.obj";
                    var newCollisionRealMesh = CollisionMesh.ToMesh(1.0f);
                    newCollisionRealMesh.RecalculateNormals();
                    newCollisionRealMesh.Optimize();

                    AssetDatabase.CreateAsset(newCollisionRealMesh, newCollisionMeshPath);
                    AssetDatabase.SaveAssets();
                    //AssetDatabase.Refresh();
                    data.NewGameObject = CreateChildGameObjectFromMesh(newRealMesh, newCollisionRealMesh);
                }
                else
                    data.NewGameObject = CreateChildGameObjectFromMesh(newRealMesh, newRealMesh);
                data.NewGameObject.transform.parent = parentHull.transform;
            }

            if (Children.Any())
            {
                data.NewHullObject = new GameObject(GetShardName(ShardId, Level) + " Hull");
                data.NewHullObject.AddComponent<ShardContainer>();

                foreach (var property in Children.First().LevelData.PropertyApplicators)
                    property.ApplyOnParentHull(data.NewHullObject, Settings);
            }
        }
		
		string GetShardPrefabName(string finalPrefabName, int sliceCnt, int level)
		{
			return finalPrefabName+ "_" + GetShardName(sliceCnt, level);
		}
		
		string GetShardName(int sliceCnt, int level)
		{
			return string.Format("Shard{0}_level{1}", sliceCnt, level);
		}
		
		GameObject CreateChildGameObjectFromMesh(Mesh shardMesh, Mesh collisionMesh)
		{
			GameObject gameObject;
			if (LevelData.PrefabTemplate != null)
				gameObject = Object.Instantiate(LevelData.PrefabTemplate) as GameObject;
			else
				gameObject = new GameObject();
			gameObject.name = GetShardName(ShardId, Level);
			
			foreach (var property in LevelData.PropertyApplicators)
				property.ApplyOnTarget(gameObject, ApplyTargetType.Shard, shardMesh, collisionMesh, Settings);
			if (gameObject.renderer != null)
				gameObject.renderer.enabled = false;
			
			return gameObject;
		}
	}
}