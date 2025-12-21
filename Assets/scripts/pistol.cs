using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Références")]
    public Transform firePoint; 
    public float bulletSpeed = 20f;

    public void FireBullet()
    {
        // 1. Spawn de la balle via le Pool
        GameObject spawnedBullet = ObjectPoolManager.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
        
        if (spawnedBullet != null)
        {
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            }
        }

        // 2. Spawn du MuzzleFlash via le Pool (Addressable géré par le Manager)
        ObjectPoolManager.Instance.SpawnFromPool("MuzzleFlash", firePoint.position, firePoint.rotation);
    }
}