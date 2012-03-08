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
	public class PiecemakerCore : System.IDisposable
	{		
		public static readonly string destructionPath = "DestructionPrefabs";
		public static readonly string plainPrefabPath = "Assets/Resources/" + destructionPath;
		
		Piecemaker.Settings settings;
		int assetCnt = 0;
        int assetIndex = 0;
		string finalPrefabName;
        List<GameObject> createdPrefabs = new List<GameObject>();
        List<SliceResult.SliceResultTempData> processedSliceResultTempDatas = new List<SliceResult.SliceResultTempData>();
        List<SliceResult.SliceResultTempData> sliceResultTempDatas = new List<SliceResult.SliceResultTempData>();
		Thread processThread = null;
		SliceResult rootSliceResult = null;
        int currentSliceCount = 0;
        GameObject rootGameObject;
		
		ManualResetEvent finishedEvent;
		ManualResetEvent cancelEvent;
		System.Exception processingError;
		
		Stopwatch sliceStopwatch = new Stopwatch();
		Stopwatch assetCreationStopwatch = new Stopwatch();
		
		System.Action<string> cleanDirectoryFunction;
		
		object syncRoot = new object();
		
		public PiecemakerCore(Piecemaker.Settings settings, System.Action<string> cleanDirectoryFunction)
		{
			this.settings = settings;
			this.cleanDirectoryFunction = cleanDirectoryFunction;
		}
		
		public bool CreateInBackground()
		{
			if (processThread != null)
				return false;

			if (!PreCreate ())
				return false;

			var threadStart = new ParameterizedThreadStart(this.Process);
			processThread = new Thread(threadStart);
			
			processThread.Start(new VirtualMesh(settings.Mesh, 1.0f));
            return true;
		}

		public void Create()
		{
			if (!PreCreate ())
				return;
			Process(new VirtualMesh(settings.Mesh, 1.0f));
			CheckProcess();
		}
		
		public bool CanCreate()
		{
			lock (syncRoot)
			{
				return processThread == null;
			}
		}
		
		public void Cancel()
		{
            if (cancelEvent != null)
            {
                cancelEvent.Set();
            }
		}
		
		public void CheckProcess()
		{
			if (finishedEvent.WaitOne(0))
			{
				if (processingError != null)
					FailedProcessing();
			}
		}

        public int CurrentSliceIndex()
        {
            lock (syncRoot)
                return currentSliceCount;
        }

        public int SliceCount()
        {
            lock (syncRoot)
            {
                var count = 1;
                for (int i = 1; i < settings.LevelData.Count; ++i)
                {
                    var lvl = settings.LevelData[i];
                    count *= ((1 + lvl.SliceXCount) * (1 + lvl.SliceYCount) * (1 + lvl.SliceZCount));
                }
                return count;
            }
        }

        public int AssetsCreateCount()
        {
            return assetCnt;
        }

        public int AssetCreateIndex()
        {
            return assetIndex;
        }
		
		public bool IsFinished()
		{
			return finishedEvent == null || finishedEvent.WaitOne(0);
		}
		
		private bool PreCreate()
		{
			finalPrefabName = settings.PrefabName.Replace(" ", "_");
			assetCnt = 1;
            assetIndex = 0;
            createdPrefabs.Clear();
			
			if (settings.Mesh == null)
			{
				EditorUtility.DisplayDialog("Error", "Please select a mesh first.", "Ok");
				return false;
			}
				
			if (!Directory.Exists(plainPrefabPath))
				Directory.CreateDirectory(plainPrefabPath);
			var prefabPath = plainPrefabPath + "/" + finalPrefabName + ".prefab";
			var templatePrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
			if (templatePrefab != null) 
				if (!EditorUtility.DisplayDialog("Are you sure?", "The prefab already exists. Do you want to overwrite it?", "Yes", "No"))
					return false;

            try
            {
                var meshPath = plainPrefabPath + "/" + finalPrefabName;
                if (Directory.Exists(meshPath))
                    cleanDirectoryFunction(meshPath);
                else
                    Directory.CreateDirectory(meshPath);
            }
            catch (System.Exception e)
            {
                processingError = e;
                FailedProcessing();
                return false;
            }

			settings.SliceAreaSettings.SubMeshId = settings.CutMaterial == null ? 
														settings.Materials.Length - 1 : 
														settings.Materials.Length;
			
			cancelEvent = new ManualResetEvent(false);
			finishedEvent = new ManualResetEvent(false);
            currentSliceCount = 0;
			
			return true;
		}
		
		private void Slice(VirtualMesh sliceMesh, SliceResult parent, int index)
		{
            if (cancelEvent.WaitOne(0))
                return;
			
			var levelData = settings.LevelData[index];
			var slicer = new Slicer();

			var sliceSettings = new MultiSliceSettings()
			{
				SlicePlanes = slicer.CreateSlicePlanes(sliceMesh.Bounds, levelData.SliceXCount, levelData.SliceYCount, levelData.SliceZCount, levelData.SliceChaos),
				SliceAreaSettings = settings.SliceAreaSettings
			};

            var meshs = slicer.Slice(sliceMesh, sliceSettings, syncRoot, ref currentSliceCount);
			if (cancelEvent.WaitOne(0))
                return;
			
			if (settings.SplitMeshIslands)
				meshs = meshs.SelectMany(m => MeshUtilities.Tools.SplitIsolatedMeshAreas(m)).ToArray();
			
			if (cancelEvent.WaitOne(0))
                return;

			foreach (var newMesh in meshs)
			{
				VirtualMesh collisionMesh = null;
                if (settings.CreateCollisionMeshs)
                    collisionMesh = MeshUtilities.Tools.CreateHull(newMesh, 255);
				var sliceResult = new SliceResult(newMesh, collisionMesh, index, assetCnt++, settings, levelData);
				
				if (index + 1 < settings.LevelData.Count)
				{
					Slice(newMesh, sliceResult, index + 1);
					if (cancelEvent.WaitOne(0))
                        return;
				}
				
				parent.Children.Add(sliceResult);
			}
		}
		
		private GameObject CreateGameObject(Mesh shardMesh, LevelData levelData)
		{
			var isRoot = levelData.IsRoot;
			
			GameObject gameObject;
			if (levelData.PrefabTemplate != null)
			{
				gameObject = Object.Instantiate(levelData.PrefabTemplate) as GameObject;
				var meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter != null)
					meshFilter.sharedMesh = shardMesh;
				var meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					var materials = settings.Materials.ToList();
					if (settings.CutMaterial != null && !isRoot)
						materials.Add(settings.CutMaterial);
					meshRenderer.sharedMaterials = materials.ToArray();
				}
				
				if (gameObject.collider != null)
				{
					if (gameObject.collider is MeshCollider)
					{					
						var col = gameObject.collider as MeshCollider;
							
						if (shardMesh.triangles.Length/3 > 255)
							Object.Destroy(col);
						else
						{
							try
							{
								col.convex = true;
								col.sharedMesh = shardMesh;
							}
							catch
							{
								gameObject.AddComponent<BoxCollider>();
							}
						}
					}
					if (gameObject.collider is BoxCollider)
					{
						var col = gameObject.collider as BoxCollider;
						col.size = shardMesh.bounds.size;
						col.center = shardMesh.bounds.center;
					}
					else if (gameObject.collider is SphereCollider)
					{
						var col = gameObject.collider as SphereCollider;
						col.radius = shardMesh.bounds.extents.magnitude;
						col.center = shardMesh.bounds.center;
					}
					else if (gameObject.collider is CapsuleCollider)
					{
						var col = gameObject.collider as CapsuleCollider;
						col.height = shardMesh.bounds.extents.y * 2;
						col.radius = Mathf.Max(shardMesh.bounds.extents.x, shardMesh.bounds.extents.z);
						col.center = shardMesh.bounds.center;
					}
					gameObject.collider.sharedMaterial = settings.PhysicMaterial;
				}
			}
			else
				gameObject = new GameObject();
			
			foreach (var property in levelData.PropertyApplicators)
				property.ApplyOnTarget(gameObject, isRoot ? ApplyTargetType.Root : ApplyTargetType.Shard, shardMesh, shardMesh, settings);
			foreach (var property in levelData.PropertyApplicators)
				property.ApplyOnParentHull(gameObject, settings);
			if (!isRoot && gameObject.renderer != null)
				gameObject.renderer.enabled = false;
				
			return gameObject;
		}
		
		private void Process(System.Object param)
		{
			sliceStopwatch.Start();
			
			try
			{
				var mesh = param as VirtualMesh;
				rootSliceResult = new SliceResult(mesh, null, 0, 0, settings, settings.LevelData[0]);
				Slice(mesh, rootSliceResult, 1);
			}
			catch (System.Exception e)
			{
				processingError = e;
			}
			finally
			{
				finishedEvent.Set();
			}
			
			sliceStopwatch.Stop();
		}
		
		private void CleanUp()
		{
			lock(syncRoot)
			{
                if (processThread != null)
                {
                    if (processThread.IsAlive)
                        processThread.Abort();
                }
                
				processThread = null;
                if (cancelEvent != null)
				    cancelEvent.Close();
				cancelEvent = null;
                if (finishedEvent != null)
                    finishedEvent.Close();
				finishedEvent = null;
				processingError = null;
			}
		}
		
		private void FailedProcessing()
		{
			EditorUtility.DisplayDialog("Error", "An error ocoured while executing the operation.\n\n" + processingError.ToString(), "Ok");
            UnityEngine.Debug.LogError(processingError);
			CleanUp();
		}

        public void ProcessExport()
        {
            while (ProcessExportStep()) ;
        }

        private bool ProcessExportStep()
        {
            if (rootGameObject == null)
            {
                if (processingError != null)
                {
                    FailedProcessing();
                    return false;
                }
                FinishedProcessing();
                return true;
            }
            
            if (sliceResultTempDatas.Count > 0)
            {
                var data = sliceResultTempDatas.Last();

                if (!data.MainResult.ExportChildren(data, sliceResultTempDatas, ref assetIndex))
                {
                    sliceResultTempDatas.Remove(data);
                    processedSliceResultTempDatas.Add(data);
                }
                return true;
            }
            FinishExport();
            return false;
        }

        private void FinishExport()
        {
            try
            {
                var cacheInfo = rootGameObject.AddComponent<CacheInfo>();
                //cacheInfo.CachablePrefabNames = createdPrefabNames.ToArray();
                cacheInfo.CachablePrefabs = createdPrefabs.ToArray();
            }
            catch (System.Exception e)
            {
                if (rootGameObject != null)
                    Object.DestroyImmediate(rootGameObject);

                EditorUtility.DisplayDialog("Error", "An error ocoured while executing the operation, please verify your settings and try again.\n\n" + e.ToString(), "Ok");
                UnityEngine.Debug.Log(e.ToString());
                AssetDatabase.StopAssetEditing();
                return;
            }
            var prefabPath = plainPrefabPath + "/" + finalPrefabName + ".prefab";
            Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (!prefab)
                prefab = EditorUtility.CreateEmptyPrefab(prefabPath);

            EditorUtility.ReplacePrefab(rootGameObject, prefab, ReplacePrefabOptions.ReplaceNameBased);
            AssetDatabase.SaveAssets();
            AssetDatabase.StopAssetEditing();

            AssetDatabase.Refresh();
            

            Object.DestroyImmediate(rootGameObject);

            assetCreationStopwatch.Stop();

            var resultText = string.Format("Created '{0}' with {1} shards.\n\nElapsed time for slicing: {2}\nElapsed time for asset creation: {3}",
                                           finalPrefabName,
                                           assetCnt,
                                           sliceStopwatch.Elapsed,
                                           assetCreationStopwatch.Elapsed);

            EditorUtility.DisplayDialog("Finished", resultText, "Ok");
        }
		
		private void FinishedProcessing()
		{
			CleanUp();

			assetCreationStopwatch.Start();
            AssetDatabase.StartAssetEditing();
			try
			{
                rootGameObject = CreateGameObject(settings.Mesh, settings.LevelData[0]);
                rootSliceResult.Export(rootGameObject, finalPrefabName, createdPrefabs, sliceResultTempDatas, ref assetIndex);
			}
			catch (System.Exception e)
			{
                if (rootGameObject != null)
                    Object.DestroyImmediate(rootGameObject);
				
				EditorUtility.DisplayDialog("Error", "An error ocoured while executing the operation, please verify your settings and try again.\n\n" + e.ToString(), "Ok");
				UnityEngine.Debug.Log(e.ToString());
			}
		}

        #region IDisposable Members

        public void Dispose()
        {
            Cancel();
            if (processThread != null)
            {
                if (processThread.IsAlive)
                    processThread.Interrupt();
                CleanUp();
            }
            if (rootGameObject != null)
                Object.DestroyImmediate(rootGameObject);
            rootGameObject = null;

            foreach (var entry in sliceResultTempDatas)
                entry.Dispose();
            sliceResultTempDatas.Clear();

            foreach (var entry in processedSliceResultTempDatas)
                entry.Dispose();
            processedSliceResultTempDatas.Clear();
        }

        #endregion
    }
}