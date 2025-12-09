using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; // Indispensable pour la Task 4
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool isAddressable; // Nouvelle option pour différencier les balles des cibles
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // --- NOUVEAU : Configuration Task 4 (Addressables) ---
    [Header("Task 4 - Addressable Target")]
    public string targetAddressableKey = "TargetPrefab"; // Le nom "Key" mis dans le groupe Addressable
    public int targetPoolSize = 10;
    private string currentLabel; // "PCVR" ou "Android"

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        // Détection automatique de la plateforme (Le fameux filtre)
        #if UNITY_ANDROID
            currentLabel = "Android";
        #else
            currentLabel = "PCVR";
        #endif
    }

    void Start()
    {
        // 1. On crée les pools classiques (ex: Balles)
        foreach (Pool pool in pools)
        {
            if (!pool.isAddressable && pool.prefab != null)
            {
                CreatePool(pool.tag, pool.prefab, pool.size);
            }
        }

        // 2. On lance le chargement intelligent de la Cible via Addressables
        LoadAndCreateTargetPool();
    }

    // Cette fonction télécharge le bon asset (PC ou Quest) puis crée le pool
    void LoadAndCreateTargetPool()
    {
        Debug.Log($"Chargement de la cible pour la plateforme : {currentLabel}...");

        // Correction : On utilise LoadAssetsAsync (pluriel) car on utilise une intersection de clés
        // On demande une liste d'objets qui ont À LA FOIS le nom "TargetPrefab" ET le label "PCVR" (ou "Android")
        Addressables.LoadAssetsAsync<GameObject>(new List<object> { targetAddressableKey, currentLabel }, 
            null, Addressables.MergeMode.Intersection).Completed += (op) => 
            {
                if (op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)
                {
                    // On prend le premier (et unique) résultat trouvé
                    GameObject loadedPrefab = op.Result[0];
                    
                    CreatePool("Target", loadedPrefab, targetPoolSize);
                    Debug.Log("Pool 'Target' créé avec succès via Addressables !");
                }
                else
                {
                    Debug.LogError("Erreur : Impossible de charger le TargetPrefab. Vérifie les Groupes Addressables.");
                }
            };
    }

    // Fonction utilitaire pour créer concrètement les objets
    void CreatePool(string tag, GameObject prefab, int size)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            
            // On force le tag pour être sûr que le script Target.cs fonctionne
            if(tag == "Target") obj.tag = "Target"; 
            
            objectPool.Enqueue(obj);
        }

        if(!poolDictionary.ContainsKey(tag))
        {
            poolDictionary.Add(tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            // Pas d'erreur critique ici, car le pool peut être encore en cours de chargement
            return null; 
        }

        if (poolDictionary[tag].Count == 0) return null;

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Reset de l'objet (Vitesse balle, PV cible, etc.)
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}