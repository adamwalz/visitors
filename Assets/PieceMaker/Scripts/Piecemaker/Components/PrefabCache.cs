using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PrefabCache : MonoBehaviour
{
	private static PrefabCache instance = null;
	public static PrefabCache Instance
	{
	    get
	    {
	        if (instance == null)
			{
	            instance = FindObjectOfType(typeof(PrefabCache)) as PrefabCache;
				if (instance == null)
				{
					var go = new GameObject("PrefabCache Holder");
					go.hideFlags = HideFlags.HideAndDontSave;
					instance = go.AddComponent<PrefabCache>();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.playmodeStateChanged += ApplicationPlaymodeStateChanged;
#endif
				}
			}
	        return instance;
	    }
	}

    private static void Destroy()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playmodeStateChanged -= ApplicationPlaymodeStateChanged;
#endif
        GameObject.DestroyImmediate(instance);
        instance = null;
    }

#if UNITY_EDITOR
    private static void ApplicationPlaymodeStateChanged()
    {
        var isPlaying = UnityEditor.EditorApplication.isPlaying;
        var isChangingOrPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
        if (!(isPlaying && !isChangingOrPlaying))
            return;
        Destroy();
    }
#endif
	
	static readonly float cacheIntervall = 0.2f;
	static readonly float shortCacheIntervall = 0.05f;

    Dictionary<GameObject, CacheData> cachedPrefabs = new Dictionary<GameObject, CacheData>();
	bool firstTimeCaching = true;
	float timeForNextEnsure = 0.0f;
	
    void OnLevelWasLoaded(int level)
    {
        Destroy();
    }

	void OnDestroy()
	{
        foreach (var val in cachedPrefabs.Values)
            val.Dispose();
        cachedPrefabs.Clear();
	}

	void Update()
	{
		timeForNextEnsure -= Time.deltaTime;
		if (timeForNextEnsure <= 0)
		{
			timeForNextEnsure = cacheIntervall;
			EnsureChache(firstTimeCaching);
			firstTimeCaching = false;
		}
	}
	
	class CacheData : System.IDisposable
	{
		public GameObject Prefab;
		public int MaxFill = 10;
		public List<GameObject> Cache = new List<GameObject>();
		
		public bool TryEnsureCache()
		{
			if (Cache.Count < MaxFill)
			{
				var entry = Object.Instantiate(Prefab) as GameObject;
                entry.hideFlags = HideFlags.HideAndDontSave;
				Cache.Add(entry);
                entry.SetActiveRecursively(false);
				
				return true;
			}
			return false;
		}
		
		public GameObject Create(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			if (Cache.Count == 0)
				TryEnsureCache();
			
			var entry = Cache[0];
			Cache.RemoveAt(0);
			entry.SetActiveRecursively(true);
			entry.hideFlags = (HideFlags)0x0;
			entry.transform.position = position;
			entry.transform.rotation = rotation;
			entry.transform.localScale = scale;
			
			return entry;
		}
		
		public void Dispose()
		{
			foreach (var entry in Cache)
				Object.DestroyImmediate(entry);
			Cache.Clear();
		}
	}
	
	
	public void AddToCache(GameObject prefab)
	{
        AddToCache(prefab, 10);
	}

    public void AddToCache(GameObject prefab, int maxFill)
	{
		CacheData cacheData;
        if (!cachedPrefabs.TryGetValue(prefab, out cacheData))
		{
			cacheData = new CacheData()
			{
				MaxFill = maxFill,
                Prefab = prefab
			};
			cacheData.TryEnsureCache();
            cachedPrefabs.Add(prefab, cacheData);
		}
	}

    public GameObject CreateInstance(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		CacheData cacheData;
        if (!cachedPrefabs.TryGetValue(prefab, out cacheData))
		{
            AddToCache(prefab);
            return CreateInstance(prefab, position, rotation, scale);
		}
		timeForNextEnsure = cacheIntervall;
		return cacheData.Create(position, rotation, scale);
	}
	
	public void EnsureChache(bool cacheFull)
	{
		if (cacheFull)
		{
			foreach (var pair in cachedPrefabs)
				while(pair.Value.TryEnsureCache());
		}
		else
		{
			var cachedSomething = false;
			foreach (var pair in cachedPrefabs)
			{
				if (pair.Value.TryEnsureCache())
				{
					cachedSomething = true;
					return;
				}
			}
			if (cachedSomething)
				timeForNextEnsure = shortCacheIntervall;
		}
	}
}