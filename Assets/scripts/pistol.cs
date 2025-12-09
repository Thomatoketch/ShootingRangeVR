using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    public float bulletSpeed = 20f;

    public void FireBullet()
    {
        //--- Instantiate a bullet and fire it forward
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.tag = "Bullet"; // S'assurer que le tag est bien "bullet"
        spawnedBullet.transform.position = firePoint.position;
        spawnedBullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        
        TriggerMuzzleFlash();

        //--- Destroy the bullet after 5 seconds
        Destroy(spawnedBullet, 5);
    }
}