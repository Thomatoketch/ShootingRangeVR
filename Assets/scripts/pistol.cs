using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; // Indispensable
using UnityEngine.ResourceManagement.AsyncOperations;

public class Pistol : MonoBehaviour
{
    [Header("Références au Gameplay")]
    public Transform firePoint; // L'objet vide pour déterminer la sortie du canon
    public GameObject bullet; // Votre préfabriqué de projectile
    public float bulletSpeed = 20f;

    [Header("Addressables Keys")]
    public string muzzleFlashKey = "MuzzleFlashFX";
    private string currentLabel;

    void Start()
    {
        // Une manière simple et efficace de détecter si on est sur une build Android (typiquement pour Quest)
        // Cette portion de code ne sera incluse que si la plateforme de build est Android.
        #if UNITY_ANDROID
            currentLabel = "Quest"; 
        #else
            currentLabel = "PCVR";
        #endif
    }

    public void FireBullet()
    {
        // Remplacement de Instantiate par le PoolManager
        GameObject spawnedBullet = ObjectPoolManager.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
        
        if (spawnedBullet != null)
        {
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            // On applique la force directement (la vélocité a été reset par OnObjectSpawn)
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }

        TriggerMuzzleFlash();
        // Plus de Destroy ici, c'est géré par PooledBullet
    }

    private void TriggerMuzzleFlash()
    {
        // Chargement dynamique avec Addressables
        // Note: On combine Key et Label pour filtrer
        Addressables.LoadAssetsAsync<GameObject>(new [] { muzzleFlashKey, currentLabel }, 
            (obj) => {
                // Cette fonction est appelée quand l'asset est chargé
                SpawnFX(obj);
            }, Addressables.MergeMode.Intersection); 
    }

    private void SpawnFX(GameObject fxPrefab)
    {
        if (fxPrefab == null) return;

        // On instancie l'asset chargé
        GameObject flashInstance = Instantiate(fxPrefab, firePoint.position, firePoint.rotation);
        
        // Nettoyage standard (Addressables gère la mémoire de l'asset source, 
        // mais l'instance doit être détruite ou poolée)
        Destroy(flashInstance, 2f); 
    }
}