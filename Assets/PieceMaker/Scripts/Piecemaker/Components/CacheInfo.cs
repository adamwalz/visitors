using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CacheInfo : MonoBehaviour
{
	public GameObject[] CachablePrefabs;
	
	void Start()
	{
        if (CachablePrefabs == null)
			return;

        foreach (var cachePrefab in CachablePrefabs)
            PrefabCache.Instance.AddToCache(cachePrefab);
	}
}