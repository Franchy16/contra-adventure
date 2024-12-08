using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int dir;
    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime * Vector3.right;
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
