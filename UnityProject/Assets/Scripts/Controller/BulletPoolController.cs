using System.Collections.Generic;
using UnityEngine;

public class BulletPoolController : MonoBehaviour {

    public GameObject bulletProp;
    public List<GameObject> bulletPool;
    public int poolSize = 10;

    private void Start() {
        bulletPool = new List<GameObject>();
        for(int i = 0; i < poolSize; i++)
        {
            InstantiateNewBullet();
        }
    }

    public GameObject GetBullet() {
        var nextBullet = bulletPool.Find(bullet => !bullet.activeInHierarchy);
        return nextBullet != null ? nextBullet : InstantiateNewBullet();
    }

    private GameObject InstantiateNewBullet() {
        var tmpBullet = Instantiate(bulletProp);
        tmpBullet.SetActive(false);
        bulletPool.Add(tmpBullet);
        return tmpBullet;
    }
}
