using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    // Classe pour configurer les pools classiques (non-Addressables)
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    // Classe pour configurer les pools Addressables (VFX + Cibles)
    [System.Serializable]
    public class AddressablePoolData
    {
        public string tag; // Ex: "Target", "MuzzleFlash", "HitEffect"
        public string addressableKey; // La clé dans ton groupe Addressable
        public int size;
    }

    [Header("Configuration des Pools")]
    public List<Pool> standardPools; // Pour les balles classiques
    public List<AddressablePoolData> addressablePools; // Pour Cibles et VFX

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private string currentLabel;

    [Header("Task 2 - Optimisation Projectiles")]
    public float cleanupInterval = 0.5f;
    public float maxBulletDistance = 50f;
    private List<GameObject> activeBullets = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        #if UNITY_ANDROID
            currentLabel = "Quest";
        #else
            currentLabel = "PCVR";
        #endif
    }

    void Start()
    {
        // 1. Pools Standards (Balles)
        foreach (Pool pool in standardPools)
        {
            CreatePool(pool.tag, pool.prefab, pool.size);
        }

        // 2. Pools Addressables (Cibles + VFX)
        foreach (AddressablePoolData data in addressablePools)
        {
            LoadAndCreateAddressablePool(data);
        }

        // 3. Lancer le nettoyage automatique des balles
        StartCoroutine(CleanupBulletsRoutine());
    }

    void LoadAndCreateAddressablePool(AddressablePoolData data)
    {
        // Chargement avec Filtre (Label) + Clé
        Addressables.LoadAssetsAsync<GameObject>(new List<object> { data.addressableKey, currentLabel }, 
            null, Addressables.MergeMode.Intersection).Completed += (op) => 
            {
                if (op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)
                {
                    GameObject loadedPrefab = op.Result[0];
                    CreatePool(data.tag, loadedPrefab, data.size);
                    Debug.Log($"Pool Addressable créé : {data.tag}");
                }
                else
                {
                    Debug.LogError($"Erreur chargement Addressable : {data.tag} (Clé: {data.addressableKey})");
                }
            };
    }

    void CreatePool(string tag, GameObject prefab, int size)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            
            // On force le tag pour être sûr que les scripts fonctionnent
            if(tag == "Target") obj.tag = "Target"; 
            if(tag == "Bullet") obj.tag = "Bullet";
            
            objectPool.Enqueue(obj);
        }

        if(!poolDictionary.ContainsKey(tag))
        {
            poolDictionary.Add(tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag) || poolDictionary[tag].Count == 0)
        {
            return null; // Pool pas encore prêt ou vide
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Reset spécifique si l'objet implémente l'interface (Balles, FX, etc.)
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        if (tag == "Bullet")
        {
            activeBullets.Add(objectToSpawn);
        }

        return objectToSpawn;
    }

    IEnumerator CleanupBulletsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cleanupInterval);

            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                GameObject bullet = activeBullets[i];

                if (!bullet.activeInHierarchy)
                {
                    activeBullets.RemoveAt(i);
                    continue;
                }

                if (Vector3.Distance(transform.position, bullet.transform.position) > maxBulletDistance)
                {
                    bullet.SetActive(false);
                    activeBullets.RemoveAt(i);
                }
            }
        }
    }
}