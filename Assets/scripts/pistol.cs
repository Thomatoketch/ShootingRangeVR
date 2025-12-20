using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic; // Ajouté pour la List

public class Pistol : MonoBehaviour
{
    [Header("Références au Gameplay")]
    public Transform firePoint; 
    // Suppression de 'public GameObject bullet' (inutile avec le pooling)
    public float bulletSpeed = 20f;

    [Header("Addressables Keys")]
    public string muzzleFlashKey = "MuzzleFlashFX";
    private string currentLabel;
    private GameObject loadedMuzzleFlashPrefab; // On stocke le prefab ici

    void Start()
    {
        #if UNITY_ANDROID
            currentLabel = "Quest"; 
        #else
            currentLabel = "PCVR";
        #endif

        // On précharge l'effet dès le début du jeu
        LoadMuzzleFlashAsset();
    }

    void LoadMuzzleFlashAsset()
    {
        Addressables.LoadAssetsAsync<GameObject>(new List<object> { muzzleFlashKey, currentLabel }, 
            null, Addressables.MergeMode.Intersection).Completed += (op) => 
            {
                if(op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)
                {
                    loadedMuzzleFlashPrefab = op.Result[0];
                }
            };
    }

    public void FireBullet()
    {
        // Utilisation du Pool
        GameObject spawnedBullet = ObjectPoolManager.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);
        
        if (spawnedBullet != null)
        {
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            }
        }

        // Appel de l'effet visuel optimisé
        PlayMuzzleFlash();
    }

    private void PlayMuzzleFlash()
    {
        if (loadedMuzzleFlashPrefab != null)
        {
            // Idéalement, on devrait aussi pooler les FX, mais Instantiate ici est déjà mieux que LoadAssetsAsync + Instantiate
            Instantiate(loadedMuzzleFlashPrefab, firePoint.position, firePoint.rotation);
            // Pas de Destroy() ici : assure-toi que le prefab MuzzleFlash a le script "AutoDisableFX" dessus.
        }
    }
}