using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBullets : Pool
{
    public static PoolBullets instance;
    private float amountOfPool;
    public bool canPool = true;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        maxObjects = 10;
    }
    public void AddPool()
    {
        amountOfPool++;

        if (amountOfPool >= maxObjects)
            canPool = false;
        else
            canPool = true;
    }

    public Bullet GetBullet()
    {
        Bullet[] bullets = GetComponentsInChildren<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            if (!bullet.gameObject.activeSelf)
                return bullet;
        }
        bullets[0].gameObject.SetActive(false);
        return bullets[0];
    }
}
