using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Pool[] pools;

    private void Awake()
    {
        pools = GetComponentsInChildren<Pool>();
        if (pools.Length == 0)
            Debug.Log("No pools found");
    }

    public Pool GetPool(string poolName)
    {
        foreach (Pool pool in pools)
            if (pool.name == poolName) return pool;
        return null;
    }
}
